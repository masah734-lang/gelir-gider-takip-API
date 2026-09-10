using System.ComponentModel.DataAnnotations;
using GelirGiderTakip.Api.Enums;

namespace GelirGiderTakip.Api.DTOs.Kategoriler;

public class KategoriPostDto
{
    [Required]
    [MaxLength(100)]
    public string Ad { get; set; } = string.Empty;

    public IslemTuru Tur { get; set; }

    public int KullaniciId { get; set; }
}