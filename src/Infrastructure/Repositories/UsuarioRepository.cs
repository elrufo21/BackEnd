using Ecommerce.Application.Contracts.Usuarios;
using Ecommerce.Application.Features.Auths.Users.Vms;
using Ecommerce.Application.Identity;
using Ecommerce.Domain;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class UsuarioRepository : IUsuario
{
    private readonly IAuthService _authService;

    public UsuarioRepository(
                    IAuthService authService)
    {
        _authService = authService;
    }

    AccesoDatos daSQL = new AccesoDatos();
    public AuthResponseA Login(EUser loginUser)
    {
        string? xvalue = string.Empty;
        string[] xinfo;
        xvalue = loginUser.Email + "|" + loginUser.Password + "|";
        string? rpt;
        rpt = daSQL.ejecutarComando("uspValidaUsuario", "@Data", xvalue);

        if (string.IsNullOrEmpty(rpt))
        {
            throw new Exception("No hay conexion con el servidor");
        }
        else
        {
            xinfo = rpt.Split('[');
            if (xinfo[0] == "~")
            {
                throw new Exception("Acceso Denegado, Usted no es Usuario del Sistema.");
            }
        }
        string[] data = xinfo[0].Split('|');
        var authResponse = new AuthResponseA
        {
            Id = data[0].ToString(),
            PersonalId = data[1].ToString(),
            Area = data[2].ToString(),
            Nombre = data[3].ToString(),
            CompaniaId = data[4].ToString(),
            RazonSocial = data[5].ToString(),
            RUC = data[6].ToString(),
            UsuarioSerie = data[7].ToString(),
            Avatar = data[23].ToString(),
            //DireccionEnvio = _mapper.Map<AddressVm>(direccionEnvio),
            Token = _authService.CreateTokenA(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")),
            Roles = "ADMIN"
        };
        return authResponse;
    }
}
