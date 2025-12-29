namespace Ecommerce.Domain;

public class DetalleNota
{
    public long DetalleId { get; set; }
    public long NotaId { get; set; }
    public long? IdProducto { get; set; }
    public decimal? DetalleCantidad { get; set; }
    public string? DetalleUm { get; set; }
    public string? DetalleDescripcion { get; set; }
    public decimal? DetalleCosto { get; set; }
    public decimal? DetallePrecio { get; set; }
    public decimal? DetalleImporte { get; set; }
    public string? DetalleEstado { get; set; }
    public decimal? CantidadSaldo { get; set; }
    public decimal? ValorUM { get; set; }
}
