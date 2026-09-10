using System.ComponentModel.DataAnnotations;
using GelirGiderTakip.Api.Enums;

namespace GelirGiderTakip.Api.DTOs.Kategoriler;

public class KategoriPutDto
{
    [Required]
    [MaxLength(100)]
    public string Ad { get; set; } = string.Empty;

    public IslemTuru Tur { get; set; }
}