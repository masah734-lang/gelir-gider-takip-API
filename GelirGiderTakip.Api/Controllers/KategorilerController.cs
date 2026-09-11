using GelirGiderTakip.Api.Data;
using GelirGiderTakip.Api.DTOs.Kategoriler;
using GelirGiderTakip.Api.Enums;
using GelirGiderTakip.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GelirGiderTakip.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/kategoriler")]
    public class KategorilerController : ControllerBase
    {
        private readonly AppDBContext _context;

        public KategorilerController(AppDBContext context)
        {
            _context = context;
        }

        //GET örnek endpointleri
        [HttpGet]
        public async Task<ActionResult<List<KategoriGetDto>>> GetKategoriler()
        {
            var kullaniciId = KullaniciIdGetir();

            var kategoriler = await _context.Kategoriler
                .AsNoTracking()
                .Where(kategori =>
                    kategori.AktifMi &&
                    (kategori.KullaniciId == null ||
                     kategori.KullaniciId == kullaniciId))
                .OrderBy(kategori => kategori.Tur)
                .ThenBy(kategori => kategori.Ad)
                .Select(kategori => new KategoriGetDto
                {
                    Id = kategori.Id,
                    Ad = kategori.Ad,
                    Tur = kategori.Tur,
                    VarsayilanMi = kategori.KullaniciId == null
                })
                .ToListAsync();

            return Ok(kategoriler);
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<KategoriGetDto>> GetKategori(int id)
        {
            var kullaniciId = KullaniciIdGetir();

            var kategori = await _context.Kategoriler
                .AsNoTracking()
                .Where(kategori =>
                    kategori.Id == id &&
                    kategori.AktifMi &&
                    (kategori.KullaniciId == null ||
                     kategori.KullaniciId == kullaniciId))
                .Select(kategori => new KategoriGetDto
                {
                    Id = kategori.Id,
                    Ad = kategori.Ad,
                    Tur = kategori.Tur,
                    VarsayilanMi = kategori.KullaniciId == null
                })
                .FirstOrDefaultAsync();

            if (kategori == null)
            {
                return NotFound("Kategori bulunamadi.");
            }

            return Ok(kategori);
        }

        [HttpPost]
        public async Task<ActionResult<KategoriGetDto>> PostKategori(
    KategoriPostDto dto)
        {
            var kullaniciId = KullaniciIdGetir();

            var kullaniciVarMi = await _context.Kullanicilar
                .AnyAsync(kullanici =>
                    kullanici.Id == kullaniciId &&
                    kullanici.AktifMi);

            if (!kullaniciVarMi)
            {
                return NotFound("Kullanici bulunamadi.");
            }

            var kategoriAdi = dto.Ad.Trim();

            if (string.IsNullOrWhiteSpace(kategoriAdi))
            {
                return BadRequest("Kategori adi bos olamaz.");
            }

            if (!Enum.IsDefined(typeof(IslemTuru), dto.Tur))
            {
                return BadRequest("Gecersiz islem turu.");
            }

            // Once varsayilan kategorileri kontrol et
            var varsayilanKategoriVarMi = await _context.Kategoriler
                .AnyAsync(kategori =>
                    kategori.KullaniciId == null &&
                    kategori.Ad == kategoriAdi &&
                    kategori.Tur == dto.Tur);

            if (varsayilanKategoriVarMi)
            {
                return Conflict(
                    "Bu isim ve turde varsayilan bir kategori zaten mevcut.");
            }

            var mevcutKategori = await _context.Kategoriler
                .FirstOrDefaultAsync(kategori =>
                    kategori.KullaniciId == kullaniciId &&
                    kategori.Ad == kategoriAdi &&
                    kategori.Tur == dto.Tur);

            if (mevcutKategori != null)
            {
                if (mevcutKategori.AktifMi)
                {
                    return Conflict("Bu kategori zaten mevcut.");
                }

                mevcutKategori.AktifMi = true;
                mevcutKategori.GuncellenmeTarihi = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var tekrarAktifKategori = new KategoriGetDto
                {
                    Id = mevcutKategori.Id,
                    Ad = mevcutKategori.Ad,
                    Tur = mevcutKategori.Tur,
                    VarsayilanMi = false
                };

                return Ok(tekrarAktifKategori);
            }

            var yeniKategori = new Kategori
            {
                Ad = kategoriAdi,
                Tur = dto.Tur,
                KullaniciId = kullaniciId,
                AktifMi = true,
                OlusturulmaTarihi = DateTime.UtcNow
            };

            _context.Kategoriler.Add(yeniKategori);

            await _context.SaveChangesAsync();

            var kategoriDto = new KategoriGetDto
            {
                Id = yeniKategori.Id,
                Ad = yeniKategori.Ad,
                Tur = yeniKategori.Tur,
                VarsayilanMi = false
            };

            return CreatedAtAction(
                nameof(GetKategori),
                new
                {
                    id = yeniKategori.Id
                },
                kategoriDto);
        }
        //PUT örnek endpointleri
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutKategori(
         int id,
         KategoriPutDto dto)
        {
            var kullaniciId = KullaniciIdGetir();
            var kategori = await _context.Kategoriler
                .FirstOrDefaultAsync(kategori =>
                    kategori.Id == id &&
                    kategori.KullaniciId == kullaniciId &&
                    kategori.AktifMi);

            if (kategori == null)
            {
                return NotFound("Kategori bulunamadi.");
            }

            var kategoriAdi = dto.Ad.Trim();

            if (string.IsNullOrWhiteSpace(kategoriAdi))
            {
                return BadRequest("Kategori adi bos olamaz.");
            }
            
            if (!Enum.IsDefined(typeof(IslemTuru), dto.Tur))
            {
                return BadRequest("Gecersiz islem turu.");
            }

            var ayniKategoriVarMi = await _context.Kategoriler
                .AnyAsync(digerKategori =>
                    digerKategori.Id != id &&
                    digerKategori.KullaniciId == kullaniciId &&
                    digerKategori.Ad == kategoriAdi &&
                    digerKategori.Tur == dto.Tur);

            if (ayniKategoriVarMi)
            {
                return Conflict("Bu isim ve turde bir kategori zaten mevcut.");
            }

            var varsayilanKategoriVarMi = await _context.Kategoriler
                .AnyAsync(digerKategori =>
                    digerKategori.KullaniciId == null &&
                    digerKategori.Ad == kategoriAdi &&
                    digerKategori.Tur == dto.Tur);

            if (varsayilanKategoriVarMi)
            {
                return Conflict("Bu isim ve turde varsayilan bir kategori zaten mevcut.");
            }


            kategori.Ad = kategoriAdi;
            kategori.Tur = dto.Tur;
            kategori.GuncellenmeTarihi = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        //DELETE örnek endpointleri
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteKategori(
            int id
            )
        {
            var kullaniciId = KullaniciIdGetir();
            var kategori = await _context.Kategoriler
                .FirstOrDefaultAsync(kategori =>
                    kategori.Id == id &&
                    kategori.KullaniciId == kullaniciId &&
                    kategori.AktifMi);

            if (kategori == null)
            {
                return NotFound("Kategori bulunamadi.");
            }

            kategori.AktifMi = false;
            kategori.GuncellenmeTarihi = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        private int KullaniciIdGetir()
        {
            var kullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.Parse(kullaniciId!);
        }
    }
}
