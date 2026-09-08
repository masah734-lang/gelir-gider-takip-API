using GelirGiderTakip.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GelirGiderTakip.Api.Enums;

namespace GelirGiderTakip.Api.Data.Yapilandirmalar
{
    public class KategoriYapilandirmasi
        : IEntityTypeConfiguration<Kategori>
    {
        public void Configure(EntityTypeBuilder<Kategori> builder)
        {
            builder.ToTable("Kategoriler");

            builder.HasOne(kategori => kategori.Kullanici)
                .WithMany(kullanici => kullanici.Kategoriler)
                .HasForeignKey(kategori => kategori.KullaniciId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(kategori => new
            {
                kategori.Ad,
                kategori.Tur
            })
            .IsUnique()
            .HasFilter("[KullaniciId] IS NULL");

            builder.HasIndex(kategori => new
            {
                kategori.KullaniciId,
                kategori.Ad,
                kategori.Tur
            })
            .IsUnique()
            .HasFilter("[KullaniciId] IS NOT NULL");

            //seed data 
            var varsayilanOlusturulmaTarihi =
             new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                new Kategori
                {
                    Id = 1,
                    Ad = "Maaş",
                    Tur = IslemTuru.Gelir,
                    AktifMi = true,
                    OlusturulmaTarihi = varsayilanOlusturulmaTarihi,
                    KullaniciId = null
                },
                new Kategori
                {
                    Id = 2,
                    Ad = "Ek Gelir",
                    Tur = IslemTuru.Gelir,
                    AktifMi = true,
                    OlusturulmaTarihi = varsayilanOlusturulmaTarihi,
                    KullaniciId = null
                },
                new Kategori
                {
                    Id = 3,
                    Ad = "Yatırım Geliri",
                    Tur = IslemTuru.Gelir,
                    AktifMi = true,
                    OlusturulmaTarihi = varsayilanOlusturulmaTarihi,
                    KullaniciId = null
                },
                new Kategori
                {
                    Id = 4,
                    Ad = "Kira Geliri",
                    Tur = IslemTuru.Gelir,
                    AktifMi = true,
                    OlusturulmaTarihi = varsayilanOlusturulmaTarihi,
                    KullaniciId = null
                },
                new Kategori
                {
                    Id = 5,
                    Ad = "Diğer Gelir",
                    Tur = IslemTuru.Gelir,
                    AktifMi = true,
                    OlusturulmaTarihi = varsayilanOlusturulmaTarihi,
                    KullaniciId = null
                },
                new Kategori
                {
                    Id = 6,
                    Ad = "Market",
                    Tur = IslemTuru.Gider,
                    AktifMi = true,
                    OlusturulmaTarihi = varsayilanOlusturulmaTarihi,
                    KullaniciId = null
                },
                new Kategori
                {
                    Id = 7,
                    Ad = "Yeme İçme",
                    Tur = IslemTuru.Gider,
                    AktifMi = true,
                    OlusturulmaTarihi = varsayilanOlusturulmaTarihi,
                    KullaniciId = null
                },
                new Kategori
                {
                    Id = 8,
                    Ad = "Ulaşım",
                    Tur = IslemTuru.Gider,
                    AktifMi = true,
                    OlusturulmaTarihi = varsayilanOlusturulmaTarihi,
                    KullaniciId = null
                },
                new Kategori
                {
                    Id = 9,
                    Ad = "Faturalar",
                    Tur = IslemTuru.Gider,
                    AktifMi = true,
                    OlusturulmaTarihi = varsayilanOlusturulmaTarihi,
                    KullaniciId = null
                },
                new Kategori
                {
                    Id = 10,
                    Ad = "Kira",
                    Tur = IslemTuru.Gider,
                    AktifMi = true,
                    OlusturulmaTarihi = varsayilanOlusturulmaTarihi,
                    KullaniciId = null
                },
                new Kategori
                {
                    Id = 11,
                    Ad = "Sağlık",
                    Tur = IslemTuru.Gider,
                    AktifMi = true,
                    OlusturulmaTarihi = varsayilanOlusturulmaTarihi,
                    KullaniciId = null
                },
                new Kategori
                {
                    Id = 12,
                    Ad = "Eğitim",
                    Tur = IslemTuru.Gider,
                    AktifMi = true,
                    OlusturulmaTarihi = varsayilanOlusturulmaTarihi,
                    KullaniciId = null
                },
                new Kategori
                {
                    Id = 13,
                    Ad = "Giyim",
                    Tur = IslemTuru.Gider,
                    AktifMi = true,
                    OlusturulmaTarihi = varsayilanOlusturulmaTarihi,
                    KullaniciId = null
                },
                new Kategori
                {
                    Id = 14,
                    Ad = "Eğlence",
                    Tur = IslemTuru.Gider,
                    AktifMi = true,
                    OlusturulmaTarihi = varsayilanOlusturulmaTarihi,
                    KullaniciId = null
                },
                new Kategori
                {
                    Id = 15,
                    Ad = "Abonelikler",
                    Tur = IslemTuru.Gider,
                    AktifMi = true,
                    OlusturulmaTarihi = varsayilanOlusturulmaTarihi,
                    KullaniciId = null
                },
                new Kategori
                {
                    Id = 16,
                    Ad = "Diğer Gider",
                    Tur = IslemTuru.Gider,
                    AktifMi = true,
                    OlusturulmaTarihi = varsayilanOlusturulmaTarihi,
                    KullaniciId = null
                }
            );


        }
    }
}