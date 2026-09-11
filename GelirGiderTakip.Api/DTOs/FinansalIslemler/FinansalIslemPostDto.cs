using GelirGiderTakip.Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace GelirGiderTakip.Api.DTOs.FinansalIslemler
{
    public class FinansalIslemPostDto
    {
        public IslemTuru Tur { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Tutar { get; set; }

        public DateTime IslemTarihi { get; set; }

        [MaxLength(500)]
        public string? Aciklama { get; set; }

        public int? KategoriId { get; set; }

        public int? IsletmeId { get; set; }
    }
}