namespace Ecommerce.Domain;

public class UsuarioBd
{
    public int UsuarioID { get; set; }
    public decimal? PersonalId { get; set; }
    public string? UsuarioAlias { get; set; }
    public byte[]? UsuarioClave { get; set; }
    public DateTime? UsuarioFechaReg { get; set; }
    public string? UsuarioEstado { get; set; }
}
