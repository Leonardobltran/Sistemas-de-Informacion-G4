using HuixinMotors.Shared.Models;

namespace HuixinMotors.BRL.Interfaces;

public interface IProveedorService
{
    Task<List<Proveedor>> GetAllAsync();
    Task<Proveedor> GetByIdAsync(int id);
    Task<List<ComboItemDto>> GetComboAsync();
    Task<int> InsertAsync(Proveedor proveedor);
    Task<int> UpdateAsync(Proveedor proveedor);
    Task<int> DeleteAsync(int id);
}
