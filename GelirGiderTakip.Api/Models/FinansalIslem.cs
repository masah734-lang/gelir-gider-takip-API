using GelirGiderTakip.Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace GelirGiderTakip.Api.Models
{
    public class FinansalIslem
    {
        public int Id { get; set; }

        public IslemTuru Tur { get; set; }

        public decimal Tutar { get; set; }

        public DateTime IslemTarihi { get; set; }

        [MaxLength(500)]
        public string? Aciklama { get; set; }

        public DateTime OlusturulmaTarihi { get; set; }
            = DateTime.UtcNow;

        public DateTime? GuncellenmeTarihi { get; set; }

        public int KullaniciId { get; set; }
        public Kullanici Kullanici { get; set; } = null!;

        public int? KategoriId { get; set; }
        public Kategori? Kategori { get; set; }

        public int? FisId { get; set; }
        public Fis? Fis { get; set; }

        public int? IsletmeId { get; set; }
        public Isletme? Isletme { get; set; }
    }
}