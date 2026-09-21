namespace GelirGiderTakip.Api.DTOs.Butceler
{
    public class ButcePostDto
    {
        public short Yil { get; set; }

        public byte Ay { get; set; }

        public decimal LimitTutari { get; set; }

        public int? KategoriId { get; set; }
    }
}