using Ecommerce.Domain;

namespace Ecommerce.Application.Contracts.Areas;

public interface IArea
{
    Task<string> InsertarAsync(Area area, CancellationToken cancellationToken = default);
    Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EGeneral>> ListarAsync(int page = 1, int pageSize = 50, CancellationToken cancellationToken = default);
}
