using Ecommerce.Domain;

namespace Ecommerce.Application.Contracts.Proveedores;

public interface ICuentaProveedor
{
    Task<string> InsertarAsync(CuentaProveedor cuenta, CancellationToken cancellationToken = default);
    Task<bool> EliminarAsync(long cuentaId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CuentaProveedor>> ListarPorProveedorAsync(
        long proveedorId,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);
}
