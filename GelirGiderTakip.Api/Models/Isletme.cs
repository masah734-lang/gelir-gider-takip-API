using System.ComponentModel.DataAnnotations;

namespace GelirGiderTakip.Api.Models
{
    public class Isletme
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Ad { get; set; } = string.Empty;

        public bool AktifMi { get; set; } = true;

        public DateTime OlusturulmaTarihi { get; set; }
            = DateTime.UtcNow;

        public DateTime? GuncellenmeTarihi { get; set; }

        public int? OlusturanKullaniciId { get; set; }
        public Kullanici? OlusturanKullanici { get; set; }

        public ICollection<FinansalIslem> FinansalIslemler { get; set; }
        = new List<FinansalIslem>();
    }
}

