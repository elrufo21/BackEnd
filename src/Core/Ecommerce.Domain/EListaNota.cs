namespace Ecommerce.Domain;

public class EListaNota
{
    public string? NotaId { get; set; }
    public string? Documento { get; set; }
    public string? Fecha { get; set; }
    public string? Cliente { get; set; }
    public string? FormaPago { get; set; }
    public string? Total { get; set; }
    public string? Acuenta { get; set; }
    public string? Saldo { get; set; }
    public string? Usuario { get; set; }
    public string? Estado { get; set; }

    // Campos completos devueltos por dbo.listaNotaPedido
    public string? ClienteId { get; set; }
    public string? ClienteRazon { get; set; }
    public string? ClienteRuc { get; set; }
    public string? ClienteDni { get; set; }
    public string? ClienteDireccion { get; set; }
    public string? ClienteTelefono { get; set; }
    public string? ClienteCorreo { get; set; }
    public string? ClienteEstado { get; set; }
    public string? ClienteDespacho { get; set; }
    public string? ClienteUsuario { get; set; }
    public string? ClienteFecha { get; set; }
    public string? NotaFecha { get; set; }
    public string? NotaUsuario { get; set; }
    public string? NotaFormaPago { get; set; }
    public string? NotaCondicion { get; set; }
    public string? NotaFechaPago { get; set; }
    public string? NotaDireccion { get; set; }
    public string? NotaTelefono { get; set; }
    public string? NotaSubtotal { get; set; }
    public string? NotaMovilidad { get; set; }
    public string? NotaDescuento { get; set; }
    public string? NotaTotal { get; set; }
    public string? NotaAcuenta { get; set; }
    public string? NotaSaldo { get; set; }
    public string? NotaAdicional { get; set; }
    public string? NotaTarjeta { get; set; }
    public string? NotaPagar { get; set; }
    public string? NotaEstado { get; set; }
    public string? CompaniaId { get; set; }
    public string? NotaEntrega { get; set; }
    public string? ModificadoPor { get; set; }
    public string? FechaEdita { get; set; }
    public string? NotaConcepto { get; set; }
    public string? NotaSerie { get; set; }
    public string? NotaNumero { get; set; }
    public string? NotaGanancia { get; set; }
    public string? Icbper { get; set; }
    public string? CajaId { get; set; }
    public string? EntidadBancaria { get; set; }
    public string? NroOperacion { get; set; }
    public string? Efectivo { get; set; }
    public string? Deposito { get; set; }
    public string? EstadoSunat { get; set; }
}
