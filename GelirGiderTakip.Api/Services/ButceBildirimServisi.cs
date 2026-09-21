using GelirGiderTakip.Api.Data;
using GelirGiderTakip.Api.Enums;
using GelirGiderTakip.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GelirGiderTakip.Api.Services
{
    public class ButceBildirimServisi
    {
        private readonly AppDBContext _context;

        public ButceBildirimServisi(AppDBContext context)
        {
            _context = context;
        }

        public async Task ButceleriKontrolEt(
            int kullaniciId,
            DateTime islemTarihi,
            int? kategoriId)
        {
            var yil = (short)islemTarihi.Year;
            var ay = (byte)islemTarihi.Month;

            var baslangic = new DateTime(yil, ay, 1);
            var bitis = baslangic.AddMonths(1);

            var butceler = await _context.Butceler
                .Where(butce =>
                    butce.KullaniciId == kullaniciId &&
                    butce.Yil == yil &&
                    butce.Ay == ay &&
                    (butce.KategoriId == null ||
                     butce.KategoriId == kategoriId))
                .ToListAsync();

            foreach (var butce in butceler)
            {
                var giderSorgusu = _context.FinansalIslemler
                    .AsNoTracking()
                    .Where(islem =>
                        islem.KullaniciId == kullaniciId &&
                        islem.Tur == IslemTuru.Gider &&
                        islem.IslemTarihi >= baslangic &&
                        islem.IslemTarihi < bitis);

                if (butce.KategoriId.HasValue)
                {
                    giderSorgusu = giderSorgusu.Where(islem =>
                        islem.KategoriId == butce.KategoriId.Value);
                }

                var harcananTutar = await giderSorgusu
                    .SumAsync(islem => (decimal?)islem.Tutar) ?? 0;

                if (butce.LimitTutari <= 0)
                {
                    continue;
                }

                var oran = harcananTutar / butce.LimitTutari;

                // Kategori adini bul
                string butceAdi;

                if (butce.KategoriId.HasValue)
                {
                    butceAdi = await _context.Kategoriler
                        .AsNoTracking()
                        .Where(kategori =>
                            kategori.Id == butce.KategoriId.Value)
                        .Select(kategori => kategori.Ad)
                        .FirstOrDefaultAsync()
                        ?? "Kategori";
                }
                else
                {
                    butceAdi = "Genel";
                }

                if (oran >= 1)
                {
                    // Artık "yaklaşılıyor" bildirimi anlamsız.
                    await BildirimSil(
                        kullaniciId,
                        butce.Id,
                        BildirimTuru.ButceyeYaklasildi);

                    await BildirimOlusturVeyaGuncelle(
                        kullaniciId,
                        butce,
                        BildirimTuru.ButceAsildi,
                        $"{butceAdi} butcesi asildi",
                        $"Kategori: {butceAdi}. " +
                        $"Butce limiti: {butce.LimitTutari:N2} TL. " +
                        $"Harcanan tutar: {harcananTutar:N2} TL. " +
                        $"Butceyi {harcananTutar - butce.LimitTutari:N2} TL astiniz.");
                }
                else if (oran >= 0.80m)
                {
                    await BildirimOlusturVeyaGuncelle(
                        kullaniciId,
                        butce,
                        BildirimTuru.ButceyeYaklasildi,
                        $"{butceAdi} butce limitine yaklasiyor",
                        $"Kategori: {butceAdi}. " +
                        $"Butce limiti: {butce.LimitTutari:N2} TL. " +
                        $"Harcanan tutar: {harcananTutar:N2} TL. " +
                        $"Kalan tutar: {butce.LimitTutari - harcananTutar:N2} TL. " +
                        $"Butcenizin %{oran * 100:N0} kadarini kullandiniz.");
                }
                else
                {
                    // Harcama tekrar %80 altina indiyse eski uyariyi temizle.
                    await BildirimSil(
                        kullaniciId,
                        butce.Id,
                        BildirimTuru.ButceyeYaklasildi);

                    await BildirimSil(
                        kullaniciId,
                        butce.Id,
                        BildirimTuru.ButceAsildi);
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task BildirimOlusturVeyaGuncelle(
            int kullaniciId,
            Butce butce,
            BildirimTuru tur,
            string baslik,
            string mesaj)
        {
            var mevcutBildirim = await _context.Bildirimler
                .FirstOrDefaultAsync(bildirim =>
                    bildirim.KullaniciId == kullaniciId &&
                    bildirim.ButceId == butce.Id &&
                    bildirim.Tur == tur);

            if (mevcutBildirim != null)
            {
                // Eski kaydi yeni harcama bilgileriyle güncelle
                mevcutBildirim.Baslik = baslik;
                mevcutBildirim.Mesaj = mesaj;

                // Yeni bir uyarı gibi tekrar okunmamis kabul et
                mevcutBildirim.OkunmaTarihi = null;

                return;
            }

            var yeniBildirim = new Bildirim
            {
                KullaniciId = kullaniciId,
                ButceId = butce.Id,
                Tur = tur,
                Baslik = baslik,
                Mesaj = mesaj,
                OlusturulmaTarihi = DateTime.UtcNow
            };

            _context.Bildirimler.Add(yeniBildirim);
        }

        private async Task BildirimSil(
            int kullaniciId,
            int butceId,
            BildirimTuru tur)
        {
            var bildirim = await _context.Bildirimler
                .FirstOrDefaultAsync(b =>
                    b.KullaniciId == kullaniciId &&
                    b.ButceId == butceId &&
                    b.Tur == tur);

            if (bildirim != null)
            {
                _context.Bildirimler.Remove(bildirim);
            }
        }
    }
}