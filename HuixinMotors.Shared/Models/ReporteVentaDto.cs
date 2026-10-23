namespace HuixinMotors.Shared.Models;

public class ReporteVentaDto
{
    public int IdVenta { get; set; }
    public DateTime FechaVenta { get; set; }
    public string NumeroFactura { get; set; }
    public string Cliente { get; set; }
    public string Empleado { get; set; }
    public string Marca { get; set; }
    public string Modelo { get; set; }
    public int Anio { get; set; }
    public string Color { get; set; }
    public string Sucursal { get; set; }
    public string Ciudad { get; set; }
    public string MetodoPago { get; set; }
    public decimal PrecioFinal { get; set; }
    public decimal Descuento { get; set; }
    public decimal Impuesto { get; set; }
    public decimal Total { get; set; }
    public string Observacion { get; set; }
}
