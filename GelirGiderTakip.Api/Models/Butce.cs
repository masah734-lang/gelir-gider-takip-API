using GelirGiderTakip.Api.Models;

namespace GelirGiderTakip.Api.Models
{
    public class Butce
    {
        public int Id { get; set; }

        public short Yil { get; set; }

        public byte Ay { get; set; }

        public decimal LimitTutari { get; set; }

        public DateTime OlusturulmaTarihi { get; set; }
        = DateTime.UtcNow;

        public DateTime? GuncellenmeTarihi { get; set; }

        public int KullaniciId { get; set; }
        public Kullanici Kullanici { get; set; } = null!;

        public int? KategoriId { get; set; }
        public Kategori? Kategori { get; set; }
        public ICollection<Bildirim> Bildirimler { get; set; }  
        = new List<Bildirim>();
    }
}