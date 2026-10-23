using HuixinMotors.Shared.Models;

namespace HuixinMotors.BRL.Interfaces;

public interface IDashboardService
{
    Task<DashboardResumenDto> GetResumenAsync();
    Task<List<InventarioVehiculoDto>> GetUltimosIngresosAsync(int top = 10);
}
