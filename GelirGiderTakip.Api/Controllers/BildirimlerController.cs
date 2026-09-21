using GelirGiderTakip.Api.Data;
using GelirGiderTakip.Api.DTOs.Bildirimler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GelirGiderTakip.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/bildirimler")]
    public class BildirimlerController : ControllerBase
    {
        private readonly AppDBContext _context;

        public BildirimlerController(AppDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<BildirimGetDto>>> GetBildirimler()
        {
            var kullaniciId = KullaniciIdGetir();

            var bildirimler = await _context.Bildirimler
                .AsNoTracking()
                .Where(bildirim =>
                    bildirim.KullaniciId == kullaniciId)
                .OrderByDescending(bildirim =>
                    bildirim.OlusturulmaTarihi)
                .Select(bildirim => new BildirimGetDto
                {
                    Id = bildirim.Id,
                    Tur = bildirim.Tur,
                    Baslik = bildirim.Baslik,
                    Mesaj = bildirim.Mesaj,
                    OlusturulmaTarihi =
                        bildirim.OlusturulmaTarihi,
                    OkunmaTarihi =
                        bildirim.OkunmaTarihi,
                    OkunduMu =
                        bildirim.OkunmaTarihi != null
                })
                .ToListAsync();

            return Ok(bildirimler);
        }

        [HttpPut("{id:int}/okundu")]
        public async Task<IActionResult> PutOkundu(int id)
        {
            var kullaniciId = KullaniciIdGetir();

            var bildirim = await _context.Bildirimler
                .FirstOrDefaultAsync(bildirim =>
                    bildirim.Id == id &&
                    bildirim.KullaniciId == kullaniciId);

            if (bildirim == null)
            {
                return NotFound("Bildirim bulunamadi.");
            }

            bildirim.OkunmaTarihi = DateTime.UtcNow;

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