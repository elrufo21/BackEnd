using System.Collections.Generic;
using Ecommerce.Domain;
using NotaPedidoEntity = Ecommerce.Domain.NotaPedido;
namespace Ecommerce.Application.Contracts.NotaPedido;
public interface INotaPedido
{
    Task<string> RegistrarOrdenAsync(string data, CancellationToken cancellationToken = default);
    Task<string> EditarOrdenAsync(string data, CancellationToken cancellationToken = default);
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
