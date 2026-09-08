using GelirGiderTakip.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GelirGiderTakip.Api.Data.Yapilandirmalar
{
    public class FinansalIslemYapilandirmasi
        : IEntityTypeConfiguration<FinansalIslem>
    {
        public void Configure(
            EntityTypeBuilder<FinansalIslem> builder)
        {
            builder.ToTable("FinansalIslemler", tablo =>
            {
                tablo.HasCheckConstraint(
                    "CK_FinansalIslemler_Tutar",
                    "[Tutar] > 0");

                tablo.HasCheckConstraint(
                    "CK_FinansalIslemler_Tur",
                    "[Tur] IN (1, 2)");
            });

            builder.Property(islem => islem.Tutar)
                .HasPrecision(18, 2);

            builder.HasOne(islem => islem.Kullanici)
                .WithMany(kullanici => kullanici.FinansalIslemler)
                .HasForeignKey(islem => islem.KullaniciId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(islem => islem.Kategori)
                .WithMany(kategori => kategori.FinansalIslemler)
                .HasForeignKey(islem => islem.KategoriId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(islem => islem.Isletme)
                .WithMany(isletme => isletme.FinansalIslemler)
                .HasForeignKey(islem => islem.IsletmeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(islem => islem.Fis)
                .WithOne(fis => fis.FinansalIslem)
                .HasForeignKey<FinansalIslem>(
                    islem => islem.FisId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(islem => new
            {
                islem.KullaniciId,
                islem.IslemTarihi
            });
        }
    }
}