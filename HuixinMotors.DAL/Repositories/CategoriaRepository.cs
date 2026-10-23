using Dapper;
using HuixinMotors.DAL.Data;
using HuixinMotors.DAL.Interfaces;
using HuixinMotors.Shared.Models;

namespace HuixinMotors.DAL.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly DbConnectionFactory _db;

    public CategoriaRepository(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<List<CategoriaVehiculo>> GetAllAsync()
    {
        using var cn = _db.Create();
        var result = await cn.QueryAsync<CategoriaVehiculo>(
            "SELECT * FROM CategoriasVehiculo ORDER BY IdCategoria DESC");
        return result.AsList();
    }

    public async Task<CategoriaVehiculo> GetByIdAsync(int id)
    {
        using var cn = _db.Create();
        return await cn.QueryFirstOrDefaultAsync<CategoriaVehiculo>(
            "SELECT * FROM CategoriasVehiculo WHERE IdCategoria = @Id", new { Id = id });
    }

    public async Task<int> InsertAsync(CategoriaVehiculo categoria)
    {
        using var cn = _db.Create();
        var sql = @"INSERT INTO CategoriasVehiculo (Nombre, Descripcion, Estado)
                     VALUES (@Nombre, @Descripcion, @Estado)";
        return await cn.ExecuteAsync(sql, categoria);
    }

    public async Task<int> UpdateAsync(CategoriaVehiculo categoria)
    {
        using var cn = _db.Create();
        var sql = @"UPDATE CategoriasVehiculo SET Nombre = @Nombre, Descripcion = @Descripcion, Estado = @Estado
                     WHERE IdCategoria = @IdCategoria";
        return await cn.ExecuteAsync(sql, categoria);
    }

    public async Task<List<ComboItemDto>> GetComboAsync()
    {
        using var cn = _db.Create();
        var result = await cn.QueryAsync<ComboItemDto>(
            "SELECT IdCategoria AS Value, Nombre AS Text FROM CategoriasVehiculo WHERE Estado = 1 ORDER BY Text");
        return result.AsList();
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var cn = _db.Create();
        return await cn.ExecuteAsync("DELETE FROM CategoriasVehiculo WHERE IdCategoria = @Id", new { Id = id });
    }
}
