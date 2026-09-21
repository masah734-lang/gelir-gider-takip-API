using GelirGiderTakip.Api.Enums;

namespace GelirGiderTakip.Api.DTOs.Bildirimler
{
    public class BildirimGetDto
    {
        public int Id { get; set; }

        public BildirimTuru Tur { get; set; }

        public string Baslik { get; set; } = string.Empty;

        public string Mesaj { get; set; } = string.Empty;

        public DateTime OlusturulmaTarihi { get; set; }

        public DateTime? OkunmaTarihi { get; set; }

        public bool OkunduMu { get; set; }
    }
}