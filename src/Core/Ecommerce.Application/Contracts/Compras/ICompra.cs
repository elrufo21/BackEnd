using System.Collections.Generic;
using Ecommerce.Domain;

namespace Ecommerce.Application.Contracts.Compras;

public interface ICompra
{
    Task<string> InsertarAsync(Compra compra, CancellationToken cancellationToken = default);
    Task<string> InsertarConDetalleAsync(Compra compra, IEnumerable<DetalleCompra> detalles, CancellationToken cancellationToken = default);
    Task<bool> EliminarAsync(long id, CancellationToken cancellationToken = default);
    Task<Compra?> ObtenerPorIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Compra>> ListarCrudAsync(
        string? estado = null,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DetalleCompra>> ListarDetalleAsync(
        long compraId,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);
}
