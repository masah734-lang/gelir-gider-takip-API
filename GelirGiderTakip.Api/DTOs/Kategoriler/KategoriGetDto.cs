using GelirGiderTakip.Api.Enums;

namespace GelirGiderTakip.Api.DTOs.Kategoriler;

public class KategoriGetDto
{
    public int Id { get; set; }

    public string Ad { get; set; } = string.Empty;

    public IslemTuru Tur { get; set; }

    public bool VarsayilanMi { get; set; }
}