namespace HuixinMotors.Shared.Models;

public class Vehiculo
{
    public int IdVehiculo { get; set; }
    public int IdProveedor { get; set; }
    public int IdCategoria { get; set; }
    public int IdSucursal { get; set; }
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
    public string ImagenUrl { get; set; }
}
