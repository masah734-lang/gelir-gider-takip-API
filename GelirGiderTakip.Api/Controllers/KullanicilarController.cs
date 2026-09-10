using GelirGiderTakip.Api.Data;
using GelirGiderTakip.Api.DTOs.Kullanicilar;
using GelirGiderTakip.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GelirGiderTakip.Api.Controllers
{
    [ApiController]
    [Route("api/kullanicilar")]
    public class KullanicilarController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly IPasswordHasher<Kullanici> _passwordHasher;

        public KullanicilarController(
            AppDBContext context,
            IPasswordHasher<Kullanici> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // POST: api/kullanicilar
        [HttpPost("kayit")]
        public async Task<IActionResult> PostKayit(KullaniciPostDto dto)
        {
            var eposta = dto.Eposta.Trim().ToLower();

            var epostaKullaniliyorMu = await _context.Kullanicilar
                .AnyAsync(kullanici => kullanici.Eposta == eposta);

            if (epostaKullaniliyorMu)
            {
                return Conflict("Bu e-posta adresi zaten kullaniliyor.");
            }

            var kullanici = new Kullanici
            {
                Ad = dto.Ad.Trim(),
                Soyad = dto.Soyad.Trim(),
                Eposta = eposta,
                AktifMi = true,
                OlusturulmaTarihi = DateTime.UtcNow
            };

            kullanici.ParolaHash = _passwordHasher.HashPassword(
                kullanici,
                dto.Parola);

            _context.Kullanicilar.Add(kullanici);

            await _context.SaveChangesAsync();

            return Created("", new
            {
                kullanici.Id,
                kullanici.Ad,
                kullanici.Soyad,
                kullanici.Eposta
            });
        }

        // POST: api/kullanicilar/giris : giriş ekranındaki şifre ve email doğrulaması için
        [HttpPost("giris")]
        public async Task<IActionResult> PostGiris(KullaniciGirisPostDto dto)
        {
            var eposta = dto.Eposta.Trim().ToLower();

            var kullanici = await _context.Kullanicilar
                .AsNoTracking()
                .FirstOrDefaultAsync(kullanici =>
                    kullanici.Eposta == eposta &&
                    kullanici.AktifMi);

            if (kullanici == null)
            {
                return Unauthorized("E-posta veya parola hatali.");
            }

            var parolaSonucu = _passwordHasher.VerifyHashedPassword(
                kullanici,
                kullanici.ParolaHash,
                dto.Parola);

            if (parolaSonucu == PasswordVerificationResult.Failed)
            {
                return Unauthorized("E-posta veya parola hatali.");
            }

            return Ok(new
            {
                kullanici.Id,
                kullanici.Ad,
                kullanici.Soyad,
                kullanici.Eposta
            });
        }
    }
}