namespace Ecommerce.Domain;

public class EDetalleNota
{
    public EDetalleNota()
    {

    }
    public EDetalleNota(
    int id,
    int productoId,
    decimal cantidad,
    string unidad,
    string producto,
    decimal precio,
    decimal importe,
    string imagen,
    decimal valorUM
    )
    {
        Id = id;
        ProductoId = productoId;
        Cantidad = cantidad;
        Unidad = unidad;
        Producto = producto;
        Precio = precio;
        Importe = importe;
        Imagen = imagen;
        ValorUM = valorUM;
    }
    public int? Id { get; set; }
    public int? ProductoId { get; set; }
    public decimal? Cantidad { get; set; }
    public string? Unidad { get; set; }
    public string? Producto { get; set; }
    //public decimal? Costo { get; set; }
    public decimal? Precio { get; set; }
    public decimal? Importe { get; set; }
    //public string? DetalleEstado { get; set; }
    public string? Imagen { get; set; }
    public decimal? ValorUM { get; set; }
    //public string? EstadoD { get; set; }

}
