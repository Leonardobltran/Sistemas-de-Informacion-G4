namespace HuixinMotors.Shared.Models;

public class InventarioVehiculoDto
{
    public int IdVehiculo { get; set; }
    public string Proveedor { get; set; }
    public string Categoria { get; set; }
    public string Sucursal { get; set; }
    public string Ciudad { get; set; }
    public string Marca { get; set; }
    public string Modelo { get; set; }
    public int Anio { get; set; }
    public string Color { get; set; }
    public string Vin { get; set; }
    public string Placa { get; set; }
    public decimal PrecioCompra { get; set; }
    public decimal PrecioVenta { get; set; }
    public string Estado { get; set; }
    public int Kilometraje { get; set; }
    public string TipoCombustible { get; set; }
    public string Transmision { get; set; }
    public DateTime FechaIngreso { get; set; }
}
