using GelirGiderTakip.Api.Data;
using GelirGiderTakip.Api.DTOs.Kullanicilar;
using GelirGiderTakip.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace GelirGiderTakip.Api.Controllers
{
    [ApiController]
    [Route("api/kullanicilar")]
    public class KullanicilarController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly IPasswordHasher<Kullanici> _passwordHasher;
        private readonly IConfiguration _configuration;

        public KullanicilarController(
            AppDBContext context,
            IPasswordHasher<Kullanici> passwordHasher,
            IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
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

        // POST: api/kullanicilar/giris : giriş ekranındaki şifre ve emaili kontrol eder
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

            var jwtKey = _configuration["Jwt:Key"];

            if (string.IsNullOrEmpty(jwtKey))
            {
                return StatusCode(500, "JWT anahtari bulunamadi.");
            }

            var claims = new List<Claim>
    {
        new Claim(
            ClaimTypes.NameIdentifier,
            kullanici.Id.ToString()),

        new Claim(
            ClaimTypes.Email,
            kullanici.Eposta),

        new Claim(
            ClaimTypes.Name,
            $"{kullanici.Ad} {kullanici.Soyad}")
    };

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey));

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler()
                .WriteToken(token);

            return Ok(new
            {
                Token = tokenString,
                Kullanici = new
                {
                    kullanici.Id,
                    kullanici.Ad,
                    kullanici.Soyad,
                    kullanici.Eposta
                }
            });
        }
    }
}