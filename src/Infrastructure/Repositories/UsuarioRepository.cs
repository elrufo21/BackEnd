using Ecommerce.Application.Contracts.Usuarios;
using Ecommerce.Application.Identity;
using Ecommerce.Application.Models.Token;
using Ecommerce.Domain;
using Ecommerce.Infrastructure.Persistence;
using Microsoft.Extensions.Options;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class UsuarioRepository : IUsuario
{
    private readonly IAuthService _authService;
    private readonly JwtSettings _jwtSettings;
    private readonly AccesoDatos _accesoDatos;

    public UsuarioRepository(
        IAuthService authService,
        IOptions<JwtSettings> jwtSettings,
        AccesoDatos accesoDatos)
    {
        _authService = authService;
        _jwtSettings = jwtSettings.Value;
        _accesoDatos = accesoDatos;
    }

    public async Task<AuthResponseA> LoginAsync(EUser loginUser, CancellationToken cancellationToken = default)
    {
        var data = $"{loginUser.Email}|{loginUser.Password}";
        var result = await _accesoDatos.EjecutarComandoAsync("uspValidaUsuario", "@Data", data, cancellationToken);

        if (string.IsNullOrWhiteSpace(result))
        {
            throw new InvalidOperationException("No hay conexión con el servidor.");
        }

        var info = result.Split('[');
        if (info.Length == 0 || info[0] == "~")
        {
            throw new UnauthorizedAccessException("Acceso denegado, usuario no válido.");
        }

        var payload = info[0].Split('|');
        if (payload.Length < 6)
        {
            throw new InvalidOperationException("Respuesta de autenticación inválida.");
        }

        var nowUtc = DateTime.UtcNow;
        var expiresAtUtc = nowUtc.Add(_jwtSettings.ExpireTime);
        var expiresInSeconds = (int)_jwtSettings.ExpireTime.TotalSeconds;
        return new AuthResponseA
        {
            Id = payload[0],
            PersonalId = payload[1],
            Area = payload[2],
            Usuario = payload[3],
            CompaniaId = payload[4],
            RazonSocial = payload[5],
            Token = _authService.CreateTokenA(expiresAtUtc.ToString("O")),
            ExpiresAtUtc = expiresAtUtc,
            ExpiresInSeconds = expiresInSeconds
        };
    }
}
