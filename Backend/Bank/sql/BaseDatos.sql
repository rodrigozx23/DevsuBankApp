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

BEGIN TRY
    BEGIN TRAN;

    INSERT INTO dbo.Personas (Nombre, Genero, Edad, Identificacion, Direccion, Telefono)
    VALUES
      (N'Jose Lema',          N'M', 30, N'CLI-001', N'Otavalo s/n y principal', N'098254785'),
      (N'Marianela Montalvo', N'F', 28, N'CLI-002', N'Amazonas y NNUU',         N'097548965'),
      (N'Juan Osorio',        N'M', 35, N'CLI-003', N'13 junio y Equinoccial',  N'098874587');

    DECLARE @pJose  INT = (SELECT PersonaId FROM dbo.Personas WHERE Identificacion = N'CLI-001');
    DECLARE @pMari  INT = (SELECT PersonaId FROM dbo.Personas WHERE Identificacion = N'CLI-002');
    DECLARE @pJuan  INT = (SELECT PersonaId FROM dbo.Personas WHERE Identificacion = N'CLI-003');

    INSERT INTO dbo.Clientes (PersonaId, Contrasena, Estado)
    VALUES (@pJose, N'1234', 1),
           (@pMari, N'5678', 1),
           (@pJuan, N'1245', 1);

    DECLARE @cJose INT = (SELECT ClienteId FROM dbo.Clientes WHERE PersonaId = @pJose);
    DECLARE @cMari INT = (SELECT ClienteId FROM dbo.Clientes WHERE PersonaId = @pMari);    
    DECLARE @cJuan INT = (SELECT ClienteId FROM dbo.Clientes WHERE PersonaId = @pJuan);

    INSERT INTO dbo.Cuentas (NumeroCuenta, TipoCuenta, Saldo, Estado, ClienteId)
    VALUES (N'478758', N'Ahorros',   2000, 1, @cJose),
           (N'225487', N'Corriente',  100, 1, @cMari),
           (N'496825', N'Ahorros',    540, 1, @cMari),
           (N'495878', N'Ahorros',    0, 1, @cJuan);

    DECLARE @cta478758 INT = (SELECT CuentaId FROM dbo.Cuentas WHERE NumeroCuenta = N'478758');
    DECLARE @cta225487 INT = (SELECT CuentaId FROM dbo.Cuentas WHERE NumeroCuenta = N'225487');
    DECLARE @cta495878 INT = (SELECT CuentaId FROM dbo.Cuentas WHERE NumeroCuenta = N'495878');
    DECLARE @cta496825 INT = (SELECT CuentaId FROM dbo.Cuentas WHERE NumeroCuenta = N'496825');
   
    DECLARE @s478758 DECIMAL(18,2) = (SELECT Saldo FROM dbo.Cuentas WHERE CuentaId = @cta478758);
    DECLARE @s225487 DECIMAL(18,2) = (SELECT Saldo FROM dbo.Cuentas WHERE CuentaId = @cta225487);
    DECLARE @s495878 DECIMAL(18,2) = (SELECT Saldo FROM dbo.Cuentas WHERE CuentaId = @cta495878);
    DECLARE @s496825 DECIMAL(18,2) = (SELECT Saldo FROM dbo.Cuentas WHERE CuentaId = @cta496825);


    INSERT INTO dbo.Movimientos (CuentaId, Fecha, TipoMovimiento, Valor, Saldo)
    VALUES
      (@cta478758, '2022-02-05T10:00:00', N'Debito',  -575.00, @s478758 - 575.00),
      (@cta225487, '2022-02-10T10:00:00', N'Credito',  600.00, @s225487 + 600.00),
      (@cta495878, '2022-02-08T10:00:00', N'Debito',  150.00, @s495878 + 150.00),
      (@cta496825, '2022-02-10T10:00:00', N'Credito',  -540.00, @s496825 - 540.00);
      

    COMMIT TRAN;
    PRINT 'Esquema listo.';
END TRY
BEGIN CATCH
    PRINT 'Esquema NO listo.';
    IF @@TRANCOUNT > 0 ROLLBACK TRAN;
    DECLARE @msg NVARCHAR(4000) = ERROR_MESSAGE();
    RAISERROR('Error en BaseDatos.sql: %s', 16, 1, @msg);
END CATCH;
GO
