namespace GelirGiderTakip.Api.DTOs.Butceler
{
    public class ButceGetDto
    {
        public int Id { get; set; }

        public short Yil { get; set; }

        public byte Ay { get; set; }

        public decimal LimitTutari { get; set; }

        public int? KategoriId { get; set; }

        public string? KategoriAdi { get; set; }

        public decimal HarcananTutar { get; set; }

        public decimal KalanTutar { get; set; }
    }
}