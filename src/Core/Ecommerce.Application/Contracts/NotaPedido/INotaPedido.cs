using System.Collections.Generic;
using Ecommerce.Domain;
using NotaPedidoEntity = Ecommerce.Domain.NotaPedido;
namespace Ecommerce.Application.Contracts.NotaPedido;
public interface INotaPedido
{
    Task<string> RegistrarOrdenAsync(string data, CancellationToken cancellationToken = default);
    Task<string> EditarOrdenAsync(string data, CancellationToken cancellationToken = default);
    Task<string> AnularDocumentoAsync(string listaOrden, CancellationToken cancellationToken = default);
    Task<string> ListarDocumentosAsync(string data, CancellationToken cancellationToken = default);
    Task<string> ListarLdDocumentosAsync(int mes, int anno, CancellationToken cancellationToken = default);
    Task<string> ListarLdDocumentosRangoAsync(DateTime fechaInicio, DateTime fechaFin, CancellationToken cancellationToken = default);
    Task<string> ListarBajasAsync(string data, CancellationToken cancellationToken = default);
    Task<string> RegistrarResumenBoletasAsync(string listaOrden, CancellationToken cancellationToken = default);
    Task<string> EditarResumenBoletasAsync(string data, CancellationToken cancellationToken = default);
    Task<string> ReenviarFacturaAsync(string data, CancellationToken cancellationToken = default);
    Task<string> RegistrarNotaCreditoAsync(string listaOrden, CancellationToken cancellationToken = default);
    Task<string> ReenviarNotaCreditoAsync(string data, CancellationToken cancellationToken = default);
    Task<string> RetornaBoletaPorTicketAsync(string resumenId, CancellationToken cancellationToken = default);
    Task<string> RetornarBoletasAsync(string resumenId, CancellationToken cancellationToken = default);
    Task<string?> ObtenerCdrBase64ResumenAsync(long resumenId, CancellationToken cancellationToken = default);
    Task<int> ActualizarRespuestaSunatDocumentoVentaPorResumenAsync(
        long resumenId,
        string codigoSunat,
        string mensajeSunat,
        string hashCdr,
        CancellationToken cancellationToken = default);
    Task<string?> ObtenerUsuarioDocumentoVentaAsync(IEnumerable<long> docuIds, CancellationToken cancellationToken = default);
    Task<string> TraerSecuenciaResumenAsync(string companiaId, CancellationToken cancellationToken = default);
    Task<string> ResumenPorFechaAsync(DateTime fechaInicio, DateTime fechaFin, CancellationToken cancellationToken = default);
    Task<CredencialesSunat?> ObtenerCredencialesSunatAsync(int companiaId, CancellationToken cancellationToken = default);
    Task<bool> GuardarCredencialesSunatAsync(
        int companiaId,
        string usuarioSol,
        string claveSol,
        string certificadoBase64,
        string claveCertificado,
        int entorno,
        CancellationToken cancellationToken = default);
    Task<string> InsertarAsync(NotaPedidoEntity notaPedido, CancellationToken cancellationToken = default);
    Task<string> InsertarConDetalleAsync(NotaPedidoEntity notaPedido, IEnumerable<DetalleNota> detalles, CancellationToken cancellationToken = default);
    Task<bool> EliminarAsync(long id, CancellationToken cancellationToken = default);
    Task<NotaPedidoEntity?> ObtenerPorIdAsync(long id, CancellationToken cancellationToken = default);
    Task<string> ObtenerNotaPedidoSpAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotaPedidoEntity>> ListarCrudAsync(
        string? estado = null,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DetalleNota>> ListarDetalleAsync(
        long notaId,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EListaNota>> ListarAsync(
        DateTime fechaInicio,
        DateTime fechaFin,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);
}
