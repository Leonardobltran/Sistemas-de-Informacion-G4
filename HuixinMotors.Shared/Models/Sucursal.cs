namespace HuixinMotors.Shared.Models;

public class Sucursal
{
    public int IdSucursal { get; set; }
    public string Nombre { get; set; }
    public string Ciudad { get; set; }
    public string Direccion { get; set; }
    public decimal? Latitud { get; set; }
    public decimal? Longitud { get; set; }
    public bool Estado { get; set; }
}
