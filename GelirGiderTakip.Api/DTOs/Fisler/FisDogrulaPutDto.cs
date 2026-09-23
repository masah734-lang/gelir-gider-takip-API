namespace GelirGiderTakip.Api.DTOs.Fisler
{
    public class FisDogrulaPutDto
    {
        public string? IsletmeAdi { get; set; }

        public decimal? Tutar { get; set; }

        public DateTime? Tarih { get; set; }
        public int? KategoriId { get; set; }
    }
}