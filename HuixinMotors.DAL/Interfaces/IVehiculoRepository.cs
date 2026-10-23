using HuixinMotors.Shared.Models;

namespace HuixinMotors.DAL.Interfaces;

public interface IVehiculoRepository
{
    Task<List<Vehiculo>> GetAllAsync();
    Task<Vehiculo> GetByIdAsync(int id);
    Task<List<ComboItemDto>> GetDisponiblesComboAsync();
    Task<int> InsertAsync(Vehiculo vehiculo);
    Task<int> UpdateAsync(Vehiculo vehiculo);
    Task<int> DeleteAsync(int id);
    Task<int> UpdateEstadoAsync(int idVehiculo, string estado);
}
