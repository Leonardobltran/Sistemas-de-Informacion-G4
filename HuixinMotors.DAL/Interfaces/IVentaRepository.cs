using HuixinMotors.Shared.Models;

namespace HuixinMotors.DAL.Interfaces;

public interface IVentaRepository
{
    Task<List<Venta>> GetAllAsync();
    Task<Venta> GetByIdAsync(int id);
    Task<int> InsertAsync(Venta venta);
    Task<int> DeleteAsync(int id);
    Task<List<ReporteVentaDto>> GetReporteVentasAsync(DateTime desde, DateTime hasta);
    Task<List<InventarioVehiculoDto>> GetInventarioAsync();
    Task<List<VentaPorSucursalDto>> GetVentasPorSucursalAsync();
}
