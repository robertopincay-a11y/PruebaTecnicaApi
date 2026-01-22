
-- Script de Base de Datos - Prueba Técnica 


USE master;
GO


IF DB_ID('BancoDb') IS NOT NULL
BEGIN
    ALTER DATABASE BancoDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE BancoDb;
END

    
CREATE DATABASE BancoDb;
GO

USE BancoDb;
GO

-- ==================================================
-- 1. CREACIÓN DE TABLAS
-- ==================================================

-- Tabla Persona
CREATE TABLE Persona (
    IdPersona INT IDENTITY(1,1) PRIMARY KEY,
    Nombres NVARCHAR(60) NOT NULL,
    Apellidos NVARCHAR(60) NOT NULL,
    Identificacion CHAR(10) NOT NULL UNIQUE,
    FechaNacimiento DATE,
    Eliminado BIT NOT NULL DEFAULT 0,

    
    CONSTRAINT CK_Persona_Identificacion_Length CHECK (LEN(Identificacion) = 10),
    CONSTRAINT CK_Persona_Identificacion_Numeric CHECK (Identificacion NOT LIKE '%[^0-9]%')
);
GO

-- Tabla Rol
CREATE TABLE Rol (
    IdRol INT IDENTITY(1,1) PRIMARY KEY,
    NombreRol NVARCHAR(50) NOT NULL,
    Eliminado BIT NOT NULL DEFAULT 0
);
GO

-- Tabla Usuario
CREATE TABLE Usuario (
    IdUsuario INT IDENTITY(1,1) PRIMARY KEY,
    IdPersona INT NOT NULL,
    UserName NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Mail NVARCHAR(120) NOT NULL UNIQUE,
    SessionActive BIT NOT NULL DEFAULT 0,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Activo', 
    IntentosFallidos INT NOT NULL DEFAULT 0,
    CreadoEn DATETIME2 DEFAULT GETDATE(),
    Eliminado BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_Usuario_Persona FOREIGN KEY (IdPersona) REFERENCES Persona(IdPersona),
    CONSTRAINT CK_UserName_Length CHECK (LEN(UserName) BETWEEN 8 AND 20)
);
GO

-- Tabla UsuarioRol 
CREATE TABLE UsuarioRol (
    IdUsuario INT NOT NULL,
    IdRol INT NOT NULL,
    PRIMARY KEY (IdUsuario, IdRol),
    FOREIGN KEY (IdUsuario) REFERENCES Usuario(IdUsuario),
    FOREIGN KEY (IdRol) REFERENCES Rol(IdRol)
);
GO

-- Tabla Session
CREATE TABLE [Session] (
    IdSession INT IDENTITY(1,1) PRIMARY KEY,
    IdUsuario INT NOT NULL,
    FechaIngreso DATETIME2 NOT NULL DEFAULT GETDATE(),
    FechaCierre DATETIME2 NULL,
    Activa BIT NOT NULL DEFAULT 1,

    FOREIGN KEY (IdUsuario) REFERENCES Usuario(IdUsuario)
);
GO


-- 2. FUNCIÓN PARA VALIDAR IDENTIFICACIÓN


CREATE FUNCTION dbo.ValidarIdentificacion(@identificacion CHAR(10))
RETURNS BIT
AS
BEGIN
    DECLARE @i INT = 1;
    DECLARE @repetidos INT = 1;
    DECLARE @valido BIT = 1;

    IF LEN(@identificacion) != 10 OR ISNUMERIC(@identificacion) = 0
        RETURN 0;

    WHILE @i < 10
    BEGIN
        IF SUBSTRING(@identificacion, @i, 1) = SUBSTRING(@identificacion, @i + 1, 1)
            SET @repetidos = @repetidos + 1;
        ELSE
            SET @repetidos = 1;

        IF @repetidos >= 4
        BEGIN
            SET @valido = 0;
            BREAK;
        END
        SET @i = @i + 1;
    END

    RETURN @valido;
END;
GO


-- 3. STORED PROCEDURE PARA REGISTRAR USUARIO

CREATE PROCEDURE sp_RegistrarUsuario
    @Nombres NVARCHAR(60),
    @Apellidos NVARCHAR(60),
    @Identificacion CHAR(10),
    @FechaNacimiento DATE,
    @UserName NVARCHAR(50),
    @Password NVARCHAR(255),
    @IdRol INT
AS
BEGIN
    SET NOCOUNT ON;

    
    IF dbo.ValidarIdentificacion(@Identificacion) = 0
    BEGIN
        RAISERROR('Identificación inválida: debe tener 10 dígitos y no contener 4 números repetidos consecutivos.', 16, 1);
        RETURN;
    END

    
    INSERT INTO Persona (Nombres, Apellidos, Identificacion, FechaNacimiento)
    VALUES (@Nombres, @Apellidos, @Identificacion, @FechaNacimiento);

    DECLARE @IdPersona INT = SCOPE_IDENTITY();

    
    DECLARE @PrimeraLetraNombre CHAR(1) = LOWER(LEFT(@Nombres, 1));
    DECLARE @ApellidoSinEspacios NVARCHAR(60) = REPLACE(LOWER(@Apellidos), ' ', '');
    DECLARE @ApellidoLimpio NVARCHAR(60) = '';
    DECLARE @i INT = 1;
    WHILE @i <= LEN(@ApellidoSinEspacios)
    BEGIN
        IF SUBSTRING(@ApellidoSinEspacios, @i, 1) LIKE '[a-z]'
            SET @ApellidoLimpio = @ApellidoLimpio + SUBSTRING(@ApellidoSinEspacios, @i, 1);
        SET @i = @i + 1;
    END

    DECLARE @BaseMail NVARCHAR(120) = @PrimeraLetraNombre + @ApellidoLimpio + '@mail.com';
    DECLARE @MailFinal NVARCHAR(120) = @BaseMail;
    DECLARE @Contador INT = 1;

    WHILE EXISTS (SELECT 1 FROM Usuario WHERE Mail = @MailFinal)
    BEGIN
        SET @MailFinal = @PrimeraLetraNombre + @ApellidoLimpio + CAST(@Contador AS NVARCHAR(10)) + '@mail.com';
        SET @Contador = @Contador + 1;
    END

    
    INSERT INTO Usuario (IdPersona, UserName, PasswordHash, Mail, Status)
    VALUES (@IdPersona, @UserName, @Password, @MailFinal, 'Activo');

    DECLARE @IdUsuario INT = SCOPE_IDENTITY();

    
    INSERT INTO UsuarioRol (IdUsuario, IdRol)
    VALUES (@IdUsuario, @IdRol);

    
    SELECT @IdUsuario AS IdUsuario, @MailFinal AS CorreoGenerado;
END;
GO

-- 4. DATOS DE PRUEBA

INSERT INTO Rol (NombreRol) VALUES ('Administrador'), ('Usuario');
GO

-- Usuario "root" (administrador)
-- Contraseña: Passw0rd!
-- Hash generado con BCrypt: $2a$11$DxL5GqkXKZyMvJzQWfRtOeF1mYpN3sL8uV7rT9oPqR4sU6vW8xYz.

DECLARE @IdPersona INT;
INSERT INTO Persona (Nombres, Apellidos, Identificacion, FechaNacimiento)
VALUES ('Admin', 'Root', '1206505023', '1985-01-01');
SET @IdPersona = SCOPE_IDENTITY();
INSERT INTO Usuario (IdPersona, UserName, PasswordHash, Mail, Status)
VALUES (
    @IdPersona,
    'AdminRoot123',
    '$2a$11$DxL5GqkXKZyMvJzQWfRtOeF1mYpN3sL8uV7rT9oPqR4sU6vW8xYz.',
    'adminroot@mail.com',
    'Activo'
);
DECLARE @IdUsuario INT = SCOPE_IDENTITY();
INSERT INTO UsuarioRol (IdUsuario, IdRol)
VALUES (@IdUsuario, 1);
GO



