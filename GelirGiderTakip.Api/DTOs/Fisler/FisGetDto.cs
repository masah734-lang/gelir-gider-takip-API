namespace GelirGiderTakip.Api.DTOs.Fisler
{
    public class FisGetDto
    {
        public int Id { get; set; }

        public string OrijinalDosyaAdi { get; set; } = string.Empty;

        public string DosyaYolu { get; set; } = string.Empty;

        public string? AlgilananIsletmeAdi { get; set; }

        public decimal? AlgilananTutar { get; set; }

        public DateTime? AlgilananTarih { get; set; }

        public DateTime YuklenmeTarihi { get; set; }

        public string? HamOcrMetni { get; set; }
    }
}