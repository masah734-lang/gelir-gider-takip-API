using GelirGiderTakip.Api.Data;
using GelirGiderTakip.Api.DTOs.FinansalIslemler;
using GelirGiderTakip.Api.Enums;
using GelirGiderTakip.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GelirGiderTakip.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/finansal-islemler")]
    public class FinansalIslemlerController : ControllerBase
    {
        private readonly AppDBContext _context;

        public FinansalIslemlerController(AppDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<FinansalIslemGetDto>>> GetFinansalIslemler()
        {
            var kullaniciId = KullaniciIdGetir();

            var finansalIslemler = await _context.FinansalIslemler
                .AsNoTracking()
                .Where(islem => islem.KullaniciId == kullaniciId)
                .OrderByDescending(islem => islem.IslemTarihi)
                .Select(islem => new FinansalIslemGetDto
                {
                    Id = islem.Id,
                    Tur = islem.Tur,
                    Tutar = islem.Tutar,
                    IslemTarihi = islem.IslemTarihi,
                    Aciklama = islem.Aciklama,

                    KategoriId = islem.KategoriId,
                    KategoriAdi = islem.Kategori != null
                        ? islem.Kategori.Ad
                        : null,

                    IsletmeId = islem.IsletmeId,
                    IsletmeAdi = islem.Isletme != null
                        ? islem.Isletme.Ad
                        : null,

                    FisId = islem.FisId
                })
                .ToListAsync();

            return Ok(finansalIslemler);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<FinansalIslemGetDto>> GetFinansalIslem(int id)
        {
            var kullaniciId = KullaniciIdGetir();

            var finansalIslem = await _context.FinansalIslemler
                .AsNoTracking()
                .Where(islem =>
                    islem.Id == id &&
                    islem.KullaniciId == kullaniciId)
                .Select(islem => new FinansalIslemGetDto
                {
                    Id = islem.Id,
                    Tur = islem.Tur,
                    Tutar = islem.Tutar,
                    IslemTarihi = islem.IslemTarihi,
                    Aciklama = islem.Aciklama,

                    KategoriId = islem.KategoriId,
                    KategoriAdi = islem.Kategori != null
                        ? islem.Kategori.Ad
                        : null,

                    IsletmeId = islem.IsletmeId,
                    IsletmeAdi = islem.Isletme != null
                        ? islem.Isletme.Ad
                        : null,

                    FisId = islem.FisId
                })
                .FirstOrDefaultAsync();

            if (finansalIslem == null)
            {
                return NotFound("Finansal islem bulunamadi.");
            }

            return Ok(finansalIslem);
        }

        private int KullaniciIdGetir()
        {
            var kullaniciId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            return int.Parse(kullaniciId!);
        }

        [HttpPost]
        public async Task<ActionResult<FinansalIslemGetDto>> PostFinansalIslem(
        FinansalIslemPostDto dto)
        {
            var kullaniciId = KullaniciIdGetir();

            if (!Enum.IsDefined(typeof(IslemTuru), dto.Tur))
            {
                return BadRequest("Gecersiz islem turu.");
            }

            if (dto.Tutar <= 0)
            {
                return BadRequest("Tutar sifirdan buyuk olmalidir.");
            }

            // Kategori secilmisse kontrol et
            if (dto.KategoriId.HasValue)
            {
                var kategoriVarMi = await _context.Kategoriler
                    .AnyAsync(kategori =>
                        kategori.Id == dto.KategoriId.Value &&
                        kategori.AktifMi &&
                        kategori.Tur == dto.Tur &&
                        (kategori.KullaniciId == null ||
                         kategori.KullaniciId == kullaniciId));

                if (!kategoriVarMi)
                {
                    return BadRequest(
                        "Kategori bulunamadi veya islem turu ile uyumlu degil.");
                }
            }

            var yeniIslem = new FinansalIslem
            {
                KullaniciId = kullaniciId,
                Tur = dto.Tur,
                Tutar = dto.Tutar,
                IslemTarihi = dto.IslemTarihi,
                Aciklama = dto.Aciklama?.Trim(),
                KategoriId = dto.KategoriId,

               

                
                OlusturulmaTarihi = DateTime.UtcNow
            };

            _context.FinansalIslemler.Add(yeniIslem);

            await _context.SaveChangesAsync();

            var sonuc = await _context.FinansalIslemler
                .AsNoTracking()
                .Where(islem => islem.Id == yeniIslem.Id)
                .Select(islem => new FinansalIslemGetDto
                {
                    Id = islem.Id,
                    Tur = islem.Tur,
                    Tutar = islem.Tutar,
                    IslemTarihi = islem.IslemTarihi,
                    Aciklama = islem.Aciklama,

                    KategoriId = islem.KategoriId,
                    KategoriAdi = islem.Kategori != null
                        ? islem.Kategori.Ad
                        : null,

                    IsletmeId = islem.IsletmeId,
                    IsletmeAdi = islem.Isletme != null
                        ? islem.Isletme.Ad
                        : null,

                    FisId = islem.FisId
                })
                .FirstAsync();

            return Created("", sonuc);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutFinansalIslem(
        int id,
        FinansalIslemPutDto dto)
        {
            var kullaniciId = KullaniciIdGetir();

            // Sadece giris yapan kullanicinin kendi islemini bul
            var finansalIslem = await _context.FinansalIslemler
                .FirstOrDefaultAsync(islem =>
                    islem.Id == id &&
                    islem.KullaniciId == kullaniciId);

            if (finansalIslem == null)
            {
                return NotFound("Finansal islem bulunamadi.");
            }

            // Gelir / Gider kontrolu
            if (!Enum.IsDefined(typeof(IslemTuru), dto.Tur))
            {
                return BadRequest("Gecersiz islem turu.");
            }

            // Tutar kontrolu
            if (dto.Tutar <= 0)
            {
                return BadRequest("Tutar sifirdan buyuk olmalidir.");
            }

            // Kategori secilmisse kontrol et
            if (dto.KategoriId.HasValue)
            {
                var kategoriVarMi = await _context.Kategoriler
                    .AnyAsync(kategori =>
                        kategori.Id == dto.KategoriId.Value &&
                        kategori.AktifMi &&
                        kategori.Tur == dto.Tur &&
                        (kategori.KullaniciId == null ||
                         kategori.KullaniciId == kullaniciId));

                if (!kategoriVarMi)
                {
                    return BadRequest(
                        "Kategori bulunamadi veya islem turu ile uyumlu degil.");
                }
            }

            
            

            // Degerleri guncelle
            finansalIslem.Tur = dto.Tur;
            finansalIslem.Tutar = dto.Tutar;
            finansalIslem.IslemTarihi = dto.IslemTarihi;
            finansalIslem.Aciklama = dto.Aciklama?.Trim();
            finansalIslem.KategoriId = dto.KategoriId;
            
            finansalIslem.GuncellenmeTarihi = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> PatchFinansalIslem(
        int id,
        FinansalIslemPatchDto dto)
        {
            var kullaniciId = KullaniciIdGetir();

            var finansalIslem = await _context.FinansalIslemler
                .FirstOrDefaultAsync(islem =>
                    islem.Id == id &&
                    islem.KullaniciId == kullaniciId);

            if (finansalIslem == null)
            {
                return NotFound("Finansal islem bulunamadi.");
            }

            if (dto.Tur.HasValue)
            {
                if (!Enum.IsDefined(typeof(IslemTuru), dto.Tur.Value))
                {
                    return BadRequest("Gecersiz islem turu.");
                }

                finansalIslem.Tur = dto.Tur.Value;
            }

            if (dto.Tutar.HasValue)
            {
                if (dto.Tutar.Value <= 0)
                {
                    return BadRequest("Tutar sifirdan buyuk olmalidir.");
                }

                finansalIslem.Tutar = dto.Tutar.Value;
            }

            if (dto.IslemTarihi.HasValue)
            {
                finansalIslem.IslemTarihi = dto.IslemTarihi.Value;
            }

            if (dto.Aciklama != null)
            {
                finansalIslem.Aciklama = dto.Aciklama.Trim();
            }

            if (dto.KategoriId.HasValue)
            {
                var kategoriVarMi = await _context.Kategoriler
                    .AnyAsync(kategori =>
                        kategori.Id == dto.KategoriId.Value &&
                        kategori.AktifMi &&
                        kategori.Tur == finansalIslem.Tur &&
                        (kategori.KullaniciId == null ||
                         kategori.KullaniciId == kullaniciId));

                if (!kategoriVarMi)
                {
                    return BadRequest(
                        "Kategori bulunamadi veya islem turu ile uyumlu degil.");
                }

                finansalIslem.KategoriId = dto.KategoriId.Value;
            }

            finansalIslem.GuncellenmeTarihi = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteFinansalIslem(int id)
        {
            var kullaniciId = KullaniciIdGetir();

            var finansalIslem = await _context.FinansalIslemler
                .FirstOrDefaultAsync(islem =>
                    islem.Id == id &&
                    islem.KullaniciId == kullaniciId);

            if (finansalIslem == null)
            {
                return NotFound("Finansal islem bulunamadi.");
            }

            // Eger islem bir fisten olusturulduysa,
            // fis silinmez ve tekrar islenebilir hale getirilir.
            if (finansalIslem.FisId.HasValue)
            {
                var fis = await _context.Fisler
                    .FirstOrDefaultAsync(fis =>
                        fis.Id == finansalIslem.FisId.Value &&
                        fis.KullaniciId == kullaniciId);

                if (fis != null)
                {
                    fis.Durum = FisDurumu.Bekliyor;
                    fis.IslenmeTarihi = null;
                }
            }

            _context.FinansalIslemler.Remove(finansalIslem);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}