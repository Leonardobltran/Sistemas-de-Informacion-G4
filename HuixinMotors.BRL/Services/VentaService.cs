using HuixinMotors.BRL.Interfaces;
using HuixinMotors.DAL.Interfaces;
using HuixinMotors.Shared.Models;

namespace HuixinMotors.BRL.Services;

public class VentaService : IVentaService
{
    private readonly IVentaRepository _ventaRepo;
    private readonly IVehiculoRepository _vehiculoRepo;

    public VentaService(IVentaRepository ventaRepo, IVehiculoRepository vehiculoRepo)
    {
        _ventaRepo = ventaRepo;
        _vehiculoRepo = vehiculoRepo;
    }

    public async Task<List<Venta>> GetAllAsync() => await _ventaRepo.GetAllAsync();
    public async Task<Venta> GetByIdAsync(int id) => await _ventaRepo.GetByIdAsync(id);

    public decimal CalcularTotal(decimal precio, decimal descuento, decimal impuesto)
    {
        return precio - descuento + impuesto;
    }

    public async Task<int> RegistrarVenta(Venta venta)
    {
        if (string.IsNullOrWhiteSpace(venta.NumeroFactura))
            throw new BusinessRuleException("Debe escribir el numero de factura.");

        if (venta.IdVehiculo == 0)
            throw new BusinessRuleException("Debe seleccionar un vehiculo disponible.");

        var vehiculo = await _vehiculoRepo.GetByIdAsync(venta.IdVehiculo);
        if (vehiculo == null)
            throw new BusinessRuleException("El vehiculo seleccionado no existe.");
        if (vehiculo.Estado != "Disponible")
            throw new BusinessRuleException("El vehiculo no esta disponible para la venta.");

        if (venta.IdCliente <= 0 || venta.IdEmpleado <= 0)
            throw new BusinessRuleException("Debe seleccionar un cliente y un empleado.");
        if (venta.PrecioFinal <= 0 || venta.Descuento < 0 || venta.Impuesto < 0 || venta.Descuento > venta.PrecioFinal)
            throw new BusinessRuleException("Verifique el precio, descuento e impuesto.");
        venta.Total = CalcularTotal(venta.PrecioFinal, venta.Descuento, venta.Impuesto);

        return await _ventaRepo.InsertAsync(venta);
    }

    public async Task<int> EliminarVenta(int idVenta, int idVehiculo)
    {
        if (idVenta == 0)
            throw new BusinessRuleException("Debe seleccionar una venta.");

        // La eliminacion y la reposicion del vehiculo son atomicas en el repositorio.
        return await _ventaRepo.DeleteAsync(idVenta);
    }

    public async Task<List<ReporteVentaDto>> GetReporteVentasAsync(DateTime desde, DateTime hasta)
        => await _ventaRepo.GetReporteVentasAsync(desde, hasta);

    public async Task<List<InventarioVehiculoDto>> GetInventarioAsync()
        => await _ventaRepo.GetInventarioAsync();

    public async Task<List<VentaPorSucursalDto>> GetVentasPorSucursalAsync()
        => await _ventaRepo.GetVentasPorSucursalAsync();
}
