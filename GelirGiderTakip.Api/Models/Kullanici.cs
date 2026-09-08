using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GelirGiderTakip.Api.Models
{
  
    public class Kullanici
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Ad { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Soyad { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]
        [EmailAddress]
        public string Eposta { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string ParolaHash { get; set; } = string.Empty;

        public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;

        public DateTime? GuncellenmeTarihi { get; set; }

        public bool AktifMi { get; set; } = true;

        public ICollection<Kategori> Kategoriler { get; set; }
            = new List<Kategori>();
        public ICollection<Isletme> OlusturduguIsletmeler { get; set; }
            = new List<Isletme>();
        public ICollection<Fis> Fisler { get; set; }
            = new List<Fis>();

    }
}
