using Ecommerce.Domain;
namespace Ecommerce.Application.Contracts.Clientes;

public interface ICliente
{
    Task<string> InsertarAsync(Cliente cliente, CancellationToken cancellationToken = default);
    Task<bool> EliminarAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Cliente>> ListarAsync(
        string? estado = "ACTIVO",
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);
    Task<string> ListarComboAsync(CancellationToken cancellationToken = default);
}
