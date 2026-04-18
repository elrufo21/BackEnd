namespace Ecommerce.Domain;
public class AuthResponseA
{
   public string? Id { get; set; }
    public string? PersonalId { get; set; }
    public string? Area { get; set; }
    public string? Usuario { get; set; }
    public string? CompaniaId { get; set; }
    public string? RazonSocial { get; set; }
    public string? FechaVencimientoClave { get; set; }
    public string? DescuentoMax { get; set; }
    public string? CompaniaRuc { get; set; }
    public string? CompaniaNomUbg { get; set; }
    public string? CompaniaComercial { get; set; }
    public string? CompaniaDirecSunat { get; set; }
    public string? UsuarioSol { get; set; }
    public string? ClaveSol { get; set; }
    public string? CertificadoBase64 { get; set; }
    public string? ClaveCertificado { get; set; }
    public string? Entorno { get; set; }
    public string? CompaniaTelefono { get; set; }
    public bool BoletaPorLote { get; set; } = true;
    //public string? RUC { get; set; }
    //public string? UsuarioSerie { get; set; }
    //public string? Avatar { get; set; }
    public string? Token { get; set; }
    //public string? Roles { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public int ExpiresInSeconds { get; set; }

}
