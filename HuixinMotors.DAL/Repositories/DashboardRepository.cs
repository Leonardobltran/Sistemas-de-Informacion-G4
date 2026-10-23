using Dapper;
using HuixinMotors.DAL.Data;
using HuixinMotors.DAL.Interfaces;
using HuixinMotors.Shared.Models;

namespace HuixinMotors.DAL.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly DbConnectionFactory _db;

    public DashboardRepository(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<DashboardResumenDto> GetResumenAsync()
    {
        using var cn = _db.Create();
        return await cn.QueryFirstOrDefaultAsync<DashboardResumenDto>("EXEC sp_DashboardResumen");
    }

    public async Task<List<InventarioVehiculoDto>> GetUltimosIngresosAsync(int top = 10)
    {
        using var cn = _db.Create();
        var result = await cn.QueryAsync<InventarioVehiculoDto>(
            $"SELECT TOP {top} Marca, Modelo, Anio, Color, PrecioVenta, Estado, FechaIngreso FROM vw_InventarioVehiculos ORDER BY FechaIngreso DESC");
        return result.AsList();
    }
}
