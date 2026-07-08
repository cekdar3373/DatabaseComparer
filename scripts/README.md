# Farklılaştırma Script'leri

Bu klasördeki SQL script'leri, Northwind_A (referans/değişmemiş) ve Northwind_B (bilinçli olarak farklılaştırılmış) veritabanları arasında test amaçlı farklar oluşturmak için Northwind_B üzerinde sırasıyla çalıştırılmıştır.

| Script | Kategori | Açıklama |
|---|---|---|
| 01_missing_table.sql | Tablo | CustomerCustomerDemo tablosu silindi |
| 02_column_diff.sql | Kolon | Customers tablosuna LoyaltyPoints kolonu eklendi |
| 03_datatype_diff.sql | Veri Tipi | Products.QuantityPerUnit nvarchar(20)'den nvarchar(50)'ye değiştirildi |
| 04_nullable_diff.sql | Nullable | Employees.Region NOT NULL yapıldı |
| 05_pk_diff.sql | Primary Key | Shippers tablosunun primary key'i kaldırıldı |
| 06_fk_diff.sql | Foreign Key | Orders.FK_Orders_Shippers foreign key'i kaldırıldı |
| 07_index_diff.sql | Index | Categories.CategoryName index'i silindi, Products.IX_Products_UnitPrice index'i eklendi |
| 08_view_diff.sql | View | "Current Product List" view'ı silindi |
| 09_procedure_diff.sql | Stored Procedure | "Ten Most Expensive Products" prosedürünün gövdesi değiştirildi (ROWCOUNT 10 → 5) |
## Ekran Görüntüleri

### Bağlantı Ekranı
![Bağlantı Ekranı](screenshots/01-baglanti-ekrani.png)

### Bağlantı Testi Başarılı
![Kaynak Bağlantı Testi](screenshots/02-baglanti-kaynak-test.png)
![Hedef Bağlantı Testi](screenshots/03-baglanti-hedef-test.png)

### Karşılaştırma Sonucu – Dashboard Özeti
![Dashboard Özet](screenshots/04-dashboard-ozet.png)

### Kategori Bazlı Fark Listesi
![Fark Listesi 1](screenshots/05-kategori-fark-listesi-1.png)
![Fark Listesi 2](screenshots/06-kategori-fark-listesi-2.png)

### Fark Bulunmama Durumu
![Fark Yok Mesajı](screenshots/07-fark-yok-mesaji.png)