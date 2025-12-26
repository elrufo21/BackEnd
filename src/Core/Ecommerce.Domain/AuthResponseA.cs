namespace Ecommerce.Domain;
public class AuthResponseA
{
   public string? Id { get; set; }
    public string? PersonalId { get; set; }
    public string? Area { get; set; }
    public string? Usuario { get; set; }
    public string? CompaniaId { get; set; }
    public string? RazonSocial { get; set; }
    //public string? RUC { get; set; }
    //public string? UsuarioSerie { get; set; }
    //public string? Avatar { get; set; }
    public string? Token { get; set; }
    //public string? Roles { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public int ExpiresInSeconds { get; set; }

}