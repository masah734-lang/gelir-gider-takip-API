using System.ComponentModel.DataAnnotations;

namespace GelirGiderTakip.Api.DTOs.Kullanicilar
{
    public class KullaniciGirisPostDto
    {
        [Required]
        public string Eposta { get; set; } = string.Empty;

        [Required]
        public string Parola { get; set; } = string.Empty;
    }
}