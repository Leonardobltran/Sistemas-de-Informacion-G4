using HuixinMotors.Shared.Models;

namespace HuixinMotors.DAL.Interfaces;

public interface IClienteRepository
{
    Task<List<Cliente>> GetAllAsync();
    Task<Cliente> GetByIdAsync(int id);
    Task<Cliente> GetByCiAsync(string ci);
    Task<List<ComboItemDto>> GetComboAsync();
    Task<int> InsertAsync(Cliente cliente);
    Task<int> UpdateAsync(Cliente cliente);
    Task<int> DeleteAsync(int id);
}
