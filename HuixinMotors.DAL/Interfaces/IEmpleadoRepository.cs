using HuixinMotors.Shared.Models;

namespace HuixinMotors.DAL.Interfaces;

public interface IEmpleadoRepository
{
    Task<List<Empleado>> GetAllAsync();
    Task<Empleado> GetByIdAsync(int id);
    Task<List<ComboItemDto>> GetComboAsync();
    Task<Empleado> AuthenticateAsync(string usuario, string clave);
    Task<int> InsertAsync(Empleado empleado);
    Task<int> UpdateAsync(Empleado empleado);
    Task<int> DeleteAsync(int id);
}
