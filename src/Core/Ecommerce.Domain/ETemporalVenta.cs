namespace Ecommerce.Domain;

public class ETemporalVenta
{
    public int? Id{ get; set; }
    public string? UsuarioId { get; set; }
    public string? IdProducto { get; set; }
    public decimal? Cantidad { get; set; }
    public decimal? Precio { get; set; }
    public decimal? Importe { get; set; }
    public decimal? ValorUM { get; set; }
    public string? Unidad { get; set; }
    public string? Codigo { get; set; }
    
}
