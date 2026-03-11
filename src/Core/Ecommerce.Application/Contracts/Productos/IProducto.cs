using Ecommerce.Domain;
namespace Ecommerce.Application.Contracts.Productos;

public interface IProducto
{
    Task<string> InsertarAsync(Producto producto, CancellationToken cancellationToken = default);
    Task<bool> EliminarAsync(long id, CancellationToken cancellationToken = default);
    Task<Producto?> ObtenerPorIdAsync(long id, CancellationToken cancellationToken = default);
    Task<string> ListarCrudRawAsync(
        string? estado = "ACTIVO",
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Producto>> ListarCrudAsync(
        string? estado = "ACTIVO",
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EListaProducto>> ListarAsync(
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EListaProducto>> BuscarProductoAsync(
        string nombre,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);
}
