using HuixinMotors.BRL.Interfaces;
using HuixinMotors.DAL.Interfaces;
using HuixinMotors.Shared.Models;

namespace HuixinMotors.BRL.Services;

public class CategoriaService : ICategoriaService
{
    private readonly ICategoriaRepository _repo;

    public CategoriaService(ICategoriaRepository repo) => _repo = repo;

    public async Task<List<CategoriaVehiculo>> GetAllAsync() => await _repo.GetAllAsync();
    public async Task<CategoriaVehiculo> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);
    public async Task<List<ComboItemDto>> GetComboAsync() => await _repo.GetComboAsync();

    public async Task<int> InsertAsync(CategoriaVehiculo categoria)
    {
        if (string.IsNullOrWhiteSpace(categoria.Nombre))
            throw new BusinessRuleException("El nombre de la categoria es obligatorio.");
        return await _repo.InsertAsync(categoria);
    }

    public async Task<int> UpdateAsync(CategoriaVehiculo categoria)
    {
        if (categoria.IdCategoria == 0)
            throw new BusinessRuleException("Debe seleccionar una categoria para actualizar.");
        return await _repo.UpdateAsync(categoria);
    }

    public async Task<int> DeleteAsync(int id)
    {
        if (id == 0)
            throw new BusinessRuleException("Debe seleccionar una categoria para eliminar.");
        return await _repo.DeleteAsync(id);
    }
}
