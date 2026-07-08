USE Northwind_B
GO

DROP INDEX CategoryName ON Categories
GO

CREATE INDEX IX_Products_UnitPrice ON Products(UnitPrice)
GO