USE Northwind_B
GO

UPDATE Employees SET Region = 'Unknown' WHERE Region IS NULL
GO

ALTER TABLE Employees ALTER COLUMN Region NVARCHAR(15) NOT NULL
GO