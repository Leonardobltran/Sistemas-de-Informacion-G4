/*
  Base de datos para HuixinMotors
  Ejecutar en SQL Server Management Studio o Azure Data Studio.
  Luego ajustar HuixinMotors.PL/appsettings.json con tu servidor/usuario/clave.
*/

IF DB_ID('huixin_motors') IS NULL
BEGIN
    CREATE DATABASE huixin_motors;
END
GO

USE huixin_motors;
GO

IF OBJECT_ID('dbo.vw_VentasPorSucursal', 'V') IS NOT NULL DROP VIEW dbo.vw_VentasPorSucursal;
GO
IF OBJECT_ID('dbo.vw_ReporteVentas', 'V') IS NOT NULL DROP VIEW dbo.vw_ReporteVentas;
GO
IF OBJECT_ID('dbo.vw_InventarioVehiculos', 'V') IS NOT NULL DROP VIEW dbo.vw_InventarioVehiculos;
GO
IF OBJECT_ID('dbo.sp_DashboardResumen', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_DashboardResumen;
GO

IF OBJECT_ID('dbo.Ventas', 'U') IS NOT NULL DROP TABLE dbo.Ventas;
IF OBJECT_ID('dbo.Vehiculos', 'U') IS NOT NULL DROP TABLE dbo.Vehiculos;
IF OBJECT_ID('dbo.CategoriasVehiculo', 'U') IS NOT NULL DROP TABLE dbo.CategoriasVehiculo;
IF OBJECT_ID('dbo.Sucursales', 'U') IS NOT NULL DROP TABLE dbo.Sucursales;
IF OBJECT_ID('dbo.Proveedores', 'U') IS NOT NULL DROP TABLE dbo.Proveedores;
IF OBJECT_ID('dbo.Clientes', 'U') IS NOT NULL DROP TABLE dbo.Clientes;
IF OBJECT_ID('dbo.Empleados', 'U') IS NOT NULL DROP TABLE dbo.Empleados;
GO

CREATE TABLE dbo.Clientes (
    IdCliente INT IDENTITY(1,1) CONSTRAINT PK_Clientes PRIMARY KEY,
    Nombres NVARCHAR(100) NOT NULL,
    Apellidos NVARCHAR(100) NOT NULL,
    Ci NVARCHAR(30) NOT NULL,
    Telefono NVARCHAR(30) NULL,
    Email NVARCHAR(150) NULL,
    Direccion NVARCHAR(250) NULL,
    FechaRegistro DATETIME NOT NULL CONSTRAINT DF_Clientes_FechaRegistro DEFAULT GETDATE(),
    Estado BIT NOT NULL CONSTRAINT DF_Clientes_Estado DEFAULT 1,
    CONSTRAINT UQ_Clientes_Ci UNIQUE (Ci)
);
GO

CREATE TABLE dbo.Empleados (
    IdEmpleado INT IDENTITY(1,1) CONSTRAINT PK_Empleados PRIMARY KEY,
    Nombres NVARCHAR(100) NOT NULL,
    Apellidos NVARCHAR(100) NOT NULL,
    Cargo NVARCHAR(100) NULL,
    Telefono NVARCHAR(30) NULL,
    Email NVARCHAR(150) NULL,
    Usuario NVARCHAR(50) NOT NULL,
    Clave NVARCHAR(100) NOT NULL,
    Estado BIT NOT NULL CONSTRAINT DF_Empleados_Estado DEFAULT 1,
    CONSTRAINT UQ_Empleados_Usuario UNIQUE (Usuario)
);
GO

CREATE TABLE dbo.Proveedores (
    IdProveedor INT IDENTITY(1,1) CONSTRAINT PK_Proveedores PRIMARY KEY,
    Nombre NVARCHAR(150) NOT NULL,
    Nit NVARCHAR(50) NULL,
    Telefono NVARCHAR(30) NULL,
    Email NVARCHAR(150) NULL,
    Direccion NVARCHAR(250) NULL,
    Pais NVARCHAR(100) NULL,
    Estado BIT NOT NULL CONSTRAINT DF_Proveedores_Estado DEFAULT 1
);
GO

CREATE TABLE dbo.Sucursales (
    IdSucursal INT IDENTITY(1,1) CONSTRAINT PK_Sucursales PRIMARY KEY,
    Nombre NVARCHAR(150) NOT NULL,
    Ciudad NVARCHAR(100) NULL,
    Direccion NVARCHAR(250) NULL,
    Latitud DECIMAL(10,7) NULL,
    Longitud DECIMAL(10,7) NULL,
    Estado BIT NOT NULL CONSTRAINT DF_Sucursales_Estado DEFAULT 1
);
GO

CREATE TABLE dbo.CategoriasVehiculo (
    IdCategoria INT IDENTITY(1,1) CONSTRAINT PK_CategoriasVehiculo PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(250) NULL,
    Estado BIT NOT NULL CONSTRAINT DF_CategoriasVehiculo_Estado DEFAULT 1
);
GO

CREATE TABLE dbo.Vehiculos (
    IdVehiculo INT IDENTITY(1,1) CONSTRAINT PK_Vehiculos PRIMARY KEY,
    IdProveedor INT NOT NULL,
    IdCategoria INT NOT NULL,
    IdSucursal INT NOT NULL,
    Marca NVARCHAR(80) NOT NULL,
    Modelo NVARCHAR(80) NOT NULL,
    Anio INT NOT NULL,
    Color NVARCHAR(50) NULL,
    Vin NVARCHAR(80) NOT NULL,
    Placa NVARCHAR(30) NULL,
    PrecioCompra DECIMAL(18,2) NOT NULL CONSTRAINT DF_Vehiculos_PrecioCompra DEFAULT 0,
    PrecioVenta DECIMAL(18,2) NOT NULL CONSTRAINT DF_Vehiculos_PrecioVenta DEFAULT 0,
    Estado NVARCHAR(30) NOT NULL CONSTRAINT DF_Vehiculos_Estado DEFAULT 'Disponible',
    Kilometraje INT NOT NULL CONSTRAINT DF_Vehiculos_Kilometraje DEFAULT 0,
    TipoCombustible NVARCHAR(50) NULL,
    Transmision NVARCHAR(50) NULL,
    FechaIngreso DATETIME NOT NULL CONSTRAINT DF_Vehiculos_FechaIngreso DEFAULT GETDATE(),
    ImagenUrl NVARCHAR(500) NULL,
    CONSTRAINT UQ_Vehiculos_Vin UNIQUE (Vin),
    CONSTRAINT FK_Vehiculos_Proveedores FOREIGN KEY (IdProveedor) REFERENCES dbo.Proveedores(IdProveedor),
    CONSTRAINT FK_Vehiculos_Categorias FOREIGN KEY (IdCategoria) REFERENCES dbo.CategoriasVehiculo(IdCategoria),
    CONSTRAINT FK_Vehiculos_Sucursales FOREIGN KEY (IdSucursal) REFERENCES dbo.Sucursales(IdSucursal)
);
GO

CREATE TABLE dbo.Ventas (
    IdVenta INT IDENTITY(1,1) CONSTRAINT PK_Ventas PRIMARY KEY,
    IdCliente INT NOT NULL,
    IdEmpleado INT NOT NULL,
    IdVehiculo INT NOT NULL,
    FechaVenta DATETIME NOT NULL CONSTRAINT DF_Ventas_FechaVenta DEFAULT GETDATE(),
    NumeroFactura NVARCHAR(50) NOT NULL,
    MetodoPago NVARCHAR(50) NOT NULL,
    PrecioFinal DECIMAL(18,2) NOT NULL,
    Descuento DECIMAL(18,2) NOT NULL CONSTRAINT DF_Ventas_Descuento DEFAULT 0,
    Impuesto DECIMAL(18,2) NOT NULL CONSTRAINT DF_Ventas_Impuesto DEFAULT 0,
    Total DECIMAL(18,2) NOT NULL,
    Observacion NVARCHAR(500) NULL,
    CONSTRAINT UQ_Ventas_NumeroFactura UNIQUE (NumeroFactura),
    CONSTRAINT FK_Ventas_Clientes FOREIGN KEY (IdCliente) REFERENCES dbo.Clientes(IdCliente),
    CONSTRAINT FK_Ventas_Empleados FOREIGN KEY (IdEmpleado) REFERENCES dbo.Empleados(IdEmpleado),
    CONSTRAINT FK_Ventas_Vehiculos FOREIGN KEY (IdVehiculo) REFERENCES dbo.Vehiculos(IdVehiculo)
);
GO

CREATE VIEW dbo.vw_InventarioVehiculos AS
SELECT
    v.IdVehiculo,
    p.Nombre AS Proveedor,
    c.Nombre AS Categoria,
    s.Nombre AS Sucursal,
    s.Ciudad,
    v.Marca,
    v.Modelo,
    v.Anio,
    v.Color,
    v.Vin,
    v.Placa,
    v.PrecioCompra,
    v.PrecioVenta,
    v.Estado,
    v.Kilometraje,
    v.TipoCombustible,
    v.Transmision,
    v.FechaIngreso
FROM dbo.Vehiculos v
INNER JOIN dbo.Proveedores p ON p.IdProveedor = v.IdProveedor
INNER JOIN dbo.CategoriasVehiculo c ON c.IdCategoria = v.IdCategoria
INNER JOIN dbo.Sucursales s ON s.IdSucursal = v.IdSucursal;
GO

CREATE VIEW dbo.vw_ReporteVentas AS
SELECT
    ve.IdVenta,
    ve.FechaVenta,
    ve.NumeroFactura,
    cl.Nombres + ' ' + cl.Apellidos AS Cliente,
    em.Nombres + ' ' + em.Apellidos AS Empleado,
    vh.Marca,
    vh.Modelo,
    vh.Anio,
    vh.Color,
    su.Nombre AS Sucursal,
    su.Ciudad,
    ve.MetodoPago,
    ve.PrecioFinal,
    ve.Descuento,
    ve.Impuesto,
    ve.Total,
    ve.Observacion
FROM dbo.Ventas ve
INNER JOIN dbo.Clientes cl ON cl.IdCliente = ve.IdCliente
INNER JOIN dbo.Empleados em ON em.IdEmpleado = ve.IdEmpleado
INNER JOIN dbo.Vehiculos vh ON vh.IdVehiculo = ve.IdVehiculo
INNER JOIN dbo.Sucursales su ON su.IdSucursal = vh.IdSucursal;
GO

CREATE VIEW dbo.vw_VentasPorSucursal AS
SELECT
    s.IdSucursal,
    s.Nombre,
    s.Ciudad,
    s.Direccion,
    s.Latitud,
    s.Longitud,
    COUNT(v.IdVenta) AS CantidadVentas,
    ISNULL(SUM(v.Total), 0) AS TotalVendido
FROM dbo.Sucursales s
LEFT JOIN dbo.Vehiculos vh ON vh.IdSucursal = s.IdSucursal
LEFT JOIN dbo.Ventas v ON v.IdVehiculo = vh.IdVehiculo
GROUP BY s.IdSucursal, s.Nombre, s.Ciudad, s.Direccion, s.Latitud, s.Longitud;
GO

CREATE PROCEDURE dbo.sp_DashboardResumen
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        (SELECT COUNT(*) FROM dbo.Clientes WHERE Estado = 1) AS ClientesActivos,
        (SELECT COUNT(*) FROM dbo.Vehiculos WHERE Estado = 'Disponible') AS VehiculosDisponibles,
        (SELECT COUNT(*) FROM dbo.Vehiculos WHERE Estado = 'Vendido') AS VehiculosVendidos,
        (SELECT COUNT(*) FROM dbo.Ventas) AS TotalVentas,
        ISNULL((SELECT SUM(Total) FROM dbo.Ventas), 0) AS IngresosTotales,
        ISNULL((SELECT AVG(Total) FROM dbo.Ventas), 0) AS PromedioVenta;
END
GO

INSERT INTO dbo.Empleados (Nombres, Apellidos, Cargo, Telefono, Email, Usuario, Clave, Estado)
VALUES ('Administrador', 'Sistema', 'Administrador', NULL, NULL, 'admin', '123456', 1);
GO

INSERT INTO dbo.CategoriasVehiculo (Nombre, Descripcion, Estado)
VALUES ('Sedan', 'Vehiculo sedan', 1), ('SUV', 'Vehiculo deportivo utilitario', 1), ('Electrico', 'Vehiculo electrico', 1);
GO

INSERT INTO dbo.Proveedores (Nombre, Nit, Telefono, Email, Direccion, Pais, Estado)
VALUES ('Proveedor Inicial', '000', NULL, NULL, NULL, 'Bolivia', 1);
GO

INSERT INTO dbo.Sucursales (Nombre, Ciudad, Direccion, Latitud, Longitud, Estado)
VALUES
('Sucursal La Paz', 'La Paz', 'Centro', -16.5000000, -68.1500000, 1),
('Sucursal Santa Cruz', 'Santa Cruz', 'Centro', -17.7833000, -63.1821000, 1),
('Sucursal Cochabamba', 'Cochabamba', 'Centro', -17.3895000, -66.1568000, 1);
GO

INSERT INTO dbo.Vehiculos (IdProveedor, IdCategoria, IdSucursal, Marca, Modelo, Anio, Color, Vin, Placa, PrecioCompra, PrecioVenta, Estado, Kilometraje, TipoCombustible, Transmision, ImagenUrl)
VALUES
(1, 3, 1, 'Huixin', 'E-Compact', 2025, 'Blanco', 'VIN-HX-0001', 'HX001', 10000, 13500, 'Disponible', 0, 'Electrico', 'Automatica', NULL),
(1, 2, 2, 'Huixin', 'SUV Max', 2025, 'Negro', 'VIN-HX-0002', 'HX002', 16000, 21000, 'Disponible', 0, 'Hibrido', 'Automatica', NULL);
GO
