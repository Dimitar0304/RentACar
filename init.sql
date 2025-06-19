CREATE DATABASE RentACarDb;
GO

USE RentACarDb;
GO

CREATE LOGIN appuser WITH PASSWORD = 'appuserpass';
GO

CREATE USER appuser FOR LOGIN appuser;
GO

ALTER ROLE db_owner ADD MEMBER appuser;
GO