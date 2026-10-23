using HuixinMotors.Shared.Models;

namespace HuixinMotors.BRL.Interfaces;

public interface IVentaService
{
    Task<List<Venta>> GetAllAsync();
    Task<Venta> GetByIdAsync(int id);
    decimal CalcularTotal(decimal precio, decimal descuento, decimal impuesto);
    Task<int> RegistrarVenta(Venta venta);
    Task<int> EliminarVenta(int idVenta, int idVehiculo);
    Task<List<ReporteVentaDto>> GetReporteVentasAsync(DateTime desde, DateTime hasta);
    Task<List<InventarioVehiculoDto>> GetInventarioAsync();
    Task<List<VentaPorSucursalDto>> GetVentasPorSucursalAsync();
}
