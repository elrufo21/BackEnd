namespace Ecommerce.Domain;

public class Producto
{
    public long IdProducto { get; set; }
    public long? IdSubLinea { get; set; }
    public string? ProductoCodigo { get; set; }
    public string? ProductoNombre { get; set; }
    public string? ProductoUM { get; set; }
    public decimal? ProductoCosto { get; set; }
    public decimal? ProductoVenta { get; set; }
    public decimal? ProductoVentaB { get; set; }
    public decimal? ProductoCantidad { get; set; }
    public string? ProductoEstado { get; set; }
    public string? ProductoUsuario { get; set; }
    public DateTime? ProductoFecha { get; set; }
    public string? ProductoImagen { get; set; }
    public decimal? ValorCritico { get; set; }
    public string? AplicaINV { get; set; }

}
