using HuixinMotors.Shared.Models;

namespace HuixinMotors.BRL.Interfaces;

public interface ISucursalService
{
    Task<List<Sucursal>> GetAllAsync();
    Task<Sucursal> GetByIdAsync(int id);
    Task<List<ComboItemDto>> GetComboAsync();
    Task<int> InsertAsync(Sucursal sucursal);
    Task<int> UpdateAsync(Sucursal sucursal);
    Task<int> DeleteAsync(int id);
}
