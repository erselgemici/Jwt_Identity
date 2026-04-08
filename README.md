# 🎵 Bepop - AI-Powered Music Streaming Platform

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![ML.NET](https://img.shields.io/badge/ML.NET-Machine_Learning-FF9900?style=for-the-badge&logo=dotnet&logoColor=white)
![Entity Framework Core](https://img.shields.io/badge/EF_Core-Database-339933?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-Security-000000?style=for-the-badge&logo=json-web-tokens&logoColor=white)
![JavaScript](https://img.shields.io/badge/JavaScript-Frontend-F7DF1E?style=for-the-badge&logo=javascript&logoColor=black)

**Bepop**, .NET 8 üzerinde geliştirilmiş, JWT tabanlı ayrık mimariye (Backend/Frontend) sahip ve ML.NET ile kişiselleştirilmiş müzik önerileri sunan akıllı bir müzik dinleme platformudur.

## 🚀 Proje Hakkında

Geleneksel müzik platformlarından farklı olarak Bepop, statik müzik listeleri sunmak yerine kullanıcı davranışlarını öğrenen dinamik bir yapıya sahiptir. Proje, modern web geliştirme prensipleri olan **Separation of Concerns (Kavramların Ayrılığı)** ve **Stateless Authentication (Durumsuz Kimlik Doğrulama)** temelleri üzerine inşa edilmiştir.

### 🌟 Temel Özellikler

* **Ayrık Mimari & JWT Güvenliği:** Backend (REST API) ve Frontend (WebUI) tamamen birbirinden bağımsız çalışır. Tüm iletişim JWT (JSON Web Token) üzerinden sağlanır.
* **Paket Bazlı Yetkilendirme (Tier-Based Auth):** Şarkıların dinlenebilmesi için kullanıcının sahip olduğu paket (Free, Premium, Elite vb.) seviyesinin, şarkının içerik seviyesini karşılaması gerekir. Yetkisiz erişimlerde `403 Forbidden` dönülür ve UI tarafında SweetAlert ile kullanıcı yönlendirilir.
* **ML.NET ile Hibrit Öneri Motoru:** * Sistem, kullanıcıların dinleme geçmişini **Matrix Factorization** algoritması ile analiz ederek kişiselleştirilmiş öneriler sunar.
    * **Diversity Shield:** "Yankı Odası" etkisini kırmak için listeleme algoritması her sanatçıdan maksimum 2 şarkı alacak şekilde sınırlandırılmıştır.
    * **Gerçek Zamanlı Eğitim:** Kullanıcı yeni bir şarkı dinlediğinde model arka planda asenkron olarak yeniden eğitilir.
* **Kesintisiz Global Player:** AJAX ve Fetch API kullanılarak geliştirilen altyapı sayesinde kullanıcılar sayfa değiştirirken bile şarkı çalmaya devam eder.
* **Dinamik Keşfet Sayfası:** JavaScript ile geliştirilen, anında arama (Real-time Search) ve sayfalama (Pagination) özelliklerine sahip performanslı keşfet ekranı.
* **Deezer API Entegrasyonu:** Şarkıların 30 saniyelik önizlemeleri dinamik olarak Deezer API'sinden çekilir.

---

## 🛠️ Kullanılan Teknolojiler

**Backend (API & Business Logic)**
* C# / .NET 8.0
* ASP.NET Core Web API
* ML.NET (Machine Learning)
* Entity Framework Core (Code First)
* SQL Server
* ASP.NET Core Identity & JWT Bearer

**Frontend (Web UI)**
* HTML5 / CSS3 / Bootstrap
* JavaScript (ES6+), jQuery, AJAX
* SweetAlert2

---

## ⚙️ Mimari Tasarım

Proje **N-Tier (Çok Katmanlı)** mimari yaklaşımıyla geliştirilmiştir:

1.  **Entity Layer:** Veritabanı tablolarının (Song, Artist, Package, User vb.) POCO sınıfları.
2.  **Data Access Layer (DAL):** EF Core DbContext ve veritabanı konfigürasyonları.
3.  **Business Layer:** İş kuralları (Business Rules), ML.NET servisleri, DTO'lar (Data Transfer Objects) ve API çağrılarını yöneten servisler.
4.  **API Layer (Identity/Backend):** Sadece JSON veri üreten ve JWT doğrulaması yapan güvenli uç noktalar (Endpoints).
5.  **WebUI Layer (Frontend):** API'yi tüketen (consume eden) ve kullanıcı ile etkileşime giren istemci (Client) katmanı.

---

## 📸 Ekran Görüntüleri
<img width="947" height="473" alt="ss0" src="https://github.com/user-attachments/assets/19e186dd-4a38-4ba7-b7d7-39b5cfa77f71" />
<img width="944" height="472" alt="ss1" src="https://github.com/user-attachments/assets/1e08a297-c8cd-41f0-aef2-ea554ad5dbce" />
<img width="947" height="471" alt="ss2" src="https://github.com/user-attachments/assets/07f81f2f-695f-4b46-a511-9d6fc6c58789" />
<img width="945" height="473" alt="ss3" src="https://github.com/user-attachments/assets/73f2ba34-d162-41d3-8935-21e18758262d" />
<img width="944" height="472" alt="ss4" src="https://github.com/user-attachments/assets/945a9160-cbc2-423a-a53e-c7158ed36e95" />
<img width="946" height="472" alt="ss5" src="https://github.com/user-attachments/assets/4e804dff-480f-4fd5-882b-b0c6d669a95d" />
<img width="945" height="472" alt="ss6" src="https://github.com/user-attachments/assets/29f1baad-db32-46d0-ac14-ce33a2711d8b" />
<img width="953" height="470" alt="ss7" src="https://github.com/user-attachments/assets/226a35fd-576c-4cd7-8d9c-0d2aab05d1bf" />
<img width="952" height="472" alt="ss8" src="https://github.com/user-attachments/assets/6c557103-41de-4632-aa1b-73e4ae4024c7" />
<img width="944" height="471" alt="ss9" src="https://github.com/user-attachments/assets/ed906a4a-a7c7-4e3f-980d-3153673215a4" />
<img width="944" height="470" alt="ss10" src="https://github.com/user-attachments/assets/aac6f2c5-ab36-417f-9e48-ac67bf028c6a" />
<img width="952" height="470" alt="ss11" src="https://github.com/user-attachments/assets/0672f750-4bea-40f8-9772-dd55b5800ab2" />
<img width="946" height="471" alt="ss12" src="https://github.com/user-attachments/assets/734c20d1-840b-41d3-8210-5e3a31d517de" />

