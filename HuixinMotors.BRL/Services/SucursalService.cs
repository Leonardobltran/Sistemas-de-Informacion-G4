using HuixinMotors.BRL.Interfaces;
using HuixinMotors.DAL.Interfaces;
using HuixinMotors.Shared.Models;

namespace HuixinMotors.BRL.Services;

public class SucursalService : ISucursalService
{
    private readonly ISucursalRepository _repo;

    public SucursalService(ISucursalRepository repo) => _repo = repo;

    public async Task<List<Sucursal>> GetAllAsync() => await _repo.GetAllAsync();
    public async Task<Sucursal> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);
    public async Task<List<ComboItemDto>> GetComboAsync() => await _repo.GetComboAsync();

    public async Task<int> InsertAsync(Sucursal sucursal)
    {
        if (string.IsNullOrWhiteSpace(sucursal.Nombre))
            throw new BusinessRuleException("El nombre de la sucursal es obligatorio.");
        return await _repo.InsertAsync(sucursal);
    }

    public async Task<int> UpdateAsync(Sucursal sucursal)
    {
        if (sucursal.IdSucursal == 0)
            throw new BusinessRuleException("Debe seleccionar una sucursal para actualizar.");
        return await _repo.UpdateAsync(sucursal);
    }

    public async Task<int> DeleteAsync(int id)
    {
        if (id == 0)
            throw new BusinessRuleException("Debe seleccionar una sucursal para eliminar.");
        return await _repo.DeleteAsync(id);
    }
}
