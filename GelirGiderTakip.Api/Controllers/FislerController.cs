using GelirGiderTakip.Api.Data;
using GelirGiderTakip.Api.DTOs.Fisler;
using GelirGiderTakip.Api.Models;
using GelirGiderTakip.Api.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;
using System.Text.RegularExpressions;
using TesseractOCR;
using TesseractOCR.Enums;
using Microsoft.EntityFrameworkCore;

namespace GelirGiderTakip.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/fisler")]
    public class FislerController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment _environment;

        public FislerController(
            AppDBContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
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

            _context.Fisler.Add(fis);

            await _context.SaveChangesAsync();

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
    }
}