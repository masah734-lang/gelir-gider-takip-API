using GelirGiderTakip.Api.Enums;

namespace GelirGiderTakip.Api.DTOs.FinansalIslemler
{
    public class FinansalIslemPatchDto
    {
        public IslemTuru? Tur { get; set; }

        public decimal? Tutar { get; set; }

        public DateTime? IslemTarihi { get; set; }

        public string? Aciklama { get; set; }

        public int? KategoriId { get; set; }
    }
}