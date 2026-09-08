using System.ComponentModel.DataAnnotations;

namespace GelirGiderTakip.Api.Models
{
    public class FisDetayi
    {
        public int Id { get; set; }

        public int FisId { get; set; }

        public int? KategoriId { get; set; }

        [Required]
        [MaxLength(250)]
        public string UrunAdi { get; set; } = string.Empty;

        public decimal? Miktar { get; set; }

        public decimal? BirimFiyat { get; set; }

        public decimal ToplamTutar { get; set; }

        [MaxLength(500)]
        public string? HamSatirMetni { get; set; }

        public int SiraNo { get; set; }

        public Fis Fis { get; set; } = null!;

        public Kategori? Kategori { get; set; }
    }
}