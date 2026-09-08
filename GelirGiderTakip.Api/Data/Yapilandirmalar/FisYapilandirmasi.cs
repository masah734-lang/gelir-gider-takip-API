using GelirGiderTakip.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GelirGiderTakip.Api.Data.Yapilandirmalar
{
    public class FisYapilandirmasi
        : IEntityTypeConfiguration<Fis>
    {
        public void Configure(EntityTypeBuilder<Fis> builder)
        {
            builder.ToTable("Fisler");

            builder.Property(fis => fis.AlgilananTutar)
                .HasPrecision(18, 2);

            builder.HasOne(fis => fis.Kullanici)
                .WithMany(kullanici => kullanici.Fisler)
                .HasForeignKey(fis => fis.KullaniciId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(fis => new
            {
                fis.KullaniciId,
                fis.YuklenmeTarihi
            });
        }
    }
}