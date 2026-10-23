using HuixinMotors.Shared.Models;

namespace HuixinMotors.BRL.Interfaces;

public interface IVehiculoService
{
    Task<List<Vehiculo>> GetAllAsync();
    Task<Vehiculo> GetByIdAsync(int id);
    Task<List<ComboItemDto>> GetDisponiblesComboAsync();
    Task<int> InsertAsync(Vehiculo vehiculo);
    Task<int> UpdateAsync(Vehiculo vehiculo);
    Task<int> DeleteAsync(int id);
}
