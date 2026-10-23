using HuixinMotors.BRL.Interfaces;
using HuixinMotors.DAL.Interfaces;
using HuixinMotors.Shared.Models;

namespace HuixinMotors.BRL.Services;

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _repo;

    public DashboardService(IDashboardRepository repo) => _repo = repo;

    public async Task<DashboardResumenDto> GetResumenAsync() => await _repo.GetResumenAsync();
    public async Task<List<InventarioVehiculoDto>> GetUltimosIngresosAsync(int top = 10)
        => await _repo.GetUltimosIngresosAsync(top);
}
