namespace Ecommerce.Domain;

public class Producto
{
    public long IdProducto { get; set; }
    public long? IdSubLinea { get; set; }
    public string? ProductoCodigo { get; set; }
    public string? ProductoNombre { get; set; }
    public decimal? ProductoTipoCambio { get; set; }
    public decimal? ProductoCostoDolar { get; set; }
    public string? ProductoUM { get; set; }
    public decimal? ProductoCosto { get; set; }
    public decimal? ProductoVenta { get; set; }
    public decimal? ProductoVentaB { get; set; }
    public decimal? ProductoCantidad { get; set; }
    public string? ProductoObs { get; set; }
    public string? ProductoEstado { get; set; }
    public string? ProductoUsuario { get; set; }
    public DateTime? ProductoFecha { get; set; }
    public string? ProductoImagen { get; set; }
    public decimal? ValorCritico { get; set; }
    public string? AplicaTC { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public bool? AplicaFechaV { get; set; }
    public string? AplicaINV { get; set; }
    public decimal? CantidadANT { get; set; }
    public DateTime? FechaModCant { get; set; }
}
