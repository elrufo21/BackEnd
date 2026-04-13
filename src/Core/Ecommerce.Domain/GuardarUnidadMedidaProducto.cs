namespace Ecommerce.Domain;

public class GuardarUnidadMedidaProductoRequest
{
    public long IdProducto { get; set; }
    public string UMDescripcion { get; set; } = string.Empty;
    public decimal ValorUM { get; set; }
    public decimal PrecioVenta { get; set; }
    public decimal PrecioVentaB { get; set; }
    public decimal PrecioCosto { get; set; }
    public string? UnidadImagen { get; set; }
}

public class GuardarUnidadMedidaProductoResponse
{
    public long IdUm { get; set; }
}
