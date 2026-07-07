namespace DatabaseComparer.Models
{
    public class TableInfo
    {
        public string Name { get; set; } = string.Empty;
    }

    public class ColumnInfo
    {
        public string TableName { get; set; } = string.Empty;
        public string ColumnName { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public bool IsNullable { get; set; }
        public int? MaxLength { get; set; }
    }

    public class ViewInfo
    {
        public string Name { get; set; } = string.Empty;
    }

    public class ProcedureInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Definition { get; set; } = string.Empty;
    }

    public class FunctionInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Definition { get; set; } = string.Empty;
    }

    public class IndexInfo
    {
        public string TableName { get; set; } = string.Empty;
        public string IndexName { get; set; } = string.Empty;
    }

    public class PrimaryKeyInfo
    {
        public string TableName { get; set; } = string.Empty;
        public string ConstraintName { get; set; } = string.Empty;
    }

    public class ForeignKeyInfo
    {
        public string TableName { get; set; } = string.Empty;
        public string ConstraintName { get; set; } = string.Empty;
    }

    // Bir veritabanının tüm şema bilgisini tutan kapsayıcı sınıf
    public class DatabaseSchema
    {
        public List<TableInfo> Tables { get; set; } = new();
        public List<ColumnInfo> Columns { get; set; } = new();
        public List<ViewInfo> Views { get; set; } = new();
        public List<ProcedureInfo> Procedures { get; set; } = new();
        public List<FunctionInfo> Functions { get; set; } = new();
        public List<IndexInfo> Indexes { get; set; } = new();
        public List<PrimaryKeyInfo> PrimaryKeys { get; set; } = new();
        public List<ForeignKeyInfo> ForeignKeys { get; set; } = new();
    }

    // Tek bir farkı temsil eder (örn. "Categories tablosu sadece kaynakta var")
    public class FarkDetayi
    {
        public string Kategori { get; set; } = string.Empty; // "Tablo", "Kolon", "View" vb.
        public string Aciklama { get; set; } = string.Empty;
        public string Durum { get; set; } = string.Empty; // "SadeceKaynakta", "SadeceHedefte", "Farkli"
    }

    // Tüm karşılaştırma sonucunu taşır
    public class KarsilastirmaSonucu
    {
        public int ToplamTablo { get; set; }
        public int ToplamKolon { get; set; }
        public int ToplamView { get; set; }
        public int ToplamProsedur { get; set; }
        public int ToplamFonksiyon { get; set; }
        public int ToplamIndex { get; set; }
        public int ToplamForeignKey { get; set; }
        public int ToplamFarkliliklar => Farklar.Count;

        public List<FarkDetayi> Farklar { get; set; } = new();
    }
}