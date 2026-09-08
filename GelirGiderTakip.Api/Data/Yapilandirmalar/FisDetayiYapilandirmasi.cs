using GelirGiderTakip.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GelirGiderTakip.Api.Data.Yapilandirmalar
{
    public class FisDetayiYapilandirmasi
        : IEntityTypeConfiguration<FisDetayi>
    {
        public void Configure(EntityTypeBuilder<FisDetayi> builder)
        {
            builder.ToTable("FisDetaylari");

            builder.Property(detay => detay.Miktar)
                .HasPrecision(10, 3);

            builder.Property(detay => detay.BirimFiyat)
                .HasPrecision(18, 2);

            builder.Property(detay => detay.ToplamTutar)
                .HasPrecision(18, 2);

            builder.HasOne(detay => detay.Fis)
                .WithMany(fis => fis.FisDetaylari)
                .HasForeignKey(detay => detay.FisId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(detay => detay.Kategori)
                .WithMany(kategori => kategori.FisDetaylari)
                .HasForeignKey(detay => detay.KategoriId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(detay => new
            {
                detay.FisId,
                detay.SiraNo
            })
            .IsUnique();
        }
    }
}