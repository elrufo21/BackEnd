namespace Ecommerce.Domain;

public class Compra
{
    public long CompraId { get; set; }
    public string? CompraCorrelativo { get; set; }
    public long? ProveedorId { get; set; }
    public DateTime? CompraRegistro { get; set; }
    public DateTime? CompraEmision { get; set; }
    public DateTime? CompraComputo { get; set; }
    public string? TipoCodigo { get; set; }
    public string? CompraSerie { get; set; }
    public string? CompraNumero { get; set; }
    public string? CompraCondicion { get; set; }
    public string? CompraMoneda { get; set; }
    public decimal? CompraTipoCambio { get; set; }
    public int? CompraDias { get; set; }
    public DateTime? CompraFechaPago { get; set; }
    public string? CompraUsuario { get; set; }
    public string? CompraTipoIgv { get; set; }
    public decimal? CompraValorVenta { get; set; }
    public decimal? CompraDescuento { get; set; }
    public decimal? CompraSubtotal { get; set; }
    public decimal? CompraIgv { get; set; }
    public decimal? CompraTotal { get; set; }
    public string? CompraEstado { get; set; }
    public string? CompraAsociado { get; set; }
    public decimal? CompraSaldo { get; set; }
    public string? CompraObs { get; set; }
    public decimal? CompraTipoSunat { get; set; }
    public string? CompraConcepto { get; set; }
    public decimal? CompraPercepcion { get; set; }
}
