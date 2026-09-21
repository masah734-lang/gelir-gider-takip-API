using GelirGiderTakip.Api.Data;
using GelirGiderTakip.Api.DTOs.Butceler;
using GelirGiderTakip.Api.Enums;
using GelirGiderTakip.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using GelirGiderTakip.Api.Services;

namespace GelirGiderTakip.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/butceler")]
    public class ButcelerController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly ButceBildirimServisi _butceBildirimServisi;

        public ButcelerController(AppDBContext context, ButceBildirimServisi butceBildirimServisi)
        {
            _context = context;
            _butceBildirimServisi = butceBildirimServisi;
        }

        // GET: api/butceler?yil=2026&ay=9
        [HttpGet]
        public async Task<ActionResult<List<ButceGetDto>>> GetButceler(
            short yil,
            byte ay)
        {
            var kullaniciId = KullaniciIdGetir();

            if (ay < 1 || ay > 12)
            {
                return BadRequest("Ay 1 ile 12 arasinda olmalidir.");
            }

            var baslangic = new DateTime(yil, ay, 1);
            var bitis = baslangic.AddMonths(1);

            var butceler = await _context.Butceler
                .AsNoTracking()
                .Where(butce =>
                    butce.KullaniciId == kullaniciId &&
                    butce.Yil == yil &&
                    butce.Ay == ay)
                .ToListAsync();

            var sonuc = new List<ButceGetDto>();

            foreach (var butce in butceler)
            {
                var giderSorgusu = _context.FinansalIslemler
                    .AsNoTracking()
                    .Where(islem =>
                        islem.KullaniciId == kullaniciId &&
                        islem.Tur == IslemTuru.Gider &&
                        islem.IslemTarihi >= baslangic &&
                        islem.IslemTarihi < bitis);

                if (butce.KategoriId.HasValue)
                {
                    giderSorgusu = giderSorgusu.Where(islem =>
                        islem.KategoriId == butce.KategoriId);
                }

                var harcananTutar = await giderSorgusu
                    .SumAsync(islem => (decimal?)islem.Tutar) ?? 0;

                string? kategoriAdi = null;

                if (butce.KategoriId.HasValue)
                {
                    kategoriAdi = await _context.Kategoriler
                        .AsNoTracking()
                        .Where(kategori =>
                            kategori.Id == butce.KategoriId.Value)
                        .Select(kategori => kategori.Ad)
                        .FirstOrDefaultAsync();
                }

                sonuc.Add(new ButceGetDto
                {
                    Id = butce.Id,
                    Yil = butce.Yil,
                    Ay = butce.Ay,
                    LimitTutari = butce.LimitTutari,
                    KategoriId = butce.KategoriId,
                    KategoriAdi = kategoriAdi,
                    HarcananTutar = harcananTutar,
                    KalanTutar = butce.LimitTutari - harcananTutar
                });
            }

            return Ok(sonuc);
        }

        // POST: api/butceler
        [HttpPost]
        public async Task<IActionResult> PostButce(ButcePostDto dto)
        {
            var kullaniciId = KullaniciIdGetir();

            if (dto.Yil < 2000 || dto.Yil > 2100)
            {
                return BadRequest("Gecersiz yil.");
            }

            if (dto.Ay < 1 || dto.Ay > 12)
            {
                return BadRequest("Ay 1 ile 12 arasinda olmalidir.");
            }

            if (dto.LimitTutari <= 0)
            {
                return BadRequest(
                    "Butce limiti sifirdan buyuk olmalidir.");
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

            var ayniButceVarMi = await _context.Butceler
                .AnyAsync(butce =>
                    butce.KullaniciId == kullaniciId &&
                    butce.Yil == dto.Yil &&
                    butce.Ay == dto.Ay &&
                    butce.KategoriId == dto.KategoriId);

            if (ayniButceVarMi)
            {
                return Conflict(
                    "Bu ay ve kategori icin zaten butce mevcut.");
            }

            var butce = new Butce
            {
                KullaniciId = kullaniciId,
                Yil = dto.Yil,
                Ay = dto.Ay,
                LimitTutari = dto.LimitTutari,
                KategoriId = dto.KategoriId,
                OlusturulmaTarihi = DateTime.UtcNow
            };

            _context.Butceler.Add(butce);

            await _context.SaveChangesAsync();

            var butceTarihi = new DateTime(
            dto.Yil,
             dto.Ay,
             1);

            await _butceBildirimServisi.ButceleriKontrolEt(
                kullaniciId,
                butceTarihi,
                dto.KategoriId);

            return Created("", new
            {
                butce.Id,
                butce.Yil,
                butce.Ay,
                butce.LimitTutari,
                butce.KategoriId
            });
        }

        // PUT: api/butceler/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutButce(
            int id,
            ButcePutDto dto)
        {
            var kullaniciId = KullaniciIdGetir();

            var butce = await _context.Butceler
                .FirstOrDefaultAsync(butce =>
                    butce.Id == id &&
                    butce.KullaniciId == kullaniciId);

            if (butce == null)
            {
                return NotFound("Butce bulunamadi.");
            }

            if (dto.LimitTutari <= 0)
            {
                return BadRequest(
                    "Butce limiti sifirdan buyuk olmalidir.");
            }

            butce.LimitTutari = dto.LimitTutari;
            butce.GuncellenmeTarihi = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/butceler/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteButce(int id)
        {
            var kullaniciId = KullaniciIdGetir();

            var butce = await _context.Butceler
                .FirstOrDefaultAsync(butce =>
                    butce.Id == id &&
                    butce.KullaniciId == kullaniciId);

            if (butce == null)
            {
                return NotFound("Butce bulunamadi.");
            }

            _context.Butceler.Remove(butce);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private int KullaniciIdGetir()
        {
            var kullaniciId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            return int.Parse(kullaniciId!);
        }
    }
}