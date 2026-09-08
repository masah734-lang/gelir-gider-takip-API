using GelirGiderTakip.Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace GelirGiderTakip.Api.Models
{
    public class Fis
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string DosyaYolu { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string OrijinalDosyaAdi { get; set; } = string.Empty;

        public string? HamOcrMetni { get; set; }

        [MaxLength(150)]
        public string? AlgilananIsletmeAdi { get; set; }

        public decimal? AlgilananTutar { get; set; }

        public DateTime? AlgilananTarih { get; set; }

        public FisDurumu Durum { get; set; }
            = FisDurumu.Bekliyor;

        [MaxLength(1000)]
        public string? HataMesaji { get; set; }

        public DateTime YuklenmeTarihi { get; set; }
            = DateTime.UtcNow;

        public DateTime? IslenmeTarihi { get; set; }

        public int KullaniciId { get; set; }
        public Kullanici Kullanici { get; set; } = null!;

        public ICollection<FisDetayi> FisDetaylari { get; set; }
         = new List<FisDetayi>();

        public FinansalIslem? FinansalIslem { get; set; }
    }
}