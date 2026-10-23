using HuixinMotors.Shared.Models;

namespace HuixinMotors.DAL.Interfaces;

public interface IDashboardRepository
{
    Task<DashboardResumenDto> GetResumenAsync();
    Task<List<InventarioVehiculoDto>> GetUltimosIngresosAsync(int top = 10);
}
