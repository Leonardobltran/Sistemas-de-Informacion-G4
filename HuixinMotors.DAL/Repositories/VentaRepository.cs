using Dapper;
using HuixinMotors.DAL.Data;
using HuixinMotors.DAL.Interfaces;
using HuixinMotors.Shared.Models;

namespace HuixinMotors.DAL.Repositories;

public class VentaRepository : IVentaRepository
{
    private readonly DbConnectionFactory _db;

    public VentaRepository(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<List<Venta>> GetAllAsync()
    {
        using var cn = _db.Create();
        var result = await cn.QueryAsync<Venta>(
            "SELECT * FROM Ventas ORDER BY IdVenta DESC");
        return result.AsList();
    }

    public async Task<Venta> GetByIdAsync(int id)
    {
        using var cn = _db.Create();
        return await cn.QueryFirstOrDefaultAsync<Venta>(
            "SELECT * FROM Ventas WHERE IdVenta = @Id", new { Id = id });
    }

    public async Task<int> InsertAsync(Venta venta)
    {
        using var cn = _db.Create();
        await cn.OpenAsync();
        using var tx = cn.BeginTransaction();
        try
        {
            // La actualizacion condicional con bloqueo evita dos ventas simultaneas.
            int reservado = await cn.ExecuteAsync(@"
                UPDATE dbo.Vehiculos WITH (UPDLOCK, ROWLOCK)
                SET Estado = 'Vendido'
                WHERE IdVehiculo = @IdVehiculo AND Estado = 'Disponible'",
                new { venta.IdVehiculo }, tx);
            if (reservado != 1)
                throw new InvalidOperationException("El vehiculo no esta disponible para la venta.");

            int inserted = await cn.ExecuteAsync(@"
                INSERT INTO dbo.Ventas (IdCliente, IdEmpleado, IdVehiculo, NumeroFactura, MetodoPago,
                    PrecioFinal, Descuento, Impuesto, Total, Observacion)
                VALUES (@IdCliente, @IdEmpleado, @IdVehiculo, @NumeroFactura, @MetodoPago,
                    @PrecioFinal, @Descuento, @Impuesto, @Total, @Observacion)", venta, tx);
            tx.Commit();
            return inserted;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var cn = _db.Create();
        await cn.OpenAsync();
        using var tx = cn.BeginTransaction();
        try
        {
            var idVehiculo = await cn.QueryFirstOrDefaultAsync<int?>(
                "SELECT IdVehiculo FROM dbo.Ventas WITH (UPDLOCK, HOLDLOCK) WHERE IdVenta = @Id",
                new { Id = id }, tx);
            if (!idVehiculo.HasValue)
                return 0;
            int affected = await cn.ExecuteAsync(
                "DELETE FROM dbo.Ventas WHERE IdVenta = @Id", new { Id = id }, tx);
            // Solo devolver al inventario si no hay otra venta para el mismo vehiculo.
            await cn.ExecuteAsync(@"
                UPDATE dbo.Vehiculos SET Estado = 'Disponible'
                WHERE IdVehiculo = @IdVehiculo
                  AND NOT EXISTS (SELECT 1 FROM dbo.Ventas WHERE IdVehiculo = @IdVehiculo)",
                new { IdVehiculo = idVehiculo.Value }, tx);
            tx.Commit();
            return affected;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    public async Task<List<ReporteVentaDto>> GetReporteVentasAsync(DateTime desde, DateTime hasta)
    {
        using var cn = _db.Create();
        var result = await cn.QueryAsync<ReporteVentaDto>(
            "SELECT * FROM vw_ReporteVentas WHERE FechaVenta BETWEEN @Desde AND @Hasta ORDER BY FechaVenta DESC",
            new { Desde = desde, Hasta = hasta.Date.AddDays(1).AddSeconds(-1) });
        return result.AsList();
    }

    public async Task<List<InventarioVehiculoDto>> GetInventarioAsync()
    {
        using var cn = _db.Create();
        var result = await cn.QueryAsync<InventarioVehiculoDto>(
            "SELECT * FROM vw_InventarioVehiculos ORDER BY Estado, Marca, Modelo");
        return result.AsList();
    }

    public async Task<List<VentaPorSucursalDto>> GetVentasPorSucursalAsync()
    {
        using var cn = _db.Create();
        var result = await cn.QueryAsync<VentaPorSucursalDto>(
            "SELECT * FROM vw_VentasPorSucursal ORDER BY TotalVendido DESC");
        return result.AsList();
    }
}
