using HuixinMotors.BRL.Interfaces;
using HuixinMotors.DAL.Interfaces;
using HuixinMotors.Shared.Models;

namespace HuixinMotors.BRL.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repo;

    public ClienteService(IClienteRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<Cliente>> GetAllAsync() => await _repo.GetAllAsync();
    public async Task<Cliente> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);
    public async Task<List<ComboItemDto>> GetComboAsync() => await _repo.GetComboAsync();

    public async Task<int> InsertAsync(Cliente cliente)
    {
        if (string.IsNullOrWhiteSpace(cliente.Nombres))
            throw new BusinessRuleException("El nombre del cliente es obligatorio.");
        if (string.IsNullOrWhiteSpace(cliente.Apellidos))
            throw new BusinessRuleException("Los apellidos del cliente son obligatorios.");
        if (string.IsNullOrWhiteSpace(cliente.Ci))
            throw new BusinessRuleException("El CI/NIT del cliente es obligatorio.");

        var existente = await _repo.GetByCiAsync(cliente.Ci);
        if (existente != null)
            throw new BusinessRuleException("Ya existe un cliente con ese CI/NIT.");

        return await _repo.InsertAsync(cliente);
    }

    public async Task<int> UpdateAsync(Cliente cliente)
    {
        if (cliente.IdCliente == 0)
            throw new BusinessRuleException("Debe seleccionar un cliente para actualizar.");
        return await _repo.UpdateAsync(cliente);
    }

    public async Task<int> DeleteAsync(int id)
    {
        if (id == 0)
            throw new BusinessRuleException("Debe seleccionar un cliente para eliminar.");
        return await _repo.DeleteAsync(id);
    }
}
