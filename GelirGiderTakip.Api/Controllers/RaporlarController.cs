using GelirGiderTakip.Api.Data;
using GelirGiderTakip.Api.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GelirGiderTakip.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/raporlar")]
    public class RaporlarController : ControllerBase
    {
        private readonly AppDBContext _context;

        public RaporlarController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/raporlar/ozet
        [HttpGet("ozet")]
        public async Task<IActionResult> GetOzet()
        {
            var kullaniciId = KullaniciIdGetir();

            var toplamGelir = await _context.FinansalIslemler
                .AsNoTracking()
                .Where(islem =>
                    islem.KullaniciId == kullaniciId &&
                    islem.Tur == IslemTuru.Gelir)
                .SumAsync(islem => (decimal?)islem.Tutar) ?? 0;

            var toplamGider = await _context.FinansalIslemler
                .AsNoTracking()
                .Where(islem =>
                    islem.KullaniciId == kullaniciId &&
                    islem.Tur == IslemTuru.Gider)
                .SumAsync(islem => (decimal?)islem.Tutar) ?? 0;

            var bakiye = toplamGelir - toplamGider;

            return Ok(new
            {
                ToplamGelir = toplamGelir,
                ToplamGider = toplamGider,
                Bakiye = bakiye
            });
        }

        // GET: api/raporlar/aylik?yil=2026&ay=9
        [HttpGet("aylik")]
        public async Task<IActionResult> GetAylik(
            int yil,
            int ay)
        {
            var kullaniciId = KullaniciIdGetir();

            if (ay < 1 || ay > 12)
            {
                return BadRequest("Ay 1 ile 12 arasinda olmalidir.");
            }

            var baslangicTarihi = new DateTime(yil, ay, 1);
            var bitisTarihi = baslangicTarihi.AddMonths(1);

            var aylikIslemler = _context.FinansalIslemler
                .AsNoTracking()
                .Where(islem =>
                    islem.KullaniciId == kullaniciId &&
                    islem.IslemTarihi >= baslangicTarihi &&
                    islem.IslemTarihi < bitisTarihi);

            var toplamGelir = await aylikIslemler
                .Where(islem => islem.Tur == IslemTuru.Gelir)
                .SumAsync(islem => (decimal?)islem.Tutar) ?? 0;

            var toplamGider = await aylikIslemler
                .Where(islem => islem.Tur == IslemTuru.Gider)
                .SumAsync(islem => (decimal?)islem.Tutar) ?? 0;

            var bakiye = toplamGelir - toplamGider;

            return Ok(new
            {
                Yil = yil,
                Ay = ay,
                ToplamGelir = toplamGelir,
                ToplamGider = toplamGider,
                Bakiye = bakiye
            });
        }

        // GET: api/raporlar/kategori-giderleri
        [HttpGet("kategori-giderleri")]
        public async Task<IActionResult> GetKategoriGiderleri()
        {
            var kullaniciId = KullaniciIdGetir();

            var kategoriGiderleri = await _context.FinansalIslemler
                .AsNoTracking()
                .Where(islem =>
                    islem.KullaniciId == kullaniciId &&
                    islem.Tur == IslemTuru.Gider)
                .GroupBy(islem => new
                {
                    islem.KategoriId,
                    KategoriAdi = islem.Kategori != null
                        ? islem.Kategori.Ad
                        : "Kategorisiz"
                })
                .Select(grup => new
                {
                    KategoriId = grup.Key.KategoriId,
                    KategoriAdi = grup.Key.KategoriAdi,
                    ToplamGider = grup.Sum(islem => islem.Tutar)
                })
                .OrderByDescending(x => x.ToplamGider)
                .ToListAsync();

            return Ok(kategoriGiderleri);
        }

        private int KullaniciIdGetir()
        {
            var kullaniciId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            return int.Parse(kullaniciId!);
        }
    }
}