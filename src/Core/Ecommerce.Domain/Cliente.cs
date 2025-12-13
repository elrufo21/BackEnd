namespace Ecommerce.Domain;

public class Cliente
{
    public long ClienteId { get; set; }
    public string? ClienteRazon { get; set; }
    public string? ClienteRuc { get; set; }
    public string? ClienteDni { get; set; }
    public string? ClienteDireccion { get; set; }
    public string? ClienteTelefono { get; set; }
    public string? ClienteCorreo { get; set; }
    public string? ClienteEstado { get; set; }
    public string? ClienteDespacho { get; set; }
    public string? ClienteUsuario { get; set; }
    public DateTime? ClienteFecha { get; set; }
}
