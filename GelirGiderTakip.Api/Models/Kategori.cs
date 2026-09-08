using GelirGiderTakip.Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace GelirGiderTakip.Api.Models
{
    public class Kategori
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Ad { get; set; } = string.Empty;

        public IslemTuru Tur { get; set; }

        public bool AktifMi { get; set; } = true;

        public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;

        public DateTime? GuncellenmeTarihi { get; set; }

        public int? KullaniciId { get; set; }
        public Kullanici? Kullanici { get; set; }

        public ICollection<FisDetayi> FisDetaylari { get; set; }
         = new List<FisDetayi>();
        public ICollection<FinansalIslem> FinansalIslemler { get; set; }
        = new List<FinansalIslem>();
        public ICollection<Butce> Butceler { get; set; }
            = new List<Butce>();
    }
}
