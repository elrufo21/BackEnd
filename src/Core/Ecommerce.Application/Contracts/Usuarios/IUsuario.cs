using Ecommerce.Application.Features.Auths.Users.Vms;
using Ecommerce.Domain;

namespace Ecommerce.Application.Contracts.Usuarios;

public interface IUsuario
{
    public AuthResponseA Login(EUser loginUser);
}
