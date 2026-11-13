# HaliSaha – Halı Saha Rezervasyon Sistemi

![CI](https://github.com/Sercansoy/HaliSaha/actions/workflows/ci.yml/badge.svg)

Oyuncuların halı sahaların boş saatlerini görüp rezervasyon yapabildiği, saha sahiplerinin ise kendi panellerinden saatleri, fiyatları ve rezervasyonları yönetebildiği bir web uygulaması.

> 🚧 Geliştirme aşamasında.

## Teknolojiler

- **Backend:** ASP.NET Core Web API (.NET 10)
- **Veritabanı:** SQL Server, Entity Framework Core
- **API dokümantasyonu:** OpenAPI + Scalar
- **CI:** GitHub Actions
- **Kimlik doğrulama:** JWT, rol bazlı yetkilendirme

## Proje Yapısı

```
src/
├── HaliSaha.Api             → REST API, controller'lar ve DTO'lar
├── HaliSaha.Core            → Domain modelleri (Facility, Pitch, User)
└── HaliSaha.Infrastructure  → EF Core, DbContext ve migration'lar
```

Bağımlılık yönü: `Api → Infrastructure → Core`. Core hiçbir projeye bağımlı değildir.

## API Uç Noktaları

| Metot | Adres | Açıklama |
|---|---|---|
| GET | `/api/facilities` | Onaylı tesisleri aktif sahalarıyla listeler |
| GET | `/api/facilities/{id}` | Tek bir onaylı tesisi getirir, bulunamazsa 404 döner |
| POST | `/api/auth/register` | Oyuncu olarak kayıt olur |
| POST | `/api/auth/login` | Giriş yapar, JWT token döner |
| GET | `/api/auth/me` | Giriş yapmış kullanıcının bilgileri 🔒 |
| GET | `/api/admin/facilities/pending` | Onay bekleyen tesisler 🔒 Admin |

Hata cevapları standart **ProblemDetails** (RFC 9457) formatındadır.

## Yerelde Çalıştırma

1. Repoyu klonla.
2. `src/HaliSaha.Api/appsettings.Development.json` dosyasındaki bağlantı dizesini kendi SQL Server'ına göre düzenle.
3. Api projesinde User Secrets'ı ayarla (HaliSaha.Api'ye sağ tık → **Manage User Secrets**):
   ```json
   {
     "Jwt:Key": "en-az-32-karakterlik-rastgele-bir-anahtar",
     "Admin:Email": "admin@halisaha.local",
     "Admin:Password": "guclu-bir-sifre"
   }
   ```
   Uygulama ilk açılışta bu bilgilerle bir admin hesabı oluşturur.
4. Package Manager Console'da Default project olarak `HaliSaha.Infrastructure` seç ve şunu çalıştır:
   ```
   Update-Database
   ```
5. Projeyi çalıştır ve `https://localhost:<port>/scalar` adresini aç.

## Yol Haritası

- [x] Katmanlı proje yapısı, EF Core ve ilk migration
- [x] Tesis listeleme ve detay uç noktaları
- [x] GitHub Actions ile otomatik derleme
- [x] Birim testleri
- [x] JWT ile kimlik doğrulama ve rol bazlı yetkilendirme (oyuncu / saha sahibi / admin)
- [ ] Rezervasyon ve aynı saatin çift kiralanmasını engelleyen eşzamanlılık kontrolü
- [ ] Docker ve Azure'a otomatik deploy
- [ ] React + TypeScript arayüz
- [ ] iyzico ile kapora ödemesi (sandbox)
- [ ] Azure Functions ile maç hatırlatma bildirimi
