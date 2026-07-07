using DatabaseComparer.Models;

namespace DatabaseComparer.Services
{
    public class CompareService
    {
        public KarsilastirmaSonucu Karsilastir(DatabaseSchema kaynak, DatabaseSchema hedef)
        {
            var sonuc = new KarsilastirmaSonucu
            {
                ToplamTablo = kaynak.Tables.Count,
                ToplamKolon = kaynak.Columns.Count,
                ToplamView = kaynak.Views.Count,
                ToplamProsedur = kaynak.Procedures.Count,
                ToplamFonksiyon = kaynak.Functions.Count,
                ToplamIndex = kaynak.Indexes.Count,
                ToplamForeignKey = kaynak.ForeignKeys.Count
            };

            KarsilastirTablolar(kaynak, hedef, sonuc);
            KarsilastirKolonlar(kaynak, hedef, sonuc);
            KarsilastirViewlar(kaynak, hedef, sonuc);
            KarsilastirProsedurler(kaynak, hedef, sonuc);
            KarsilastirFonksiyonlar(kaynak, hedef, sonuc);
            KarsilastirIndexler(kaynak, hedef, sonuc);
            KarsilastirPrimaryKeyler(kaynak, hedef, sonuc);
            KarsilastirForeignKeyler(kaynak, hedef, sonuc);

            return sonuc;
        }

        private void KarsilastirTablolar(DatabaseSchema kaynak, DatabaseSchema hedef, KarsilastirmaSonucu sonuc)
        {
            var kaynakSet = kaynak.Tables.Select(t => t.Name).ToHashSet();
            var hedefSet = hedef.Tables.Select(t => t.Name).ToHashSet();

            foreach (var isim in kaynakSet.Except(hedefSet))
                sonuc.Farklar.Add(new FarkDetayi { Kategori = "Tablo", Aciklama = $"\"{isim}\" tablosu sadece kaynakta var.", Durum = "SadeceKaynakta" });

            foreach (var isim in hedefSet.Except(kaynakSet))
                sonuc.Farklar.Add(new FarkDetayi { Kategori = "Tablo", Aciklama = $"\"{isim}\" tablosu sadece hedefte var.", Durum = "SadeceHedefte" });
        }

        private void KarsilastirKolonlar(DatabaseSchema kaynak, DatabaseSchema hedef, KarsilastirmaSonucu sonuc)
        {
            var kaynakDict = kaynak.Columns.ToDictionary(c => $"{c.TableName}.{c.ColumnName}");
            var hedefDict = hedef.Columns.ToDictionary(c => $"{c.TableName}.{c.ColumnName}");

            foreach (var key in kaynakDict.Keys.Except(hedefDict.Keys))
                sonuc.Farklar.Add(new FarkDetayi { Kategori = "Kolon", Aciklama = $"\"{key}\" kolonu sadece kaynakta var.", Durum = "SadeceKaynakta" });

            foreach (var key in hedefDict.Keys.Except(kaynakDict.Keys))
                sonuc.Farklar.Add(new FarkDetayi { Kategori = "Kolon", Aciklama = $"\"{key}\" kolonu sadece hedefte var.", Durum = "SadeceHedefte" });

            foreach (var key in kaynakDict.Keys.Intersect(hedefDict.Keys))
            {
                var k = kaynakDict[key];
                var h = hedefDict[key];

                if (k.DataType != h.DataType || k.MaxLength != h.MaxLength)
                {
                    sonuc.Farklar.Add(new FarkDetayi
                    {
                        Kategori = "Kolon",
                        Aciklama = $"\"{key}\" veri tipi farklı: Kaynak={k.DataType}({k.MaxLength}), Hedef={h.DataType}({h.MaxLength})",
                        Durum = "Farkli"
                    });
                }

                if (k.IsNullable != h.IsNullable)
                {
                    sonuc.Farklar.Add(new FarkDetayi
                    {
                        Kategori = "Kolon",
                        Aciklama = $"\"{key}\" nullable farklı: Kaynak={(k.IsNullable ? "NULL" : "NOT NULL")}, Hedef={(h.IsNullable ? "NULL" : "NOT NULL")}",
                        Durum = "Farkli"
                    });
                }
            }
        }

        private void KarsilastirViewlar(DatabaseSchema kaynak, DatabaseSchema hedef, KarsilastirmaSonucu sonuc)
        {
            var kaynakSet = kaynak.Views.Select(v => v.Name).ToHashSet();
            var hedefSet = hedef.Views.Select(v => v.Name).ToHashSet();

            foreach (var isim in kaynakSet.Except(hedefSet))
                sonuc.Farklar.Add(new FarkDetayi { Kategori = "View", Aciklama = $"\"{isim}\" view'ı sadece kaynakta var.", Durum = "SadeceKaynakta" });

            foreach (var isim in hedefSet.Except(kaynakSet))
                sonuc.Farklar.Add(new FarkDetayi { Kategori = "View", Aciklama = $"\"{isim}\" view'ı sadece hedefte var.", Durum = "SadeceHedefte" });
        }

        private void KarsilastirProsedurler(DatabaseSchema kaynak, DatabaseSchema hedef, KarsilastirmaSonucu sonuc)
        {
            var kaynakDict = kaynak.Procedures.ToDictionary(p => p.Name);
            var hedefDict = hedef.Procedures.ToDictionary(p => p.Name);

            foreach (var isim in kaynakDict.Keys.Except(hedefDict.Keys))
                sonuc.Farklar.Add(new FarkDetayi { Kategori = "Stored Procedure", Aciklama = $"\"{isim}\" prosedürü sadece kaynakta var.", Durum = "SadeceKaynakta" });

            foreach (var isim in hedefDict.Keys.Except(kaynakDict.Keys))
                sonuc.Farklar.Add(new FarkDetayi { Kategori = "Stored Procedure", Aciklama = $"\"{isim}\" prosedürü sadece hedefte var.", Durum = "SadeceHedefte" });

            foreach (var isim in kaynakDict.Keys.Intersect(hedefDict.Keys))
            {
                var kaynakTanim = Normallestir(kaynakDict[isim].Definition);
                var hedefTanim = Normallestir(hedefDict[isim].Definition);

                if (kaynakTanim != hedefTanim)
                {
                    sonuc.Farklar.Add(new FarkDetayi
                    {
                        Kategori = "Stored Procedure",
                        Aciklama = $"\"{isim}\" prosedürünün gövdesi (içeriği) farklı.",
                        Durum = "Farkli"
                    });
                }
            }
        }

        private void KarsilastirFonksiyonlar(DatabaseSchema kaynak, DatabaseSchema hedef, KarsilastirmaSonucu sonuc)
        {
            var kaynakDict = kaynak.Functions.ToDictionary(f => f.Name);
            var hedefDict = hedef.Functions.ToDictionary(f => f.Name);

            foreach (var isim in kaynakDict.Keys.Except(hedefDict.Keys))
                sonuc.Farklar.Add(new FarkDetayi { Kategori = "Function", Aciklama = $"\"{isim}\" fonksiyonu sadece kaynakta var.", Durum = "SadeceKaynakta" });

            foreach (var isim in hedefDict.Keys.Except(kaynakDict.Keys))
                sonuc.Farklar.Add(new FarkDetayi { Kategori = "Function", Aciklama = $"\"{isim}\" fonksiyonu sadece hedefte var.", Durum = "SadeceHedefte" });

            foreach (var isim in kaynakDict.Keys.Intersect(hedefDict.Keys))
            {
                var kaynakTanim = Normallestir(kaynakDict[isim].Definition);
                var hedefTanim = Normallestir(hedefDict[isim].Definition);

                if (kaynakTanim != hedefTanim)
                {
                    sonuc.Farklar.Add(new FarkDetayi
                    {
                        Kategori = "Function",
                        Aciklama = $"\"{isim}\" fonksiyonunun gövdesi (içeriği) farklı.",
                        Durum = "Farkli"
                    });
                }
            }
        }
        private string Normallestir(string tanim)
        {
            return string.Join(" ", tanim.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
        }

        private void KarsilastirIndexler(DatabaseSchema kaynak, DatabaseSchema hedef, KarsilastirmaSonucu sonuc)
        {
            var kaynakSet = kaynak.Indexes.Select(i => $"{i.TableName}.{i.IndexName}").ToHashSet();
            var hedefSet = hedef.Indexes.Select(i => $"{i.TableName}.{i.IndexName}").ToHashSet();

            foreach (var isim in kaynakSet.Except(hedefSet))
                sonuc.Farklar.Add(new FarkDetayi { Kategori = "Index", Aciklama = $"\"{isim}\" index'i sadece kaynakta var.", Durum = "SadeceKaynakta" });

            foreach (var isim in hedefSet.Except(kaynakSet))
                sonuc.Farklar.Add(new FarkDetayi { Kategori = "Index", Aciklama = $"\"{isim}\" index'i sadece hedefte var.", Durum = "SadeceHedefte" });
        }

        private void KarsilastirPrimaryKeyler(DatabaseSchema kaynak, DatabaseSchema hedef, KarsilastirmaSonucu sonuc)
        {
            var kaynakSet = kaynak.PrimaryKeys.Select(p => p.TableName).ToHashSet();
            var hedefSet = hedef.PrimaryKeys.Select(p => p.TableName).ToHashSet();

            foreach (var isim in kaynakSet.Except(hedefSet))
                sonuc.Farklar.Add(new FarkDetayi { Kategori = "Primary Key", Aciklama = $"\"{isim}\" tablosunun primary key'i sadece kaynakta var.", Durum = "SadeceKaynakta" });

            foreach (var isim in hedefSet.Except(kaynakSet))
                sonuc.Farklar.Add(new FarkDetayi { Kategori = "Primary Key", Aciklama = $"\"{isim}\" tablosunun primary key'i sadece hedefte var.", Durum = "SadeceHedefte" });
        }

        private void KarsilastirForeignKeyler(DatabaseSchema kaynak, DatabaseSchema hedef, KarsilastirmaSonucu sonuc)
        {
            var kaynakSet = kaynak.ForeignKeys.Select(f => $"{f.TableName}.{f.ConstraintName}").ToHashSet();
            var hedefSet = hedef.ForeignKeys.Select(f => $"{f.TableName}.{f.ConstraintName}").ToHashSet();

            foreach (var isim in kaynakSet.Except(hedefSet))
                sonuc.Farklar.Add(new FarkDetayi { Kategori = "Foreign Key", Aciklama = $"\"{isim}\" foreign key'i sadece kaynakta var.", Durum = "SadeceKaynakta" });

            foreach (var isim in hedefSet.Except(kaynakSet))
                sonuc.Farklar.Add(new FarkDetayi { Kategori = "Foreign Key", Aciklama = $"\"{isim}\" foreign key'i sadece hedefte var.", Durum = "SadeceHedefte" });
        }
    }
}
