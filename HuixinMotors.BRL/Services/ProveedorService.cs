using HuixinMotors.BRL.Interfaces;
using HuixinMotors.DAL.Interfaces;
using HuixinMotors.Shared.Models;

namespace HuixinMotors.BRL.Services;

public class ProveedorService : IProveedorService
{
    private readonly IProveedorRepository _repo;

    public ProveedorService(IProveedorRepository repo) => _repo = repo;

    public async Task<List<Proveedor>> GetAllAsync() => await _repo.GetAllAsync();
    public async Task<Proveedor> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);
    public async Task<List<ComboItemDto>> GetComboAsync() => await _repo.GetComboAsync();

    public async Task<int> InsertAsync(Proveedor proveedor)
    {
        if (string.IsNullOrWhiteSpace(proveedor.Nombre))
            throw new BusinessRuleException("El nombre del proveedor es obligatorio.");
        return await _repo.InsertAsync(proveedor);
    }

    public async Task<int> UpdateAsync(Proveedor proveedor)
    {
        if (proveedor.IdProveedor == 0)
            throw new BusinessRuleException("Debe seleccionar un proveedor para actualizar.");
        return await _repo.UpdateAsync(proveedor);
    }

    public async Task<int> DeleteAsync(int id)
    {
        if (id == 0)
            throw new BusinessRuleException("Debe seleccionar un proveedor para eliminar.");
        return await _repo.DeleteAsync(id);
    }
}
