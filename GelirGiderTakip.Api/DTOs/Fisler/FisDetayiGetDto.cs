namespace GelirGiderTakip.Api.DTOs.Fisler
{
    public class FisDetayiGetDto
    {
        public int Id { get; set; }

        public string UrunAdi { get; set; } = string.Empty;

        public decimal? Miktar { get; set; }

        public decimal? BirimFiyat { get; set; }

        public decimal ToplamTutar { get; set; }

        public string? HamSatirMetni { get; set; }

        public int SiraNo { get; set; }

        public int? KategoriId { get; set; }
    }
}