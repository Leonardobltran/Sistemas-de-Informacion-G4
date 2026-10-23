using Dapper;
using HuixinMotors.DAL.Data;
using HuixinMotors.DAL.Interfaces;
using HuixinMotors.Shared.Models;

namespace HuixinMotors.DAL.Repositories;

public class VehiculoRepository : IVehiculoRepository
{
    private readonly DbConnectionFactory _db;

    public VehiculoRepository(DbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<List<Vehiculo>> GetAllAsync()
    {
        using var cn = _db.Create();
        var result = await cn.QueryAsync<Vehiculo>(
            "SELECT * FROM Vehiculos ORDER BY IdVehiculo DESC");
        return result.AsList();
    }

    public async Task<Vehiculo> GetByIdAsync(int id)
    {
        using var cn = _db.Create();
        return await cn.QueryFirstOrDefaultAsync<Vehiculo>(
            "SELECT * FROM Vehiculos WHERE IdVehiculo = @Id", new { Id = id });
    }

    public async Task<List<ComboItemDto>> GetDisponiblesComboAsync()
    {
        using var cn = _db.Create();
        var result = await cn.QueryAsync<ComboItemDto>(
            "SELECT IdVehiculo AS Value, Marca + ' ' + Modelo + ' ' + CAST(Anio AS VARCHAR) + ' - $' + CAST(PrecioVenta AS VARCHAR) AS Text FROM Vehiculos WHERE Estado = 'Disponible' ORDER BY IdVehiculo DESC");
        return result.AsList();
    }

    public async Task<int> InsertAsync(Vehiculo vehiculo)
    {
        using var cn = _db.Create();
        var sql = @"INSERT INTO Vehiculos (IdProveedor, IdCategoria, IdSucursal, Marca, Modelo, Anio, Color, Vin, Placa,
                     PrecioCompra, PrecioVenta, Estado, Kilometraje, TipoCombustible, Transmision, ImagenUrl)
                     VALUES (@IdProveedor, @IdCategoria, @IdSucursal, @Marca, @Modelo, @Anio, @Color, @Vin, @Placa,
                     @PrecioCompra, @PrecioVenta, @Estado, @Kilometraje, @TipoCombustible, @Transmision, @ImagenUrl)";
        return await cn.ExecuteAsync(sql, vehiculo);
    }

    public async Task<int> UpdateAsync(Vehiculo vehiculo)
    {
        using var cn = _db.Create();
        var sql = @"UPDATE Vehiculos SET IdProveedor = @IdProveedor, IdCategoria = @IdCategoria, IdSucursal = @IdSucursal,
                     Marca = @Marca, Modelo = @Modelo, Anio = @Anio, Color = @Color, Vin = @Vin, Placa = @Placa,
                     PrecioCompra = @PrecioCompra, PrecioVenta = @PrecioVenta, Estado = @Estado,
                     Kilometraje = @Kilometraje, TipoCombustible = @TipoCombustible, Transmision = @Transmision,
                     ImagenUrl = @ImagenUrl WHERE IdVehiculo = @IdVehiculo";
        return await cn.ExecuteAsync(sql, vehiculo);
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var cn = _db.Create();
        return await cn.ExecuteAsync("DELETE FROM Vehiculos WHERE IdVehiculo = @Id", new { Id = id });
    }

    public async Task<int> UpdateEstadoAsync(int idVehiculo, string estado)
    {
        using var cn = _db.Create();
        return await cn.ExecuteAsync("UPDATE Vehiculos SET Estado = @Estado WHERE IdVehiculo = @Id",
            new { Id = idVehiculo, Estado = estado });
    }
}
