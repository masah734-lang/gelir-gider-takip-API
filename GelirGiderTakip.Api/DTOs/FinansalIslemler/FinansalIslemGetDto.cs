using GelirGiderTakip.Api.Enums;

namespace GelirGiderTakip.Api.DTOs.FinansalIslemler
{
    public class FinansalIslemGetDto
    {
        public int Id { get; set; }

        public IslemTuru Tur { get; set; }

        public decimal Tutar { get; set; }

        public DateTime IslemTarihi { get; set; }

        public string? Aciklama { get; set; }

        public int? KategoriId { get; set; }

        public string? KategoriAdi { get; set; }

        public int? IsletmeId { get; set; }

        public string? IsletmeAdi { get; set; }

        public int? FisId { get; set; }
    }
}