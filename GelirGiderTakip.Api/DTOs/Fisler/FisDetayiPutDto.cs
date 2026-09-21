namespace GelirGiderTakip.Api.DTOs.Fisler
{
    public class FisDetayiPutDto
    {
        public string UrunAdi { get; set; } = string.Empty;

        public decimal? Miktar { get; set; }

        public decimal? BirimFiyat { get; set; }

        public decimal ToplamTutar { get; set; }

        public int? KategoriId { get; set; }
    }
}