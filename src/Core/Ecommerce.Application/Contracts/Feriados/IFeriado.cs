using Ecommerce.Domain;

namespace Ecommerce.Application.Contracts.Feriados;

public interface IFeriado
{
    Task<string> InsertarAsync(Feriado feriado, CancellationToken cancellationToken = default);
    Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
    Task<Feriado?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<string> ListarAsync(CancellationToken cancellationToken = default);
}
