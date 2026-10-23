namespace HuixinMotors.Shared.Models;

public class Cliente
{
    public int IdCliente { get; set; }
    public string Nombres { get; set; }
    public string Apellidos { get; set; }
    public string Ci { get; set; }
    public string Telefono { get; set; }
    public string Email { get; set; }
    public string Direccion { get; set; }
    public DateTime FechaRegistro { get; set; }
    public bool Estado { get; set; }
}
