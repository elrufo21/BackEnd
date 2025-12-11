namespace Ecommerce.Domain;

public class Compania
{
    public int CompaniaId { get; set; }
    public string? CompaniaRazonSocial { get; set; }
    public string? CompaniaRUC { get; set; }
    public string? CompaniaDireccion { get; set; }
    public string? CompaniaTelefono { get; set; }
    public string? CompaniaEmail { get; set; }
    public string? CompaniaIniFecha { get; set; }
    public string? CompaniaComercial { get; set; }
    public string? CompaniaUserSecun { get; set; }
    public string? ComapaniaPWD { get; set; }
    public string? CompaniaPFX { get; set; }
    public string? CompaniaClave { get; set; }
    public string? CompaniaNomUBG { get; set; }
    public string? CompaniaCodigoUBG { get; set; }
    public string? CompaniaDistrito { get; set; }
    public string? CompaniaDirecSunat { get; set; }
    public decimal? ICBPER { get; set; }
    public string? TokenApi { get; set; }
    public string? ClienIdToken { get; set; }
    public decimal? DescuentoMax { get; set; }
    public int? DiasMaxDep { get; set; }
    public DateTime? RenovacionOSE { get; set; }
    public DateTime? RenovacionFirma { get; set; }
    public DateTime? RenovacionSome { get; set; }
    public string? CorreoSGO { get; set; }
    public string? PasswordCorreo { get; set; }
    public string? CorreosAdmin { get; set; }
}
