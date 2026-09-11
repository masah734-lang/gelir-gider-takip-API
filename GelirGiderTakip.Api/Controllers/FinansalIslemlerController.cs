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

                // Isletme API daha sonra
                IsletmeId = dto.IsletmeId,

                
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
    }
}