using Dapper;
using HuixinMotors.DAL.Data;
using HuixinMotors.DAL.Interfaces;
using HuixinMotors.Shared.Models;

namespace HuixinMotors.DAL.Repositories;

public class ProveedorRepository : IProveedorRepository
{
    private readonly DbConnectionFactory _db;

    public ProveedorRepository(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<List<Proveedor>> GetAllAsync()
    {
        using var cn = _db.Create();
        var result = await cn.QueryAsync<Proveedor>(
            "SELECT * FROM Proveedores ORDER BY IdProveedor DESC");
        return result.AsList();
    }

    public async Task<Proveedor> GetByIdAsync(int id)
    {
        using var cn = _db.Create();
        return await cn.QueryFirstOrDefaultAsync<Proveedor>(
            "SELECT * FROM Proveedores WHERE IdProveedor = @Id", new { Id = id });
    }

    public async Task<int> InsertAsync(Proveedor proveedor)
    {
        using var cn = _db.Create();
        var sql = @"INSERT INTO Proveedores (Nombre, Nit, Telefono, Email, Direccion, Pais, Estado)
                     VALUES (@Nombre, @Nit, @Telefono, @Email, @Direccion, @Pais, @Estado)";
        return await cn.ExecuteAsync(sql, proveedor);
    }

    public async Task<int> UpdateAsync(Proveedor proveedor)
    {
        using var cn = _db.Create();
        var sql = @"UPDATE Proveedores SET Nombre = @Nombre, Nit = @Nit, Telefono = @Telefono,
                     Email = @Email, Direccion = @Direccion, Pais = @Pais, Estado = @Estado
                     WHERE IdProveedor = @IdProveedor";
        return await cn.ExecuteAsync(sql, proveedor);
    }

    public async Task<List<ComboItemDto>> GetComboAsync()
    {
        using var cn = _db.Create();
        var result = await cn.QueryAsync<ComboItemDto>(
            "SELECT IdProveedor AS Value, Nombre AS Text FROM Proveedores WHERE Estado = 1 ORDER BY Text");
        return result.AsList();
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var cn = _db.Create();
        return await cn.ExecuteAsync("DELETE FROM Proveedores WHERE IdProveedor = @Id", new { Id = id });
    }
}
