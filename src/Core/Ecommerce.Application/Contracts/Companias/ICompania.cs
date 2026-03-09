using Ecommerce.Domain;

namespace Ecommerce.Application.Contracts.Companias;

public interface ICompania
{
    Task<bool> InsertarAsync(Compania compania, CancellationToken cancellationToken = default);
    Task<bool> EditarAsync(int id, Compania compania, CancellationToken cancellationToken = default);
    Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Compania>> ListarAsync(int page = 1, int pageSize = 50, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EGeneral>> ListarComboAsync(int page = 1, int pageSize = 50, CancellationToken cancellationToken = default);
}
