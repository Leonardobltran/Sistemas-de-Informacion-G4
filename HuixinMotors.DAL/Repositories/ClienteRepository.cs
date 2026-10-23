using Dapper;
using HuixinMotors.DAL.Data;
using HuixinMotors.DAL.Interfaces;
using HuixinMotors.Shared.Models;

namespace HuixinMotors.DAL.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly DbConnectionFactory _db;

    public ClienteRepository(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<List<Cliente>> GetAllAsync()
    {
        using var cn = _db.Create();
        var result = await cn.QueryAsync<Cliente>(
            "SELECT * FROM Clientes ORDER BY IdCliente DESC");
        return result.AsList();
    }

    public async Task<Cliente> GetByIdAsync(int id)
    {
        using var cn = _db.Create();
        return await cn.QueryFirstOrDefaultAsync<Cliente>(
            "SELECT * FROM Clientes WHERE IdCliente = @Id", new { Id = id });
    }

    public async Task<Cliente> GetByCiAsync(string ci)
    {
        using var cn = _db.Create();
        return await cn.QueryFirstOrDefaultAsync<Cliente>(
            "SELECT * FROM Clientes WHERE Ci = @Ci", new { Ci = ci });
    }

    public async Task<List<ComboItemDto>> GetComboAsync()
    {
        using var cn = _db.Create();
        var result = await cn.QueryAsync<ComboItemDto>(
            "SELECT IdCliente AS Value, Nombres + ' ' + Apellidos AS Text FROM Clientes WHERE Estado = 1 ORDER BY Text");
        return result.AsList();
    }

    public async Task<int> InsertAsync(Cliente cliente)
    {
        using var cn = _db.Create();
        var sql = @"INSERT INTO Clientes (Nombres, Apellidos, Ci, Telefono, Email, Direccion, Estado)
                     VALUES (@Nombres, @Apellidos, @Ci, @Telefono, @Email, @Direccion, @Estado)";
        return await cn.ExecuteAsync(sql, cliente);
    }

    public async Task<int> UpdateAsync(Cliente cliente)
    {
        using var cn = _db.Create();
        var sql = @"UPDATE Clientes SET Nombres = @Nombres, Apellidos = @Apellidos, Ci = @Ci,
                     Telefono = @Telefono, Email = @Email, Direccion = @Direccion, Estado = @Estado
                     WHERE IdCliente = @IdCliente";
        return await cn.ExecuteAsync(sql, cliente);
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var cn = _db.Create();
        return await cn.ExecuteAsync("DELETE FROM Clientes WHERE IdCliente = @Id", new { Id = id });
    }
}
