namespace HuixinMotors.Shared.Models;

public class DashboardResumenDto
{
    public int ClientesActivos { get; set; }
    public int VehiculosDisponibles { get; set; }
    public int VehiculosVendidos { get; set; }
    public int TotalVentas { get; set; }
    public decimal IngresosTotales { get; set; }
    public decimal PromedioVenta { get; set; }
}
