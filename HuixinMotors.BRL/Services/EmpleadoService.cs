using HuixinMotors.BRL.Interfaces;
using HuixinMotors.DAL.Interfaces;
using HuixinMotors.Shared.Models;

namespace HuixinMotors.BRL.Services;

public class EmpleadoService : IEmpleadoService
{
    private readonly IEmpleadoRepository _repo;

    public EmpleadoService(IEmpleadoRepository repo) => _repo = repo;

    public async Task<List<Empleado>> GetAllAsync() => await _repo.GetAllAsync();
    public async Task<Empleado> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);
    public async Task<List<ComboItemDto>> GetComboAsync() => await _repo.GetComboAsync();

    public async Task<Empleado> AuthenticateAsync(string usuario, string clave)
    {
        if (string.IsNullOrWhiteSpace(usuario))
            throw new BusinessRuleException("El usuario es obligatorio.");
        if (string.IsNullOrWhiteSpace(clave))
            throw new BusinessRuleException("La clave es obligatoria.");
        var emp = await _repo.AuthenticateAsync(usuario, clave);
        if (emp == null)
            throw new BusinessRuleException("Usuario o clave incorrectos.");
        return emp;
    }

    public async Task<int> InsertAsync(Empleado empleado)
    {
        if (string.IsNullOrWhiteSpace(empleado.Nombres))
            throw new BusinessRuleException("El nombre del empleado es obligatorio.");
        if (string.IsNullOrWhiteSpace(empleado.Usuario))
            throw new BusinessRuleException("El usuario es obligatorio.");
        return await _repo.InsertAsync(empleado);
    }

    public async Task<int> UpdateAsync(Empleado empleado)
    {
        if (empleado.IdEmpleado == 0)
            throw new BusinessRuleException("Debe seleccionar un empleado para actualizar.");
        return await _repo.UpdateAsync(empleado);
    }

    public async Task<int> DeleteAsync(int id)
    {
        if (id == 0)
            throw new BusinessRuleException("Debe seleccionar un empleado para eliminar.");
        return await _repo.DeleteAsync(id);
    }
}
