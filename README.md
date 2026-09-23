# Gelir-Gider Takip API

Gelir-Gider Takip API; kullanıcıların gelir-gider işlemlerini takip edebilmesi, harcamalarını kategorilere ayırabilmesi, bütçe oluşturabilmesi, rapor alabilmesi ve fiş görsellerini OCR ile işleyebilmesi amacıyla geliştirilmiş bir **ASP.NET Core Web API** projesidir.

Proje backend geliştirme stajı kapsamında geliştirilmiştir.

---

# Projeyi Çalıştırma

Projeyi kendi bilgisayarında denemek isteyenler aşağıdaki adımları takip edebilir.

## Gereksinimler

* .NET 10 SDK
* SQL Server
* Git
* Visual Studio, Visual Studio Code veya Rider

Entity Framework CLI kullanılacaksa:

```bash
dotnet tool install --global dotnet-ef
```

---

## 1. Repository'yi Klonlayın

```bash
git clone https://github.com/masah734-lang/gelir-gider-takip-API.git
```

Proje klasörüne geçin:

```bash
cd gelir-gider-takip-API
```

---

## 2. NuGet Paketlerini Yükleyin

```bash
dotnet restore
```

---

## 3. SQL Server Bağlantısını Ayarlayın

`appsettings.json` içerisindeki connection string'i kendi SQL Server kurulumunuza göre düzenleyin.

Örnek:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=GelirGiderTakipDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

SQL Server instance adına göre `Server` değeri değiştirilmelidir.

---

## 4. JWT Ayarlarını Yapın

JWT oluşturmak için gerekli signing key yapılandırılmalıdır.

Gizli anahtarların repository içerisinde tutulması önerilmez.

Geliştirme ortamında:

```bash
dotnet user-secrets
```

veya environment variable kullanılabilir.

---

## 5. Veritabanını Oluşturun

Mevcut migration'ları SQL Server'a uygulamak için:

```bash
dotnet ef database update --project GelirGiderTakip.Api
```

Visual Studio Package Manager Console kullanılıyorsa:

```powershell
Update-Database
```

komutu da kullanılabilir.

Migration işlemi tamamlandığında:

```text
GelirGiderTakipDb
```

veritabanı oluşturulur.

---

## 6. OCR Dosyalarını Kontrol Edin

Tesseract OCR'ın çalışabilmesi için proje içerisindeki:

```text
tessdata/
```

klasörünün ve gerekli Türkçe dil dosyasının bulunması gerekir.

Bu dosya bulunmazsa fiş OCR işlemi çalışmayacaktır.

---

## 7. Projeyi Çalıştırın

```bash
dotnet run --project GelirGiderTakip.Api
```

Visual Studio kullanılıyorsa proje doğrudan başlatılabilir.

Uygulama çalıştıktan sonra Swagger/OpenAPI arayüzünden endpointler test edilebilir.

---

# API Nasıl Kullanılır?

Genel kullanım sırası şöyledir:

```text
Kullanıcı kayıt olur
        ↓
Kullanıcı giriş yapar
        ↓
JWT Token alınır
        ↓
Token Swagger'a eklenir
        ↓
Kategori / işlem / fiş / bütçe işlemleri yapılır
```

---

## 1. Kullanıcı Oluşturma

```text
POST /api/kullanicilar/kayit
```

ile kullanıcı oluşturulur.

Temel bilgiler:

```text
Ad
Soyad
Eposta
Parola
```

---

## 2. Giriş Yapma

```text
POST /api/kullanicilar/giris
```

endpointi üzerinden giriş yapılır.

Başarılı giriş sonucunda JWT token döndürülür.

---

## 3. JWT Token Kullanımı

Swagger'da **Authorize** butonuna basılarak token:

```text
Bearer {token}
```

şeklinde girilir.

Normal HTTP isteğinde ise:

```http
Authorization: Bearer {token}
```

header'ı kullanılmalıdır.

Bundan sonra `[Authorize]` ile korunan endpointlere erişilebilir.

---

# Örnek Kullanım

## Manuel Gelir veya Gider Ekleme

Öncelikle kategoriler alınır:

```text
GET /api/kategoriler
```

Daha sonra seçilen kategoriyle finansal işlem oluşturulabilir.

Örneğin:

```text
Tur: Gider
Kategori: Market
Tutar: 750 TL
Tarih: 23.09.2026
```

---

# Fiş ile Gider Ekleme

Fiş yükleme:

```text
POST /api/fisler
```

üzerinden yapılır.

Desteklenen dosya türleri:

```text
.jpg
.jpeg
.png
```

Temel akış:

```text
Fiş Görseli
      ↓
Tesseract OCR
      ↓
Ham OCR Metni
      ↓
İşletme / Tarih / Tutar
      ↓
Finansal İşlem
```

OCR sonucu hatalıysa:

```text
PUT /api/fisler/{id}/dogrula
```

endpointi üzerinden düzeltilebilir.

Fiş içerisindeki ürünler:

```text
GET /api/fisler/{id}/detaylar
```

ile görüntülenebilir.

---

# Projenin Temel Özellikleri

* Kullanıcı kayıt ve giriş sistemi
* JWT Authentication ve Authorization
* Gelir ve gider kategorileri
* Finansal işlem yönetimi
* Kullanıcı bazlı veri izolasyonu
* Fiş yükleme
* Tesseract OCR
* OCR sonucundan finansal işlem oluşturma
* Fiş doğrulama
* Fiş ürün detayları
* Bütçe yönetimi
* Bütçe bildirimleri
* Finansal raporlama

---

# Kullanılan Teknolojiler

* C#
* .NET 10
* ASP.NET Core Web API
* Entity Framework Core
* Microsoft SQL Server
* JWT
* Tesseract OCR
* Swagger / OpenAPI
* Git
* GitHub

---

# Genel Mimari

```text
Client
   ↓
Controller
   ↓
DTO
   ↓
Service / Business Logic
   ↓
Entity Framework Core
   ↓
SQL Server
```

API giriş ve çıkışlarında mümkün olduğunca DTO kullanılmıştır.

Böylece veritabanındaki entity yapısı doğrudan dışarı açılmamaktadır.

---

# Veritabanı Yapısı

Projede temel olarak şu tablolar bulunmaktadır:

```text
Kullanicilar
Kategoriler
Isletmeler
FinansalIslemler
Fisler
FisDetaylari
Butceler
Bildirimler
```

---

# Kritik Tasarım Kararları

## 1. Kullanıcı Güvenliği

Kullanıcı parolaları açık şekilde tutulmaz, hashlenerek saklanır.

E-posta adresi benzersizdir.

Kullanıcının kimliği request içerisinden alınmak yerine JWT içerisindeki:

```text
ClaimTypes.NameIdentifier
```

alanından belirlenir.

Böylece kullanıcı başka bir kullanıcının ID'sini göndererek onun verilerine erişemez.

---

## 2. Kategoriler Fiziksel Olarak Silinmez

Kategori silme işlemi gerçek SQL `DELETE` işlemi değildir.

Bunun yerine:

```text
AktifMi = false
```

yapılır.

Örneğin kullanıcı daha önce:

```text
Market → 1.500 TL gider
```

kaydetmiş olsun.

`Market` kategorisi tamamen silinirse geçmiş finansal işlemlerin kategori ilişkisi bozulabilir.

Bu nedenle kategori yalnızca pasif hale getirilir.

Geçmiş işlemler korunurken kategori yeni işlemlerde kullanılamaz.

---

## 3. Sistem ve Kullanıcı Kategorileri

Kategori tablosundaki `KullaniciId` nullable'dır.

```text
KullaniciId = NULL
→ Sistem kategorisi

KullaniciId = kullanıcı ID'si
→ Kullanıcıya özel kategori
```

Bu sayede sistem hazır kategoriler sağlayabilirken kullanıcı da kendi kategorilerini oluşturabilir.

---

## 4. Gelir ve Gider Aynı Tabloda Tutulur

Gelir ve gider için ayrı tablolar oluşturmak yerine tek bir:

```text
FinansalIslemler
```

tablosu kullanılmaktadır.

İşlem türü:

```text
Tur = Gelir
```

veya:

```text
Tur = Gider
```

olarak belirtilir.

Bu yapı raporlamayı kolaylaştırır.

---

## 5. Tutar Negatif Saklanmaz

Gider:

```text
-500 TL
```

olarak tutulmaz.

Bunun yerine:

```text
Tur = Gider
Tutar = 500
```

şeklinde saklanır.

Gelir/gider ayrımı işlem türü üzerinden yapılır.

---

## 6. Kategori Zorunlu Değildir

`FinansalIslemler.KategoriId` nullable'dır.

Bunun nedeni özellikle OCR üzerinden oluşturulan finansal işlemlerde kategorinin ilk anda bilinmeyebilmesidir.

Kategori daha sonra kullanıcı tarafından atanabilir.

---

## 7. Her Finansal İşlemin Fişi Olmak Zorunda Değildir

`FisId` nullable'dır.

Çünkü:

```text
Maaş
Burs
Kira geliri
Elle girilen gider
```

gibi işlemlerin fişi bulunmayabilir.

---

## 8. Aynı Fişten İki Finansal İşlem Oluşturulmaz

`FinansalIslemler.FisId` üzerinde unique index bulunmaktadır.

Bu sayede:

```text
1 Fiş
  ↓
En fazla 1 Finansal İşlem
```

ilişkisi korunur.

---

# Fiş ve OCR Tasarımı

Fişler:

```text
Uploads/Fisler
```

klasöründe saklanır.

SQL Server içerisinde ise dosyanın yolu tutulur.

Dosya adlarının çakışmasını engellemek için sunucuda GUID tabanlı isimler kullanılır.

---

## Ham OCR Metni Saklanır

OCR sonucundan yalnızca tutar veya tarih alınmaz.

Ham OCR çıktısı da:

```text
HamOcrMetni
```

alanında saklanır.

Bunun amacı:

* OCR hatalarını incelemek
* Parser'ı geliştirebilmek
* Eski fişleri tekrar analiz etmek
* Kullanıcı düzeltmesiyle OCR sonucunu karşılaştırmaktır

---

## OCR Tutarı Bulamazsa

Tutar bulunamazsa finansal işlem oluşturulmaz.

Fiş:

```text
Bekliyor
```

durumunda kalır.

Kullanıcı daha sonra fişi düzelttiğinde işlem tekrar oluşturulabilir.

---

## OCR Tarihi Bulamazsa

Tutar bulunduğu halde tarih bulunamazsa geçici olarak fiş yükleme tarihi kullanılabilir.

Kullanıcı gerçek tarihi daha sonra düzeltebilir.

---

## Kullanıcı Doğrulaması

OCR sonucu kesin doğru kabul edilmez.

Kullanıcı:

```text
PUT /api/fisler/{id}/dogrula
```

üzerinden:

* İşletme
* Tarih
* Tutar
* Kategori

bilgilerini düzeltebilir.

Fiş güncellendiğinde ilişkili finansal işlem de senkronize edilir.

---

# Fiş Detayları

Fiş içerisindeki ürünler ayrı tabloda tutulmaktadır.

Örneğin:

```text
Süt       35 TL
Ekmek     15 TL
Peynir   120 TL
```

Her ürün için:

```text
UrunAdi
Miktar
BirimFiyat
ToplamTutar
HamSatirMetni
SiraNo
```

gibi bilgiler tutulabilir.

`HamSatirMetni`, OCR'ın orijinal çıktısını kaybetmemek için saklanmaktadır.

---

# Bütçeler

Bütçe genel veya kategori bazlı olabilir.

Genel bütçe:

```text
KategoriId = null
```

Örneğin:

```text
Eylül 2026
Genel bütçe: 15.000 TL
```

Kategori bazlı:

```text
Eylül 2026
Market: 4.000 TL
```

Aynı kullanıcı aynı ay için yalnızca bir genel bütçe oluşturabilir.

Aynı kullanıcı, kategori, yıl ve ay kombinasyonu için de birden fazla bütçe oluşturulamaz.

Veritabanı seviyesinde:

```text
Ay = 1-12
LimitTutari > 0
Yıl = 2000-2100
```

kontrolleri bulunmaktadır.

---

# Bildirimler

Bütçe kullanımına göre bildirimler oluşturulur.

Sadece:

```text
OkunduMu = true
```

tutmak yerine:

```text
OkunmaTarihi
```

tutulur.

Böylece bildirimin ne zaman okunduğu da bilinmektedir.

Bütçe silinirse geçmiş bildirimin de silinmemesi için:

```text
ON DELETE SET NULL
```

kullanılmıştır.

---

# Foreign Key Silme Stratejileri

| İlişki                     | Davranış               | Neden                                     |
| -------------------------- | ---------------------- | ----------------------------------------- |
| Kategori → Finansal İşlem  | Restrict / Soft Delete | Finansal geçmiş korunsun                  |
| İşletme → Finansal İşlem   | Restrict               | Eski işlemler bozulmasın                  |
| Kullanıcı → Finansal İşlem | Restrict               | Finansal kayıtlar korunabilsin            |
| Fiş → Finansal İşlem       | Restrict               | Oluşturulmuş işlem yanlışlıkla silinmesin |
| Fiş → Fiş Detayı           | Cascade                | Detay fiş olmadan anlamlı değildir        |
| Bütçe → Bildirim           | SetNull                | Geçmiş bildirim korunsun                  |

---

# Proje Klasör Yapısı

```text
GelirGiderTakip.Api
│
├── Controllers
├── Data
├── DTOs
├── Enums
├── Migrations
├── Models
├── Services
├── tessdata
├── Uploads
│   └── Fisler
├── Program.cs
├── appsettings.json
└── GelirGiderTakip.Api.csproj
```

---

# Bilinen Eksikler ve Geliştirilmesi Gereken Özellikler

## OCR Doğruluğu

Projenin en önemli geliştirme alanıdır.

Her fiş aynı formatta değildir.

Bir fişte:

```text
GENEL TOPLAM 450,00
```

yazarken başka bir fişte:

```text
TOPLAM: 450,00
```

veya:

```text
ÖDENECEK TUTAR
450,00 TL
```

şeklinde olabilir.

Mevcut sistem Regex tabanlı belirli kalıplara dayandığı için bütün fişleri doğru okuyamamaktadır.

---

## İşletme Tespiti

Şu anda ilk anlamlı OCR satırı işletme adı olarak kabul edilmektedir.

Bu nedenle bazı fişlerde işletme adı yanlış bulunabilir.

İleride:

* Fuzzy matching
* Bilinen işletmelerle eşleştirme
* Vergi numarası analizi

gibi yöntemler eklenebilir.

---

## Ürün Ayrıştırma

Fişlerde ürünler farklı biçimlerde yazılabilir.

```text
Süt 35,00
```

veya:

```text
2 X 17,50
SÜT
35,00
```

gibi formatlar bulunabilir.

Mevcut parser tüm formatları desteklememektedir.

---

## Görüntü Ön İşleme

OCR öncesinde:

* Deskew
* Contrast enhancement
* Threshold
* Noise reduction
* Perspective correction

gibi görüntü işleme adımları eklenebilir.

---

## OCR Güven Skoru

OCR sonucuna confidence değeri eklenebilir.

Düşük güvenilirliğe sahip sonuçlar otomatik finansal işleme dönüştürülmek yerine kullanıcı doğrulamasına gönderilebilir.

---

## Kod Mimarisi

OCR ve parser işlemlerinin bir kısmı şu anda `FislerController` içerisindedir.

İleride:

```text
FislerController
      ↓
FisService
      ↓
OcrService
      ↓
FisParserService
```

şeklinde ayrılması daha uygun olacaktır.

---

## Otomatik Testler

Projeye:

* Unit Test
* Integration Test
* Authentication / Authorization testleri
* OCR parser testleri
* Bütçe testleri

eklenmelidir.

---

## Global Hata Yönetimi ve Loglama

Merkezi bir exception handler ve yapılandırılmış loglama sistemi eklenebilir.

Örneğin:

```text
Global Exception Handler
Serilog
```

kullanılabilir.

---

## Dosya Saklama

Fişler şu anda yerel dosya sisteminde tutulmaktadır.

Production ortamında:

* Azure Blob Storage
* AWS S3
* MinIO

gibi çözümler tercih edilebilir.

---

## Refresh Token

JWT sistemi ileride:

```text
Access Token
+
Refresh Token
```

yapısına dönüştürülebilir.

---

## Frontend

Proje şu anda yalnızca backend Web API'dir.

Henüz web veya mobil kullanıcı arayüzü bulunmamaktadır.

---

# Öncelikli Gelecek Geliştirmesi

Özellikle OCR tarafında hedeflenen yapı:

```text
Fiş Fotoğrafı
      ↓
Görüntü Ön İşleme
      ↓
OCR
      ↓
Gelişmiş Parser
      ↓
İşletme / Tarih / Tutar / Ürün
      ↓
Güven Skoru
      ↓
Kullanıcı Doğrulaması
      ↓
Finansal İşlem
```

şeklindedir.

---

# Sonuç

Bu proje kapsamında:

* ASP.NET Core Web API
* REST API
* Entity Framework Core
* SQL Server
* Migration
* DTO
* JWT
* Authorization
* Kullanıcı izolasyonu
* Soft Delete
* Foreign Key yönetimi
* Dosya yükleme
* OCR
* Bütçe
* Bildirim
* Finansal raporlama

gibi backend geliştirme konuları uygulamalı olarak kullanılmıştır.

Proje temel gelir-gider takip işlevlerini gerçekleştirmektedir; ancak özellikle **OCR doğruluğu, test altyapısı, kod mimarisi ve production hazırlığı** açısından geliştirilmeye açıktır.

---

# Repository

```text
https://github.com/masah734-lang/gelir-gider-takip-API
```

