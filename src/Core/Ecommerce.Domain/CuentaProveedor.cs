namespace Ecommerce.Domain;

public class CuentaProveedor
{
    public long CuentaId { get; set; }
    public long ProveedorId { get; set; }
    public string? Entidad { get; set; }
    public string? TipoCuenta { get; set; }
    public string? Moneda { get; set; }
    public string? NroCuenta { get; set; }
}
