using HuixinMotors.Shared.Models;

namespace HuixinMotors.DAL.Interfaces;

public interface IProveedorRepository
{
    Task<List<Proveedor>> GetAllAsync();
    Task<Proveedor> GetByIdAsync(int id);
    Task<List<ComboItemDto>> GetComboAsync();
    Task<int> InsertAsync(Proveedor proveedor);
    Task<int> UpdateAsync(Proveedor proveedor);
    Task<int> DeleteAsync(int id);
}
