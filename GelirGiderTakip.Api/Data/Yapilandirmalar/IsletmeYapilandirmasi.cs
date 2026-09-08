using GelirGiderTakip.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GelirGiderTakip.Api.Data.Yapilandirmalar
{
    public class IsletmeYapilandirmasi
        : IEntityTypeConfiguration<Isletme>
    {
        public void Configure(EntityTypeBuilder<Isletme> builder)
        {
            builder.ToTable("Isletmeler");

            builder.HasOne(isletme => isletme.OlusturanKullanici)
                .WithMany(kullanici => kullanici.OlusturduguIsletmeler)
                .HasForeignKey(isletme => isletme.OlusturanKullaniciId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(isletme => isletme.Ad)
                .IsUnique()
                .HasFilter("[OlusturanKullaniciId] IS NULL");

            builder.HasIndex(isletme => new
            {
                isletme.OlusturanKullaniciId,
                isletme.Ad
            })
            .IsUnique()
            .HasFilter("[OlusturanKullaniciId] IS NOT NULL");
        }
    }
}