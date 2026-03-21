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
            Id = GetPayloadValue(payload, 0),
            PersonalId = GetPayloadValue(payload, 1),
            Area = GetPayloadValue(payload, 2),
            Usuario = GetPayloadValue(payload, 3),
            CompaniaId = GetPayloadValue(payload, 4),
            RazonSocial = GetPayloadValue(payload, 5),
            FechaVencimientoClave = GetPayloadValue(payload, 6, null),
            DescuentoMax = GetPayloadValue(payload, 7, "0"),
            CompaniaRuc = GetPayloadValue(payload, 8),
            CompaniaNomUbg = GetPayloadValue(payload, 9),
            CompaniaComercial = GetPayloadValue(payload, 10),
            CompaniaDirecSunat = GetPayloadValue(payload, 11),
            UsuarioSol = GetPayloadValue(payload, 12),
            ClaveSol = GetPayloadValue(payload, 13),
            CertificadoBase64 = GetPayloadValue(payload, 14),
            ClaveCertificado = GetPayloadValue(payload, 15),
            Entorno = GetPayloadValue(payload, 16, "3"),
            Token = _authService.CreateTokenA(expiresAtUtc.ToString("O")),
            ExpiresAtUtc = expiresAtUtc,
            ExpiresInSeconds = expiresInSeconds
        };
    }

    private static string? GetPayloadValue(string[] payload, int index, string? fallback = "")
    {
        return payload.Length > index ? payload[index] : fallback;
    }
}
