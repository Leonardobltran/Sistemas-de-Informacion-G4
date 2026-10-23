using HuixinMotors.Shared.Models;

namespace HuixinMotors.BRL.Interfaces;

public interface IClienteService
{
    Task<List<Cliente>> GetAllAsync();
    Task<Cliente> GetByIdAsync(int id);
    Task<List<ComboItemDto>> GetComboAsync();
    Task<int> InsertAsync(Cliente cliente);
    Task<int> UpdateAsync(Cliente cliente);
    Task<int> DeleteAsync(int id);
}
