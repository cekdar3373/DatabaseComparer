USE Northwind_B
GO

ALTER PROCEDURE "Ten Most Expensive Products" AS
SET ROWCOUNT 5
SELECT Products.ProductName AS TenMostExpensiveProducts, Products.UnitPrice
FROM Products
ORDER BY Products.UnitPrice DESC
GO