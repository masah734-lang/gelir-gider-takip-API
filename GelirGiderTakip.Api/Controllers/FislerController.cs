using GelirGiderTakip.Api.Data;
using GelirGiderTakip.Api.DTOs.Fisler;
using GelirGiderTakip.Api.Enums;
using GelirGiderTakip.Api.Models;
using GelirGiderTakip.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;
using System.Text.RegularExpressions;
using TesseractOCR;
using TesseractOCR.Enums;

namespace GelirGiderTakip.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/fisler")]
    public class FislerController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ButceBildirimServisi _butceBildirimServisi;

        public FislerController(
            AppDBContext context,
            IWebHostEnvironment environment,
             ButceBildirimServisi butceBildirimServisi)
        {
            _context = context;
            _environment = environment;
            _butceBildirimServisi = butceBildirimServisi;
        }

        private async Task FinansalIslemSenkronizeEt(
         Fis fis,
        int kullaniciId)
        {
            // OCR tutari okuyamamissa simdilik islem olusturma.
            // Kullanici duzeltince tekrar bu metot calisacak.
            if (!fis.AlgilananTutar.HasValue ||
                fis.AlgilananTutar.Value <= 0)
            {
                fis.Durum = FisDurumu.Bekliyor;
                return;
            }

            var mevcutIslem = await _context.FinansalIslemler
                .FirstOrDefaultAsync(islem =>
                    islem.FisId == fis.Id &&
                    islem.KullaniciId == kullaniciId);

            // Fis tarihi okunamadiysa gecici olarak yuklenme tarihini kullan.
            var islemTarihi =
                fis.AlgilananTarih ?? fis.YuklenmeTarihi;

            if (mevcutIslem == null)
            {
                var yeniIslem = new FinansalIslem
                {
                    KullaniciId = kullaniciId,
                    Tur = IslemTuru.Gider,
                    Tutar = fis.AlgilananTutar.Value,
                    IslemTarihi = islemTarihi,

                    Aciklama = string.IsNullOrWhiteSpace(
                        fis.AlgilananIsletmeAdi)
                        ? "Fis ile eklenen gider"
                        : $"Fis - {fis.AlgilananIsletmeAdi}",

                    // Kategori tahmin edilemiyorsa simdilik null.
                    KategoriId = null,

                    FisId = fis.Id,

                    OlusturulmaTarihi = DateTime.UtcNow
                };

                _context.FinansalIslemler.Add(yeniIslem);
            }
            else
            {
                // Kullanici OCR sonucunu duzeltmisse
                // finansal islem de otomatik guncellensin.
                mevcutIslem.Tutar = fis.AlgilananTutar.Value;
                mevcutIslem.IslemTarihi = islemTarihi;
                mevcutIslem.GuncellenmeTarihi = DateTime.UtcNow;
            }

            fis.Durum = FisDurumu.Tamamlandi;
            fis.IslenmeTarihi = DateTime.UtcNow;
        }

        [HttpGet]
        public async Task<ActionResult<List<FisGetDto>>> GetFisler()
        {
            var kullaniciId = KullaniciIdGetir();

            var fisler = await _context.Fisler
                .AsNoTracking()
                .Where(fis => fis.KullaniciId == kullaniciId)
                .OrderByDescending(fis => fis.YuklenmeTarihi)
                .Select(fis => new FisGetDto
                {
                    Id = fis.Id,
                    OrijinalDosyaAdi = fis.OrijinalDosyaAdi,
                    DosyaYolu = fis.DosyaYolu,
                    AlgilananIsletmeAdi = fis.AlgilananIsletmeAdi,
                    AlgilananTutar = fis.AlgilananTutar,
                    AlgilananTarih = fis.AlgilananTarih,
                    YuklenmeTarihi = fis.YuklenmeTarihi,
                    DogrulanmaTarihi = fis.DogrulanmaTarihi,
                    HamOcrMetni = fis.HamOcrMetni
                })
                .ToListAsync();

            return Ok(fisler);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<FisGetDto>> GetFis(int id)
        {
            var kullaniciId = KullaniciIdGetir();

            var fis = await _context.Fisler
                .AsNoTracking()
                .Where(fis =>
                    fis.Id == id &&
                    fis.KullaniciId == kullaniciId)
                .Select(fis => new FisGetDto
                {
                    Id = fis.Id,
                    OrijinalDosyaAdi = fis.OrijinalDosyaAdi,
                    DosyaYolu = fis.DosyaYolu,
                    AlgilananIsletmeAdi = fis.AlgilananIsletmeAdi,
                    AlgilananTutar = fis.AlgilananTutar,
                    AlgilananTarih = fis.AlgilananTarih,
                    YuklenmeTarihi = fis.YuklenmeTarihi,
                    DogrulanmaTarihi = fis.DogrulanmaTarihi,
                    HamOcrMetni = fis.HamOcrMetni
                })
                .FirstOrDefaultAsync();

            if (fis == null)
            {
                return NotFound("Fis bulunamadi.");
            }

            return Ok(fis);
        }

        

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> PostFis(
            [FromForm] FisPostDto dto)
        {
            var kullaniciId = KullaniciIdGetir();

            // Dosya secilmis mi?
            if (dto.Dosya == null || dto.Dosya.Length == 0)
            {
                return BadRequest("Dosya secilmedi.");
            }

            // Izin verilen dosya uzantilari
            var izinliUzantilar = new[]
            {
                ".jpg",
                ".jpeg",
                ".png"
            };

            var uzanti = Path.GetExtension(
                dto.Dosya.FileName).ToLowerInvariant();

            if (!izinliUzantilar.Contains(uzanti))
            {
                return BadRequest(
                    "Sadece JPG, JPEG veya PNG dosyasi yuklenebilir.");
            }

            // Uploads/Fisler klasorunu olustur
            var klasorYolu = Path.Combine(
                _environment.ContentRootPath,
                "Uploads",
                "Fisler");

            Directory.CreateDirectory(klasorYolu);

            // Dosyaya benzersiz isim ver
            var yeniDosyaAdi =
                $"{Guid.NewGuid()}{uzanti}";

            var tamDosyaYolu = Path.Combine(
                klasorYolu,
                yeniDosyaAdi);

            // Dosyayi diske kaydet
            await using (var stream = new FileStream(
                tamDosyaYolu,
                FileMode.Create))
            {
                await dto.Dosya.CopyToAsync(stream);
            }

            // Tessdata klasorunun yolu
            var tessdataYolu = Path.Combine(
                _environment.ContentRootPath,
                "tessdata");

            // Bu degiskenleri try disinda tanimliyoruz.
            // Boylece try bittikten sonra da kullanabiliriz.
            string ocrMetni = string.Empty;
            string? algilananIsletmeAdi = null;
            DateTime? algilananTarih = null;
            decimal? algilananTutar = null;

            try
            {
                using var engine = new Engine(
                    tessdataYolu,
                    Language.Turkish,
                    EngineMode.Default);

                using var image =
                    TesseractOCR.Pix.Image.LoadFromFile(
                        tamDosyaYolu);

                using var page = engine.Process(image);

                // OCR ham metni
                ocrMetni = page.Text ?? string.Empty;

                // OCR metninden bilgileri cikarmaya calis
                algilananIsletmeAdi =
                    IsletmeAdiBul(ocrMetni);

                algilananTarih =
                    TarihBul(ocrMetni);

                algilananTutar =
                    ToplamTutarBul(ocrMetni);

                // Test icin Output ekraninda gorebiliriz
                Console.WriteLine("OCR METNI:");
                Console.WriteLine(ocrMetni);

                Console.WriteLine(
                    $"Isletme: {algilananIsletmeAdi}");

                Console.WriteLine(
                    $"Tarih: {algilananTarih}");

                Console.WriteLine(
                    $"Tutar: {algilananTutar}");
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    $"OCR islemi sirasinda hata olustu: {ex.Message}");
            }

            // Fis veritabanina kaydediliyor
            var fis = new Fis
            {
                KullaniciId = kullaniciId,

                DosyaYolu = Path.Combine(
                    "Uploads",
                    "Fisler",
                    yeniDosyaAdi),

                OrijinalDosyaAdi = dto.Dosya.FileName,

                HamOcrMetni = ocrMetni,

                AlgilananIsletmeAdi =
                    algilananIsletmeAdi,

                AlgilananTarih =
                    algilananTarih,

                AlgilananTutar =
                    algilananTutar,

                YuklenmeTarihi = DateTime.UtcNow
            };

            // Once fisi kaydediyoruz ki Id olussun.
            _context.Fisler.Add(fis);

            await _context.SaveChangesAsync();

            var fisDetaylari = FisDetaylariniBul(
            ocrMetni,
            fis.Id);

            if (fisDetaylari.Count > 0)
            {
                _context.FisDetaylari.AddRange(
                    fisDetaylari);
            }

            // OCR sonucu yeterliyse finansal islemi otomatik olustur.
            await FinansalIslemSenkronizeEt(
                fis,
                kullaniciId);

            await _context.SaveChangesAsync();

            if (fis.AlgilananTutar.HasValue)
            {
                await _butceBildirimServisi.ButceleriKontrolEt(
                    kullaniciId,
                    fis.AlgilananTarih ?? fis.YuklenmeTarihi,
                    null);
            }

            return Created("", new
            {
                fis.Id,
                fis.OrijinalDosyaAdi,
                fis.DosyaYolu,
                fis.AlgilananIsletmeAdi,
                fis.AlgilananTarih,
                fis.AlgilananTutar,
                fis.DogrulanmaTarihi,
                fis.HamOcrMetni
            });
        }

        // JWT icindeki kullanici ID'sini getirir
        private int KullaniciIdGetir()
        {
            var kullaniciId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            return int.Parse(kullaniciId!);
        }

        // OCR metnindeki ilk dolu satiri
        // isletme adi kabul ediyoruz.
        private string? IsletmeAdiBul(string ocrMetni)
        {
            var satirlar = ocrMetni
                .Split(
                    '\n',
                    StringSplitOptions.RemoveEmptyEntries)
                .Select(satir => satir.Trim())
                .Where(satir =>
                    !string.IsNullOrWhiteSpace(satir))
                .ToList();

            return satirlar.FirstOrDefault();
        }

        // 07.05.2019, 07/05/2019 veya
        // 07-05-2019 gibi tarihleri bulur.
        private DateTime? TarihBul(string ocrMetni)
        {
            var eslesme = Regex.Match(
                ocrMetni,
                @"\b(\d{2})[./-](\d{2})[./-](\d{4})\b");

            if (!eslesme.Success)
            {
                return null;
            }

            var tarihMetni = eslesme.Value
                .Replace(".", "/")
                .Replace("-", "/");

            if (DateTime.TryParseExact(
                tarihMetni,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var tarih))
            {
                return tarih;
            }

            return null;
        }

        // "GENEL TOPLAM 90,20" gibi
        // bir ifadeden tutari bulur.
        private decimal? ToplamTutarBul(string ocrMetni)
        {
            var eslesme = Regex.Match(
                ocrMetni,
                @"GENEL\s+TOPLAM\s+(\d+[.,]\d{2})",
                RegexOptions.IgnoreCase);

            if (!eslesme.Success)
            {
                return null;
            }

            var tutarMetni = eslesme
                .Groups[1]
                .Value
                .Replace(",", ".");

            if (decimal.TryParse(
                tutarMetni,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var tutar))
            {
                return tutar;
            }

            return null;
        }

        [HttpPut("{id:int}/dogrula")]
        public async Task<IActionResult> PutFisDogrula(
    int id,
    FisDogrulaPutDto dto)
        {
            var kullaniciId = KullaniciIdGetir();

            var fis = await _context.Fisler
                .FirstOrDefaultAsync(fis =>
                    fis.Id == id &&
                    fis.KullaniciId == kullaniciId);

            if (fis == null)
            {
                return NotFound("Fis bulunamadi.");
            }

            if (dto.Tutar.HasValue && dto.Tutar.Value <= 0)
            {
                return BadRequest("Tutar sifirdan buyuk olmalidir.");
            }

            fis.AlgilananIsletmeAdi = dto.IsletmeAdi?.Trim();
            fis.AlgilananTutar = dto.Tutar;
            fis.AlgilananTarih = dto.Tarih;
            fis.DogrulanmaTarihi = DateTime.UtcNow;

            
            await FinansalIslemSenkronizeEt(
            fis,
            kullaniciId);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                fis.Id,
                fis.OrijinalDosyaAdi,
                fis.AlgilananIsletmeAdi,
                fis.AlgilananTutar,
                fis.AlgilananTarih,
                fis.DogrulanmaTarihi,
                fis.HamOcrMetni
            });
        }

        [HttpPost("{id:int}/finansal-islem")]
        public async Task<IActionResult> PostFisFinansalIslem(
        int id,
        FisFinansalIslemPostDto dto)
        {
            var kullaniciId = KullaniciIdGetir();

            // Fis bu kullaniciya ait mi?
            var fis = await _context.Fisler
                .FirstOrDefaultAsync(fis =>
                    fis.Id == id &&
                    fis.KullaniciId == kullaniciId);

            if (fis == null)
            {
                return NotFound("Fis bulunamadi.");
            }

            // Bu fisten daha once finansal islem olusturulmus mu?
            var finansalIslemVarMi = await _context.FinansalIslemler
                .AnyAsync(islem =>
                    islem.FisId == fis.Id);

            if (finansalIslemVarMi)
            {
                return Conflict(
                    "Bu fis icin daha once finansal islem olusturulmus.");
            }

            // Tutar OCR tarafinda bulunmus / dogrulanmis olmali
            if (!fis.AlgilananTutar.HasValue ||
                fis.AlgilananTutar.Value <= 0)
            {
                return BadRequest(
                    "Fis tutari bulunamadi. Once fis bilgilerini dogrulayin.");
            }

            // Tarih de bulunmus / dogrulanmis olmali
            if (!fis.AlgilananTarih.HasValue)
            {
                return BadRequest(
                    "Fis tarihi bulunamadi. Once fis bilgilerini dogrulayin.");
            }

            // Secilen kategori kullanilabilir bir gider kategorisi mi?
            var kategori = await _context.Kategoriler
                .AsNoTracking()
                .FirstOrDefaultAsync(kategori =>
                    kategori.Id == dto.KategoriId &&
                    kategori.AktifMi &&
                    kategori.Tur == IslemTuru.Gider &&
                    (kategori.KullaniciId == null ||
                     kategori.KullaniciId == kullaniciId));

            if (kategori == null)
            {
                return BadRequest(
                    "Gecerli bir gider kategorisi secilmelidir.");
            }

            var finansalIslem = new FinansalIslem
            {
                KullaniciId = kullaniciId,

                Tur = IslemTuru.Gider,

                Tutar = fis.AlgilananTutar.Value,

                IslemTarihi = fis.AlgilananTarih.Value,

                KategoriId = kategori.Id,

                FisId = fis.Id,

                IsletmeId = null,

                Aciklama = string.IsNullOrWhiteSpace(dto.Aciklama)
                    ? $"Fis islemi - {fis.AlgilananIsletmeAdi}"
                    : dto.Aciklama.Trim(),

                OlusturulmaTarihi = DateTime.UtcNow
            };

            _context.FinansalIslemler.Add(finansalIslem);

            // Fis artik tamamlanmis kabul edilebilir
            fis.Durum = FisDurumu.Tamamlandi;
            fis.IslenmeTarihi = DateTime.UtcNow;

            await _context.SaveChangesAsync();


            return Created("", new
            {
                finansalIslem.Id,
                finansalIslem.Tur,
                finansalIslem.Tutar,
                finansalIslem.IslemTarihi,
                finansalIslem.Aciklama,
                finansalIslem.KategoriId,
                KategoriAdi = kategori.Ad,
                finansalIslem.FisId,
                IsletmeAdi = fis.AlgilananIsletmeAdi
            });
        }


        private List<FisDetayi> FisDetaylariniBul(
        string ocrMetni,
        int fisId)
        {
            var detaylar = new List<FisDetayi>();

            var satirlar = ocrMetni
                .Split(
                    '\n',
                    StringSplitOptions.RemoveEmptyEntries)
                .Select(satir => satir.Trim())
                .Where(satir => !string.IsNullOrWhiteSpace(satir))
                .ToList();

            var siraNo = 1;

            foreach (var satir in satirlar)
            {
                // Barkod / miktar satirlarini simdilik atla
                if (satir.Contains(
                        "ADET",
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // Toplam, para ustu gibi satirlari urun sanma
                if (satir.Contains(
                        "GENEL TOPLAM",
                        StringComparison.OrdinalIgnoreCase) ||
                    satir.Contains(
                        "ALINAN PARA",
                        StringComparison.OrdinalIgnoreCase) ||
                    satir.Contains(
                        "PARA ÜSTÜ",
                        StringComparison.OrdinalIgnoreCase) ||
                    satir.Contains(
                        "KDV",
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var eslesme = Regex.Match(
                    satir,
                    @"^(?<urun>.+?)\s+(?<tutar>\d+[.,]\d{2})$");

                if (!eslesme.Success)
                {
                    continue;
                }

                var urunAdi = eslesme
                    .Groups["urun"]
                    .Value
                    .Trim();

                var tutarMetni = eslesme
                    .Groups["tutar"]
                    .Value
                    .Replace(",", ".");

                if (!decimal.TryParse(
                        tutarMetni,
                        NumberStyles.Number,
                        CultureInfo.InvariantCulture,
                        out var toplamTutar))
                {
                    continue;
                }

                detaylar.Add(new FisDetayi
                {
                    FisId = fisId,
                    UrunAdi = urunAdi,
                    ToplamTutar = toplamTutar,
                    HamSatirMetni = satir,
                    SiraNo = siraNo
                });

                siraNo++;
            }

            return detaylar;
        }

        [HttpGet("{id:int}/detaylar")]
        public async Task<ActionResult<List<FisDetayiGetDto>>> GetFisDetaylari(
    int id)
        {
            var kullaniciId = KullaniciIdGetir();

            var fisVarMi = await _context.Fisler
                .AsNoTracking()
                .AnyAsync(fis =>
                    fis.Id == id &&
                    fis.KullaniciId == kullaniciId);

            if (!fisVarMi)
            {
                return NotFound("Fis bulunamadi.");
            }

            var detaylar = await _context.FisDetaylari
                .AsNoTracking()
                .Where(detay =>
                    detay.FisId == id)
                .OrderBy(detay =>
                    detay.SiraNo)
                .Select(detay => new FisDetayiGetDto
                {
                    Id = detay.Id,
                    UrunAdi = detay.UrunAdi,
                    Miktar = detay.Miktar,
                    BirimFiyat = detay.BirimFiyat,
                    ToplamTutar = detay.ToplamTutar,
                    HamSatirMetni = detay.HamSatirMetni,
                    SiraNo = detay.SiraNo,
                    KategoriId = detay.KategoriId
                })
                .ToListAsync();

            return Ok(detaylar);
        }

        [HttpPut("{fisId:int}/detaylar/{detayId:int}")]
        public async Task<IActionResult> PutFisDetayi(
    int fisId,
    int detayId,
    FisDetayiPutDto dto)
        {
            var kullaniciId = KullaniciIdGetir();

            var detay = await _context.FisDetaylari
                .Include(detay => detay.Fis)
                .FirstOrDefaultAsync(detay =>
                    detay.Id == detayId &&
                    detay.FisId == fisId &&
                    detay.Fis.KullaniciId == kullaniciId);

            if (detay == null)
            {
                return NotFound("Fis detayi bulunamadi.");
            }

            var urunAdi = dto.UrunAdi.Trim();

            if (string.IsNullOrWhiteSpace(urunAdi))
            {
                return BadRequest("Urun adi bos olamaz.");
            }

            if (dto.ToplamTutar <= 0)
            {
                return BadRequest(
                    "Toplam tutar sifirdan buyuk olmalidir.");
            }

            if (dto.Miktar.HasValue &&
                dto.Miktar.Value <= 0)
            {
                return BadRequest(
                    "Miktar sifirdan buyuk olmalidir.");
            }

            if (dto.BirimFiyat.HasValue &&
                dto.BirimFiyat.Value <= 0)
            {
                return BadRequest(
                    "Birim fiyat sifirdan buyuk olmalidir.");
            }

            if (dto.KategoriId.HasValue)
            {
                var kategoriVarMi = await _context.Kategoriler
                    .AnyAsync(kategori =>
                        kategori.Id == dto.KategoriId.Value &&
                        kategori.AktifMi &&
                        kategori.Tur == IslemTuru.Gider &&
                        (kategori.KullaniciId == null ||
                         kategori.KullaniciId == kullaniciId));

                if (!kategoriVarMi)
                {
                    return BadRequest(
                        "Gecerli bir gider kategorisi secilmelidir.");
                }
            }

            detay.UrunAdi = urunAdi;
            detay.Miktar = dto.Miktar;
            detay.BirimFiyat = dto.BirimFiyat;
            detay.ToplamTutar = dto.ToplamTutar;
            detay.KategoriId = dto.KategoriId;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}