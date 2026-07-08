\# Database Comparer



İki SQL Server veritabanının şemalarını (tablo, kolon, view, stored procedure, function, index, primary key, foreign key) karşılaştıran ve farklılıkları kategori bazında raporlayan bir web uygulaması.



\## Teknolojiler

\- ASP.NET Core MVC (.NET 8)

\- Bootstrap

\- Microsoft.Data.SqlClient



\## Nasıl Çalıştırılır



1\. Projeyi Visual Studio'da açın.

2\. Gerekli NuGet paketlerinin yüklü olduğundan emin olun.

3\. F5 ile projeyi çalıştırın.

4\. Açılan sayfada kaynak ve hedef veritabanı bağlantı bilgilerini girin.

5\. "Bağlantıyı Test Et" ile bağlantıyı doğrulayın ve veritabanlarını seçin.

6\. "Karşılaştır" butonuna basarak sonuçları görüntüleyin.



\## Ekran Görüntüleri



\### Bağlantı Ekranı

!\[Bağlantı Ekranı](screenshots/01-baglanti-ekrani.png)



\### Bağlantı Testi Başarılı

!\[Kaynak Bağlantı Testi](screenshots/02-baglanti-kaynak-test.png)

!\[Hedef Bağlantı Testi](screenshots/03-baglanti-hedef-test.png)



\### Karşılaştırma Sonucu – Dashboard Özeti

!\[Dashboard Özet](screenshots/04-dashboard-ozet.png)



\### Kategori Bazlı Fark Listesi

!\[Fark Listesi 1](screenshots/05-kategori-fark-listesi-1.png)

!\[Fark Listesi 2](screenshots/06-kategori-fark-listesi-2.png)



\### Fark Bulunmama Durumu

!\[Fark Yok Mesajı](screenshots/07-fark-yok-mesaji.png)



\## Proje Yapısı



\- `Controllers/` – Uygulama mantığını yöneten MVC controller'ları

\- `Models/` – Bağlantı ve şema karşılaştırma verilerini temsil eden sınıflar

\- `Services/` – Veritabanı bağlantısı, şema okuma ve karşılaştırma servisleri

\- `Views/` – Bootstrap tabanlı kullanıcı arayüzü sayfaları

\- `scripts/` – Northwind\_A ve Northwind\_B veritabanları arasında test amaçlı farklar oluşturan SQL script'leri

\- `docs/` – Test senaryoları ve diğer proje dokümanları



\## Test Senaryoları



Uygulamanın doğruluğunu kanıtlayan detaylı test senaryoları için `docs/TestSenaryolari.md` dosyasına bakınız.

