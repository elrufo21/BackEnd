namespace Ecommerce.Domain;

public class UsuarioBd
{
    public int UsuarioID { get; set; }
    public int? PersonalId { get; set; }
    public string? Nombre { get; set; }
    public string? UsuarioAlias { get; set; }
    public string? UsuarioClave { get; set; }
    public string? Area { get; set; }
    public DateTime? UsuarioFechaReg { get; set; }
    public string? UsuarioEstado { get; set; }
}
