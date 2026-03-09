using Ecommerce.Domain;

namespace Ecommerce.Application.Contracts.Maquinas;

public interface IMaquina
{
    Task<string> InsertarAsync(Maquina maquina, CancellationToken cancellationToken = default);
    Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Maquina>> ListarAsync(int page = 1, int pageSize = 50, CancellationToken cancellationToken = default);
}
