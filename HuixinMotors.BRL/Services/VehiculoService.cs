using HuixinMotors.BRL.Interfaces;
using HuixinMotors.DAL.Interfaces;
using HuixinMotors.Shared.Models;

namespace HuixinMotors.BRL.Services;

public class VehiculoService : IVehiculoService
{
    private readonly IVehiculoRepository _repo;

    public VehiculoService(IVehiculoRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<Vehiculo>> GetAllAsync() => await _repo.GetAllAsync();
    public async Task<Vehiculo> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);
    public async Task<List<ComboItemDto>> GetDisponiblesComboAsync() => await _repo.GetDisponiblesComboAsync();

    public async Task<int> InsertAsync(Vehiculo vehiculo)
    {
        if (string.IsNullOrWhiteSpace(vehiculo.Marca))
            throw new BusinessRuleException("La marca es obligatoria.");
        if (string.IsNullOrWhiteSpace(vehiculo.Modelo))
            throw new BusinessRuleException("El modelo es obligatorio.");
        if (string.IsNullOrWhiteSpace(vehiculo.Vin))
            throw new BusinessRuleException("El VIN es obligatorio.");
        if (vehiculo.PrecioVenta <= 0)
            throw new BusinessRuleException("El precio de venta debe ser mayor a cero.");

        return await _repo.InsertAsync(vehiculo);
    }

    public async Task<int> UpdateAsync(Vehiculo vehiculo)
    {
        if (vehiculo.IdVehiculo == 0)
            throw new BusinessRuleException("Debe seleccionar un vehiculo para actualizar.");
        return await _repo.UpdateAsync(vehiculo);
    }

    public async Task<int> DeleteAsync(int id)
    {
        if (id == 0)
            throw new BusinessRuleException("Debe seleccionar un vehiculo para eliminar.");
        return await _repo.DeleteAsync(id);
    }
}
