using Ecommerce.Domain;

namespace Ecommerce.Application.Contracts.Usuarios;

public interface IUsuario
{
    public AuthResponseA Login(EUser loginUser);
}
