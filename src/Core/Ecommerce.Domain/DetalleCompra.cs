namespace Ecommerce.Domain;

public class DetalleCompra
{
    public long DetalleId { get; set; }
    public long CompraId { get; set; }
    public long? IdProducto { get; set; }
    public string? DetalleCodigo { get; set; }
    public string? Descripcion { get; set; }
    public string? DetalleUm { get; set; }
    public decimal? DetalleCantidad { get; set; }
    public decimal? PrecioCosto { get; set; }
    public decimal? DetalleImporte { get; set; }
    public decimal? DetalleDescuento { get; set; }
    public string? DetalleEstado { get; set; }
    public decimal? DescuentoB { get; set; }
    public string? EstadoB { get; set; }
    public decimal? ValorUM { get; set; }
}
