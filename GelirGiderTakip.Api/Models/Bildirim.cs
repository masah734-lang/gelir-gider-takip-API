using GelirGiderTakip.Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace GelirGiderTakip.Api.Models
{
    public class Bildirim
    {
        public int Id { get; set; }

        public BildirimTuru Tur { get; set; }

        [Required]
        [MaxLength(150)]
        public string Baslik { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Mesaj { get; set; } = string.Empty;

        public DateTime? OkunmaTarihi { get; set; }

        public DateTime OlusturulmaTarihi { get; set; }
            = DateTime.UtcNow;

        public int KullaniciId { get; set; }
        public Kullanici Kullanici { get; set; } = null!;

        public int? ButceId { get; set; }
        public Butce? Butce { get; set; }
    }
}