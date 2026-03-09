using Ecommerce.Domain;

namespace Ecommerce.Application.Contracts.Usuarios;

public interface IUsuario
{
    Task<AuthResponseA> LoginAsync(EUser loginUser, CancellationToken cancellationToken = default);
}
