namespace Ecommerce.Domain;

public class NotaPedido
{
    public long NotaId { get; set; }
    public string? NotaDocu { get; set; }
    public long? ClienteId { get; set; }
    public DateTime? NotaFecha { get; set; }
    public string? NotaUsuario { get; set; }
    public string? NotaFormaPago { get; set; }
    public string? NotaCondicion { get; set; }
    public DateTime? NotaFechaPago { get; set; }
    public string? NotaDireccion { get; set; }
    public string? NotaTelefono { get; set; }
    public decimal? NotaSubtotal { get; set; }
    public decimal? NotaMovilidad { get; set; }
    public decimal? NotaDescuento { get; set; }
    public decimal? NotaTotal { get; set; }
    public decimal? NotaAcuenta { get; set; }
    public decimal? NotaSaldo { get; set; }
    public decimal? NotaAdicional { get; set; }
    public decimal? NotaTarjeta { get; set; }
    public decimal? NotaPagar { get; set; }
    public string? NotaEstado { get; set; }
    public int? CompaniaId { get; set; }
    public string? NotaEntrega { get; set; }
    public string? ModificadoPor { get; set; }
    public DateTime? FechaEdita { get; set; }
    public string? NotaConcepto { get; set; }
    public string? NotaSerie { get; set; }
    public string? NotaNumero { get; set; }
    public decimal? NotaGanancia { get; set; }
    public decimal? ICBPER { get; set; }
    public int? CajaId { get; set; }
    public string? EntidadBancaria { get; set; }
    public string? NroOperacion { get; set; }
    public decimal? Efectivo { get; set; }
    public decimal? Deposito { get; set; }
}
