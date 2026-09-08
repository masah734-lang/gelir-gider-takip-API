using GelirGiderTakip.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
        }
    }
}