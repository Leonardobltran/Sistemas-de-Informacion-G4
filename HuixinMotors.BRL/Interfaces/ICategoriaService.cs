using HuixinMotors.Shared.Models;

namespace HuixinMotors.BRL.Interfaces;

public interface ICategoriaService
{
    Task<List<CategoriaVehiculo>> GetAllAsync();
    Task<CategoriaVehiculo> GetByIdAsync(int id);
    Task<List<ComboItemDto>> GetComboAsync();
    Task<int> InsertAsync(CategoriaVehiculo categoria);
    Task<int> UpdateAsync(CategoriaVehiculo categoria);
    Task<int> DeleteAsync(int id);
}
