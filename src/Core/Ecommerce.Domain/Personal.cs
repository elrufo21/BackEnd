namespace Ecommerce.Domain;

public class Personal
{
    public long PersonalId { get; set; }
    public string? PersonalNombres { get; set; }
    public string? PersonalApellidos { get; set; }
    public long? AreaId { get; set; }
    public string? PersonalCodigo { get; set; }
    public DateTime? PersonalNacimiento { get; set; }
    public DateTime? PersonalIngreso { get; set; }
    public string? PersonalDNI { get; set; }
    public string? PersonalDireccion { get; set; }
    public string? PersonalTelefono { get; set; }
    public string? PersonalEmail { get; set; }
    public string? PersonalEstado { get; set; }
    public string? PersonalImagen { get; set; }
    public int? CompaniaId { get; set; }
}
