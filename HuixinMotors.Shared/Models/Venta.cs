namespace HuixinMotors.Shared.Models;

public class Venta
{
    public int IdVenta { get; set; }
    public int IdCliente { get; set; }
    public int IdEmpleado { get; set; }
    public int IdVehiculo { get; set; }
    public DateTime FechaVenta { get; set; }
    public string NumeroFactura { get; set; }
    public string MetodoPago { get; set; }
    public decimal PrecioFinal { get; set; }
    public decimal Descuento { get; set; }
    public decimal Impuesto { get; set; }
    public decimal Total { get; set; }
    public string Observacion { get; set; }
}
