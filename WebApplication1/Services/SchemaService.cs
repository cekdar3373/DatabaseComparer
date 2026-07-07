using Microsoft.Data.SqlClient;
using DatabaseComparer.Models;

namespace DatabaseComparer.Services
{
    public class SchemaService
    {
        public DatabaseSchema GetSchema(string connectionString)
        {
            var schema = new DatabaseSchema();

            using var connection = new SqlConnection(connectionString);
            connection.Open();

            schema.Tables = GetTables(connection);
            schema.Columns = GetColumns(connection);
            schema.Views = GetViews(connection);
            schema.Procedures = GetProcedures(connection);
            schema.Functions = GetFunctions(connection);
            schema.Indexes = GetIndexes(connection);
            schema.PrimaryKeys = GetPrimaryKeys(connection);
            schema.ForeignKeys = GetForeignKeys(connection);

            return schema;
        }

        private List<TableInfo> GetTables(SqlConnection connection)
        {
            var list = new List<TableInfo>();
            const string sql = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME";

            using var command = new SqlCommand(sql, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new TableInfo { Name = reader.GetString(0) });
            }
            return list;
        }

        private List<ColumnInfo> GetColumns(SqlConnection connection)
        {
            var list = new List<ColumnInfo>();
            const string sql = @"
                SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE, IS_NULLABLE, CHARACTER_MAXIMUM_LENGTH
                FROM INFORMATION_SCHEMA.COLUMNS
                ORDER BY TABLE_NAME, ORDINAL_POSITION";

            using var command = new SqlCommand(sql, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new ColumnInfo
                {
                    TableName = reader.GetString(0),
                    ColumnName = reader.GetString(1),
                    DataType = reader.GetString(2),
                    IsNullable = reader.GetString(3) == "YES",
                    MaxLength = reader.IsDBNull(4) ? null : reader.GetInt32(4)
                });
            }
            return list;
        }

        private List<ViewInfo> GetViews(SqlConnection connection)
        {
            var list = new List<ViewInfo>();
            const string sql = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS ORDER BY TABLE_NAME";

            using var command = new SqlCommand(sql, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new ViewInfo { Name = reader.GetString(0) });
            }
            return list;
        }

        private List<ProcedureInfo> GetProcedures(SqlConnection connection)
        {
            var list = new List<ProcedureInfo>();
            const string sql = @"
        SELECT p.name, OBJECT_DEFINITION(p.object_id)
        FROM sys.procedures p
        ORDER BY p.name";

            using var command = new SqlCommand(sql, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new ProcedureInfo
                {
                    Name = reader.GetString(0),
                    Definition = reader.IsDBNull(1) ? string.Empty : reader.GetString(1)
                });
            }
            return list;
        }

        private List<FunctionInfo> GetFunctions(SqlConnection connection)
        {
            var list = new List<FunctionInfo>();
            const string sql = @"
        SELECT o.name, OBJECT_DEFINITION(o.object_id)
        FROM sys.objects o
        WHERE o.type IN ('FN','IF','TF')
        ORDER BY o.name";

            using var command = new SqlCommand(sql, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new FunctionInfo
                {
                    Name = reader.GetString(0),
                    Definition = reader.IsDBNull(1) ? string.Empty : reader.GetString(1)
                });
            }
            return list;
        }
        private List<IndexInfo> GetIndexes(SqlConnection connection)
        {
            var list = new List<IndexInfo>();
            const string sql = @"
                SELECT t.name AS TableName, i.name AS IndexName
                FROM sys.indexes i
                INNER JOIN sys.tables t ON i.object_id = t.object_id
                WHERE i.name IS NOT NULL AND i.is_primary_key = 0 AND i.is_unique_constraint = 0
                ORDER BY t.name, i.name";

            using var command = new SqlCommand(sql, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new IndexInfo
                {
                    TableName = reader.GetString(0),
                    IndexName = reader.GetString(1)
                });
            }
            return list;
        }

        private List<PrimaryKeyInfo> GetPrimaryKeys(SqlConnection connection)
        {
            var list = new List<PrimaryKeyInfo>();
            const string sql = @"
                SELECT t.name AS TableName, kc.name AS ConstraintName
                FROM sys.key_constraints kc
                INNER JOIN sys.tables t ON kc.parent_object_id = t.object_id
                WHERE kc.type = 'PK'
                ORDER BY t.name";

            using var command = new SqlCommand(sql, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new PrimaryKeyInfo
                {
                    TableName = reader.GetString(0),
                    ConstraintName = reader.GetString(1)
                });
            }
            return list;
        }

        private List<ForeignKeyInfo> GetForeignKeys(SqlConnection connection)
        {
            var list = new List<ForeignKeyInfo>();
            const string sql = @"
                SELECT t.name AS TableName, fk.name AS ConstraintName
                FROM sys.foreign_keys fk
                INNER JOIN sys.tables t ON fk.parent_object_id = t.object_id
                ORDER BY t.name";

            using var command = new SqlCommand(sql, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new ForeignKeyInfo
                {
                    TableName = reader.GetString(0),
                    ConstraintName = reader.GetString(1)
                });
            }
            return list;
        }
    }
}