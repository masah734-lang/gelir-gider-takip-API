using GelirGiderTakip.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GelirGiderTakip.Api.Data.Yapilandirmalar
{
    public class ButceYapilandirmasi
        : IEntityTypeConfiguration<Butce>
    {
        public void Configure(EntityTypeBuilder<Butce> builder)
        {
            builder.ToTable("Butceler", tablo =>
            {
                tablo.HasCheckConstraint(
                    "CK_Butceler_Ay",
                    "[Ay] BETWEEN 1 AND 12");

                tablo.HasCheckConstraint(
                    "CK_Butceler_Yil",
                    "[Yil] BETWEEN 2000 AND 2100");

                tablo.HasCheckConstraint(
                    "CK_Butceler_LimitTutari",
                    "[LimitTutari] > 0");
            });

            builder.Property(butce => butce.LimitTutari)
                .HasPrecision(18, 2);

            builder.HasOne(butce => butce.Kullanici)
                .WithMany(kullanici => kullanici.Butceler)
                .HasForeignKey(butce => butce.KullaniciId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(butce => butce.Kategori)
                .WithMany(kategori => kategori.Butceler)
                .HasForeignKey(butce => butce.KategoriId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(butce => new
            {
                butce.KullaniciId,
                butce.Yil,
                butce.Ay
            })
            .IsUnique()
            .HasFilter("[KategoriId] IS NULL");

            builder.HasIndex(butce => new
            {
                butce.KullaniciId,
                butce.KategoriId,
                butce.Yil,
                butce.Ay
            })
            .IsUnique()
            .HasFilter("[KategoriId] IS NOT NULL");
        }
    }
}