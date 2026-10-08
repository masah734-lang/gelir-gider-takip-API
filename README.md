# Gelir-Gider Takip API

Backend geliştirme stajım kapsamında geliştirdiğim **ASP.NET Core Web API** projesi. Gelir ve gider kayıtlarını yönetme, harcamaları kategorilere ayırma, aylık bütçe takibi ve fiş görsellerinden OCR ile veri çıkarma işlevlerini içerir.

## Özellikler

- **Kullanıcı yönetimi:** Kayıt, giriş ve JWT ile kimlik doğrulama; kullanıcıya özel kayıt erişimi.
- **Gelir-gider takibi:** Finansal işlem oluşturma, listeleme, güncelleme ve silme.
- **Kategori yönetimi:** Hazır ve kullanıcıya özel kategoriler; kategorileri pasifleştirerek geçmiş kayıtları koruma.
- **Fiş işleme:** JPG/PNG fiş yükleme, Tesseract OCR ile işletme, tarih, tutar ve ürün bilgilerini ayrıştırma; sonuçları düzeltme ve finansal işlem oluşturma.
- **Bütçe ve bildirimler:** Genel veya kategori bazlı aylık bütçeler; %80 kullanım ve limit aşımı bildirimleri.
- **Raporlama:** Gelir-gider özeti, aylık raporlar ve kategori bazlı gider dağılımı.

## Teknolojiler

C# · .NET 10 · ASP.NET Core · Entity Framework Core · Microsoft SQL Server · JWT · Tesseract OCR · Swagger / OpenAPI

## Kurulum

**Gereksinimler:** .NET 10 SDK, SQL Server ve Entity Framework CLI (`dotnet-ef` 10.x).

1. Repoyu klonlayın ve paketleri yükleyin:

   ```bash
   git clone https://github.com/masah734-lang/gelir-gider-takip-API.git
   cd gelir-gider-takip-API
   dotnet restore
   cd GelirGiderTakip.Api
   ```

2. SQL Server bağlantısını ve JWT anahtarını User Secrets üzerinden tanımlayın. Aşağıdaki değerleri kendi ortamınıza göre değiştirin:

   ```bash
   dotnet user-secrets set "ConnectionStrings:GelirGiderTakipDb" "Server=localhost;Database=GelirGiderTakipDb;Trusted_Connection=True;TrustServerCertificate=True"
   dotnet user-secrets set "Jwt:Key" "<en-az-32-baytlik-rastgele-gizli-anahtar>"
   ```

   SQL Server kimlik doğrulama yönteminize göre bağlantı dizesini düzenleyin. Gizli anahtarınızı repoya eklemeyin.

3. Veritabanını oluşturun ve uygulamayı başlatın:

   ```bash
   dotnet ef database update
   dotnet run --launch-profile http
   ```

4. Swagger arayüzünü açın: [http://localhost:5218/swagger](http://localhost:5218/swagger).

**OCR:** Türkçe dil dosyası `tessdata/tur.traineddata` repoda bulunmaktadır. OCR için Tesseract'ın yerel bağımlılıklarının da çalıştırılan ortamla uyumlu olması gerekir.

## Kullanım

1. `POST /api/kullanicilar/kayit` ile kayıt oluşturun.
2. `POST /api/kullanicilar/giris` ile JWT alın ve Swagger'daki **Authorize** alanına token'ı girin.
3. Kategori, finansal işlem, fiş ve bütçe uç noktalarını Swagger üzerinden deneyin.

HTTP isteklerinde token `Authorization: Bearer <token>` başlığıyla gönderilir.

## Notlar

- Proje yalnızca backend API içerir; web veya mobil arayüz bulunmaz.
- OCR doğruluğu fiş formatına ve görüntü kalitesine bağlıdır. Ayrıştırılan bilgiler kullanıcı tarafından düzeltilebilir.
