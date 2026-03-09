using Ecommerce.Domain;

namespace Ecommerce.Application.Contracts.Proveedores;

public interface IProveedor
{
    Task<string> InsertarAsync(Proveedor proveedor, CancellationToken cancellationToken = default);
    Task<bool> EliminarAsync(long id, CancellationToken cancellationToken = default);
    Task<Proveedor?> ObtenerPorIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Proveedor>> ListarAsync(
        string? estado = "ACTIVO",
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);
}
