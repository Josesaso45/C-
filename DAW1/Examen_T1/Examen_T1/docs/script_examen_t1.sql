-- =====================================================
-- EXAMEN T1 - Desarrollo de Servicios Web I
-- Script completo: BD, tablas, datos y procedures
-- =====================================================

IF DB_ID('BDVENTAS2025') IS NULL
    CREATE DATABASE BDVENTAS2025;
GO

USE BDVENTAS2025;
GO

-- =====================================================
-- TABLAS BASE
-- =====================================================

IF OBJECT_ID('dbo.Categorias', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Categorias
    (
        cod_cat INT IDENTITY(1,1) PRIMARY KEY,
        nom_cat VARCHAR(50) NOT NULL
    );
END;
GO

IF OBJECT_ID('dbo.Clientes', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Clientes
    (
        cod_cli CHAR(5) PRIMARY KEY,
        nom_cli VARCHAR(80) NOT NULL
    );
END;
GO

IF OBJECT_ID('dbo.Vendedor', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Vendedor
    (
        cod_ven CHAR(5) PRIMARY KEY,
        nom_ven VARCHAR(80) NOT NULL
    );
END;
GO

IF OBJECT_ID('dbo.Ventas_Cab', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Ventas_Cab
    (
        num_vta  CHAR(8)       PRIMARY KEY,
        fec_vta  DATE          NOT NULL,
        cod_cli  CHAR(5)       REFERENCES dbo.Clientes(cod_cli),
        cod_ven  CHAR(5)       REFERENCES dbo.Vendedor(cod_ven),
        tot_vta  DECIMAL(10,2) NOT NULL DEFAULT 0
    );
END;
GO

IF OBJECT_ID('dbo.Articulos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Articulos
    (
        cod_art CHAR(5)       PRIMARY KEY,
        nom_art VARCHAR(50)   NOT NULL,
        uni_med VARCHAR(25)   NOT NULL,
        pre_art DECIMAL(7,2),
        stk_art INT,
        cod_cat INT           REFERENCES dbo.Categorias(cod_cat)
    );
END;
GO

-- =====================================================
-- DATOS DE PRUEBA
-- =====================================================

IF NOT EXISTS (SELECT 1 FROM dbo.Categorias)
BEGIN
    INSERT INTO dbo.Categorias (nom_cat) VALUES
        ('Lacteos'),
        ('Bebidas'),
        ('Abarrotes'),
        ('Embutidos'),
        ('Limpieza');
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Clientes)
BEGIN
    INSERT INTO dbo.Clientes VALUES
        ('C0001', 'Supermercados El Precio'),
        ('C0002', 'Distribuidora Los Andes'),
        ('C0003', 'Tiendas Cruz Verde'),
        ('C0004', 'Minimarket El Sol'),
        ('C0005', 'Bodega La Esquina');
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Vendedor)
BEGIN
    INSERT INTO dbo.Vendedor VALUES
        ('V0001', 'Carlos Ramirez Torres'),
        ('V0002', 'Maria Lopez Sanchez'),
        ('V0003', 'Juan Perez Huamani');
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Ventas_Cab)
BEGIN
    INSERT INTO dbo.Ventas_Cab VALUES
        ('VT000001', '2025-01-05', 'C0001', 'V0001', 1500.00),
        ('VT000002', '2025-01-12', 'C0002', 'V0002', 3200.50),
        ('VT000003', '2025-02-08', 'C0003', 'V0001', 870.00),
        ('VT000004', '2025-02-20', 'C0001', 'V0003', 2100.75),
        ('VT000005', '2025-03-15', 'C0004', 'V0002', 460.00),
        ('VT000006', '2025-04-10', 'C0005', 'V0001', 990.00),
        ('VT000007', '2025-05-22', 'C0002', 'V0003', 1750.25),
        ('VT000008', '2025-06-30', 'C0003', 'V0002', 3400.00),
        ('VT000009', '2025-07-14', 'C0001', 'V0001', 580.50),
        ('VT000010', '2025-08-03', 'C0004', 'V0003', 2200.00),
        ('VT000011', '2025-09-18', 'C0005', 'V0002', 1120.00),
        ('VT000012', '2025-10-25', 'C0001', 'V0001', 4300.00),
        ('VT000013', '2025-11-07', 'C0002', 'V0003', 670.75),
        ('VT000014', '2025-12-19', 'C0003', 'V0001', 1980.00),
        ('VT000015', '2025-12-28', 'C0004', 'V0002', 2750.00),
        ('VT000016', '2024-03-10', 'C0001', 'V0001', 1400.00),
        ('VT000017', '2024-07-22', 'C0002', 'V0002', 2800.00),
        ('VT000018', '2024-11-15', 'C0005', 'V0003', 960.00);
END;
GO

-- =====================================================
-- PROCEDIMIENTOS ALMACENADOS
-- =====================================================

CREATE OR ALTER PROCEDURE dbo.usp_ventas_por_anio
    @year INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        vc.num_vta,
        vc.fec_vta,
        c.nom_cli,
        v.nom_ven,
        vc.tot_vta
    FROM dbo.Ventas_Cab vc
    INNER JOIN dbo.Clientes  c ON c.cod_cli = vc.cod_cli
    INNER JOIN dbo.Vendedor  v ON v.cod_ven = vc.cod_ven
    WHERE YEAR(vc.fec_vta) = @year
    ORDER BY vc.fec_vta DESC, vc.num_vta DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_listar_categorias
AS
BEGIN
    SET NOCOUNT ON;

    SELECT cod_cat, nom_cat
    FROM dbo.Categorias
    ORDER BY nom_cat;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_listar_articulos
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        a.cod_art,
        a.nom_art,
        a.uni_med,
        a.pre_art,
        a.stk_art,
        a.cod_cat,
        c.nom_cat
    FROM dbo.Articulos a
    INNER JOIN dbo.Categorias c ON c.cod_cat = a.cod_cat
    ORDER BY a.cod_art;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_agregar_articulo
    @cod_art CHAR(5),
    @nom_art VARCHAR(50),
    @uni_med VARCHAR(25),
    @pre_art DECIMAL(7, 2),
    @stk_art INT,
    @cod_cat INT,
    @mensaje VARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Articulos WHERE cod_art = @cod_art)
    BEGIN
        SET @mensaje = 'El codigo de articulo ya existe.';
        RETURN;
    END;

    INSERT INTO dbo.Articulos (cod_art, nom_art, uni_med, pre_art, stk_art, cod_cat)
    VALUES (@cod_art, @nom_art, @uni_med, @pre_art, @stk_art, @cod_cat);

    SET @mensaje = 'Articulo registrado correctamente.';
END;
GO
