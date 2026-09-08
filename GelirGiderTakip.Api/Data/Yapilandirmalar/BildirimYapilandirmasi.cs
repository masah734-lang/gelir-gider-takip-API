using GelirGiderTakip.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GelirGiderTakip.Api.Data.Yapilandirmalar
{
    public class BildirimYapilandirmasi
        : IEntityTypeConfiguration<Bildirim>
    {
        public void Configure(EntityTypeBuilder<Bildirim> builder)
        {
            builder.ToTable("Bildirimler", tablo =>
            {
                tablo.HasCheckConstraint(
                    "CK_Bildirimler_Tur",
                    "[Tur] IN (1, 2, 3)");
            });

            builder.HasOne(bildirim => bildirim.Kullanici)
                .WithMany(kullanici => kullanici.Bildirimler)
                .HasForeignKey(bildirim => bildirim.KullaniciId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(bildirim => bildirim.Butce)
                .WithMany(butce => butce.Bildirimler)
                .HasForeignKey(bildirim => bildirim.ButceId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(bildirim => new
            {
                bildirim.KullaniciId,
                bildirim.OkunmaTarihi,
                bildirim.OlusturulmaTarihi
            });
        }
    }
}