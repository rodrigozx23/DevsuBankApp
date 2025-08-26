IF DB_ID(N'BankDb') IS NULL
BEGIN
    PRINT 'Creando base de datos BankDb...';
    CREATE DATABASE BankDb;
END
GO

USE BankDb;
GO
SET NOCOUNT ON;
GO

IF OBJECT_ID(N'dbo.Personas', N'U') IS NULL
BEGIN
    PRINT 'Creando tabla dbo.Personas...';
    CREATE TABLE dbo.Personas
    (
        PersonaId      INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Personas PRIMARY KEY,
        Nombre         NVARCHAR(200)     NOT NULL,
        Genero         NCHAR(1)          NULL,
        Edad           INT               NULL,
        Identificacion NVARCHAR(50)      NOT NULL,
        Direccion      NVARCHAR(250)     NULL,
        Telefono       NVARCHAR(50)      NULL
    );    
    CREATE UNIQUE INDEX UX_Personas_Identificacion ON dbo.Personas(Identificacion);
END
GO

IF OBJECT_ID(N'dbo.Clientes', N'U') IS NULL
BEGIN
    PRINT 'Creando tabla dbo.Clientes...';
    CREATE TABLE dbo.Clientes
    (
        PersonaId  INT NOT NULL
            CONSTRAINT PK_Clientes PRIMARY KEY,
        ClienteId  INT IDENTITY(1,1) NOT NULL,
        Contrasena NVARCHAR(200) NOT NULL,
        Estado     BIT NOT NULL CONSTRAINT DF_Clientes_Estado DEFAULT (1),
        CONSTRAINT FK_Clientes_Personas
            FOREIGN KEY (PersonaId) REFERENCES dbo.Personas(PersonaId)
            ON DELETE CASCADE,
        CONSTRAINT UQ_Clientes_ClienteId UNIQUE (ClienteId)
    );
END
GO

IF OBJECT_ID(N'dbo.Cuentas', N'U') IS NULL
BEGIN
    PRINT 'Creando tabla dbo.Cuentas...';
    CREATE TABLE dbo.Cuentas
    (
        CuentaId      INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_Cuentas PRIMARY KEY,
        NumeroCuenta  NVARCHAR(50) NOT NULL,
        TipoCuenta    NVARCHAR(20) NOT NULL,
        Saldo         DECIMAL(18,2) NOT NULL
            CONSTRAINT DF_Cuentas_SaldoInicial DEFAULT (0),
        Estado        BIT NOT NULL
            CONSTRAINT DF_Cuentas_Estado DEFAULT (1),
        ClienteId     INT NOT NULL
    );

    ALTER TABLE dbo.Cuentas
      ADD CONSTRAINT FK_Cuentas_Clientes
          FOREIGN KEY (ClienteId)
          REFERENCES dbo.Clientes(ClienteId)
          ON DELETE CASCADE;

    CREATE UNIQUE INDEX UX_Cuentas_Numero ON dbo.Cuentas(NumeroCuenta);
    CREATE INDEX IX_Cuentas_ClienteId ON dbo.Cuentas(ClienteId);
END
GO

IF OBJECT_ID(N'dbo.Movimientos', N'U') IS NULL
BEGIN
    PRINT 'Creando tabla dbo.Movimientos...';
    CREATE TABLE dbo.Movimientos
    (
        MovimientoId   INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_Movimientos PRIMARY KEY,
        CuentaId       INT            NOT NULL,
        Fecha          DATETIME2(0)   NOT NULL,
        TipoMovimiento NVARCHAR(10)   NOT NULL,
        Valor          DECIMAL(18,2)  NOT NULL,
        Saldo          DECIMAL(18,2)  NOT NULL
            CONSTRAINT DF_Movimientos_Saldo DEFAULT(0)
    );

    ALTER TABLE dbo.Movimientos
      ADD CONSTRAINT FK_Movimientos_Cuentas
          FOREIGN KEY (CuentaId) REFERENCES dbo.Cuentas(CuentaId)
          ON DELETE CASCADE;

    CREATE INDEX IX_Movimientos_CuentaId_Fecha ON dbo.Movimientos(CuentaId, Fecha);

    ALTER TABLE dbo.Movimientos
      ADD CONSTRAINT CK_Movimientos_Tipo
          CHECK (TipoMovimiento IN (N'Debito', N'Credito'));

    ALTER TABLE dbo.Movimientos
      ADD CONSTRAINT CK_Movimientos_ValorNonZero
          CHECK (Valor <> 0);
END
GO
