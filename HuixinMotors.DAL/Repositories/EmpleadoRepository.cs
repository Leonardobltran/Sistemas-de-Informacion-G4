using Dapper;
using HuixinMotors.DAL.Security;
using HuixinMotors.DAL.Data;
using HuixinMotors.DAL.Interfaces;
using HuixinMotors.Shared.Models;

namespace HuixinMotors.DAL.Repositories;

public class EmpleadoRepository : IEmpleadoRepository
{
    private readonly DbConnectionFactory _db;

    public EmpleadoRepository(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<List<Empleado>> GetAllAsync()
    {
        using var cn = _db.Create();
        var result = await cn.QueryAsync<Empleado>(
            "SELECT * FROM Empleados ORDER BY IdEmpleado DESC");
        return result.AsList();
    }

    public async Task<Empleado> GetByIdAsync(int id)
    {
        using var cn = _db.Create();
        return await cn.QueryFirstOrDefaultAsync<Empleado>(
            "SELECT * FROM Empleados WHERE IdEmpleado = @Id", new { Id = id });
    }

    public async Task<List<ComboItemDto>> GetComboAsync()
    {
        using var cn = _db.Create();
        var result = await cn.QueryAsync<ComboItemDto>(
            "SELECT IdEmpleado AS Value, Nombres + ' ' + Apellidos AS Text FROM Empleados WHERE Estado = 1 ORDER BY Text");
        return result.AsList();
    }

    public async Task<int> InsertAsync(Empleado empleado)
    {
        using var cn = _db.Create();
        var sql = @"INSERT INTO Empleados (Nombres, Apellidos, Cargo, Telefono, Email, Usuario, Clave, Estado)
                     VALUES (@Nombres, @Apellidos, @Cargo, @Telefono, @Email, @Usuario, @Clave, @Estado)";
        empleado.Clave = PasswordHasher.Hash(empleado.Clave);
        return await cn.ExecuteAsync(sql, empleado);
    }

    public async Task<int> UpdateAsync(Empleado empleado)
    {
        using var cn = _db.Create();
        var sql = @"UPDATE Empleados SET Nombres = @Nombres, Apellidos = @Apellidos, Cargo = @Cargo,
                     Telefono = @Telefono, Email = @Email, Usuario = @Usuario, Clave = @Clave, Estado = @Estado
                     WHERE IdEmpleado = @IdEmpleado";
        var actual = await cn.QueryFirstOrDefaultAsync<string>(
            "SELECT Clave FROM Empleados WHERE IdEmpleado = @Id", new { Id = empleado.IdEmpleado });
        if (actual == null) throw new InvalidOperationException("Empleado no encontrado.");
        empleado.Clave = string.IsNullOrWhiteSpace(empleado.Clave) || empleado.Clave == actual
            ? actual : PasswordHasher.Hash(empleado.Clave);
        return await cn.ExecuteAsync(sql, empleado);
    }

    public async Task<Empleado> AuthenticateAsync(string usuario, string clave)
    {
        using var cn = _db.Create();
        var empleado = await cn.QueryFirstOrDefaultAsync<Empleado>(
            "SELECT * FROM Empleados WHERE Usuario = @Usuario AND Estado = 1",
            new { Usuario = usuario });
        if (empleado == null || !PasswordHasher.Verify(clave, empleado.Clave)) return null;
        if (!empleado.Clave.StartsWith("PBKDF2$", StringComparison.Ordinal))
        {
            var nuevoHash = PasswordHasher.Hash(clave);
            await cn.ExecuteAsync("UPDATE Empleados SET Clave = @Hash WHERE IdEmpleado = @Id AND Clave = @Anterior",
                new { Hash = nuevoHash, Id = empleado.IdEmpleado, Anterior = empleado.Clave });
            empleado.Clave = nuevoHash;
        }
        return empleado;
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var cn = _db.Create();
        return await cn.ExecuteAsync("DELETE FROM Empleados WHERE IdEmpleado = @Id", new { Id = id });
    }
}
