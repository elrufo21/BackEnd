using Ecommerce.Domain;

namespace Ecommerce.Application.Contracts.Usuarios;

public interface IUsuariosCrud
{
    Task<int> InsertarAsync(UsuarioBd usuario, CancellationToken cancellationToken = default);
    Task<bool> EditarAsync(int id, UsuarioBd usuario, CancellationToken cancellationToken = default);
    Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UsuarioBd>> ListarAsync(
        string? estado = "ACTIVO",
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);
    Task<UsuarioBd?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UsuarioConPersonal>> ListarConPersonalAsync(
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);
    Task<UsuarioConPersonal?> ObtenerPorIdConPersonalAsync(int id, CancellationToken cancellationToken = default);
}
