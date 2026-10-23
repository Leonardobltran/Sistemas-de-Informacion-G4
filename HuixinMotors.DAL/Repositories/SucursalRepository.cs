using Dapper;
using HuixinMotors.DAL.Data;
using HuixinMotors.DAL.Interfaces;
using HuixinMotors.Shared.Models;

namespace HuixinMotors.DAL.Repositories;

public class SucursalRepository : ISucursalRepository
{
    private readonly DbConnectionFactory _db;

    public SucursalRepository(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<List<Sucursal>> GetAllAsync()
    {
        using var cn = _db.Create();
        var result = await cn.QueryAsync<Sucursal>(
            "SELECT * FROM Sucursales ORDER BY IdSucursal DESC");
        return result.AsList();
    }

    public async Task<Sucursal> GetByIdAsync(int id)
    {
        using var cn = _db.Create();
        return await cn.QueryFirstOrDefaultAsync<Sucursal>(
            "SELECT * FROM Sucursales WHERE IdSucursal = @Id", new { Id = id });
    }

    public async Task<int> InsertAsync(Sucursal sucursal)
    {
        using var cn = _db.Create();
        var sql = @"INSERT INTO Sucursales (Nombre, Ciudad, Direccion, Latitud, Longitud, Estado)
                     VALUES (@Nombre, @Ciudad, @Direccion, @Latitud, @Longitud, @Estado)";
        return await cn.ExecuteAsync(sql, sucursal);
    }

    public async Task<int> UpdateAsync(Sucursal sucursal)
    {
        using var cn = _db.Create();
        var sql = @"UPDATE Sucursales SET Nombre = @Nombre, Ciudad = @Ciudad, Direccion = @Direccion,
                     Latitud = @Latitud, Longitud = @Longitud, Estado = @Estado
                     WHERE IdSucursal = @IdSucursal";
        return await cn.ExecuteAsync(sql, sucursal);
    }

    public async Task<List<ComboItemDto>> GetComboAsync()
    {
        using var cn = _db.Create();
        var result = await cn.QueryAsync<ComboItemDto>(
            "SELECT IdSucursal AS Value, Nombre AS Text FROM Sucursales WHERE Estado = 1 ORDER BY Text");
        return result.AsList();
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var cn = _db.Create();
        return await cn.ExecuteAsync("DELETE FROM Sucursales WHERE IdSucursal = @Id", new { Id = id });
    }
}
