using System.Data;
using Ecommerce.Application.Contracts.Usuarios;
using Ecommerce.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class UsuariosCrudRepository : IUsuariosCrud
{
    private readonly string _connectionString;

    public UsuariosCrudRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing connection string: DefaultConnection");
    }

    public async Task<int> InsertarAsync(UsuarioBd usuario, CancellationToken cancellationToken = default)
    {
        var (id, status) = await EjecutarUpsertAsync(0, usuario, cancellationToken);
        if (string.Equals(status, "EXISTE_USUARIO", StringComparison.OrdinalIgnoreCase))
        {
            return -1;
        }

        return id;
    }

    public async Task<bool> EditarAsync(int id, UsuarioBd usuario, CancellationToken cancellationToken = default)
    {
        var (_, status) = await EjecutarUpsertAsync(id, usuario, cancellationToken);
        return string.Equals(status, "UPDATED", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "uspEliminarUsuario";
        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, con)
        {
            CommandTimeout = 300,
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.AddWithValue("@Id", id);
        await con.OpenAsync(cancellationToken);
        var rows = await cmd.ExecuteNonQueryAsync(cancellationToken);
        return rows > 0;
    }

    public async Task<IReadOnlyList<UsuarioBd>> ListarAsync(string? estado = "ACTIVO", int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        (page, pageSize) = NormalizePagination(page, pageSize);

        const string sql = """
            SELECT U.UsuarioID,
                   U.PersonalId,
                   CONCAT(ISNULL(P.PersonalNombres, ''), ' ', ISNULL(P.PersonalApellidos, '')) AS Nombre,
                   U.UsuarioAlias,
                   U.UsuarioFechaReg AS Fecha,
                   U.UsuarioEstado AS Estado,
                   A.AreaNombre AS Area
            FROM Usuarios U
            LEFT JOIN Personal P ON P.PersonalId = U.PersonalId
            LEFT JOIN Area A ON A.AreaId = P.AreaId
            WHERE (@Estado IS NULL OR U.UsuarioEstado = @Estado)
            ORDER BY U.UsuarioID DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@Estado", (object?)estado ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
        cmd.Parameters.AddWithValue("@PageSize", pageSize);
        await con.OpenAsync(cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

        var lista = new List<UsuarioBd>();
        while (await reader.ReadAsync(cancellationToken))
        {
            lista.Add(Map(reader));
        }

        return lista;
    }

    public async Task<UsuarioBd?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT U.UsuarioID,
                   U.PersonalId,
                   CONCAT(ISNULL(P.PersonalNombres, ''), ' ', ISNULL(P.PersonalApellidos, '')) AS Nombre,
                   U.UsuarioAlias,
                   U.UsuarioFechaReg AS Fecha,
                   U.UsuarioEstado AS Estado,
                   A.AreaNombre AS Area
            FROM Usuarios U
            LEFT JOIN Personal P ON P.PersonalId = U.PersonalId
            LEFT JOIN Area A ON A.AreaId = P.AreaId
            WHERE U.UsuarioID = @UsuarioID;
            """;

        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@UsuarioID", id);
        await con.OpenAsync(cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
    }

    public async Task<IReadOnlyList<UsuarioConPersonal>> ListarConPersonalAsync(int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        (page, pageSize) = NormalizePagination(page, pageSize);

        const string sql = """
            SELECT U.UsuarioID,
                   U.PersonalId,
                   U.UsuarioAlias,
                   U.UsuarioFechaReg AS Fecha,
                   U.UsuarioEstado,
                   P.PersonalId AS PersonalIdPersonal,
                   P.PersonalNombres,
                   P.PersonalApellidos,
                   P.AreaId,
                   P.PersonalCodigo,
                   P.PersonalNacimiento,
                   P.PersonalIngreso,
                   P.PersonalDNI,
                   P.PersonalDireccion,
                   P.PersonalTelefono,
                   P.PersonalEmail,
                   P.PersonalEstado,
                   P.PersonalImagen,
                   P.CompaniaId
            FROM Usuarios U
            LEFT JOIN Personal P ON U.PersonalId = P.PersonalId
            ORDER BY U.UsuarioID DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
        cmd.Parameters.AddWithValue("@PageSize", pageSize);
        await con.OpenAsync(cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

        var lista = new List<UsuarioConPersonal>();
        while (await reader.ReadAsync(cancellationToken))
        {
            lista.Add(MapWithPersonal(reader));
        }
        return lista;
    }

    public async Task<UsuarioConPersonal?> ObtenerPorIdConPersonalAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT U.UsuarioID,
                   U.PersonalId,
                   U.UsuarioAlias,
                   U.UsuarioFechaReg AS Fecha,
                   U.UsuarioEstado,
                   P.PersonalId AS PersonalIdPersonal,
                   P.PersonalNombres,
                   P.PersonalApellidos,
                   P.AreaId,
                   P.PersonalCodigo,
                   P.PersonalNacimiento,
                   P.PersonalIngreso,
                   P.PersonalDNI,
                   P.PersonalDireccion,
                   P.PersonalTelefono,
                   P.PersonalEmail,
                   P.PersonalEstado,
                   P.PersonalImagen,
                   P.CompaniaId
            FROM Usuarios U
            LEFT JOIN Personal P ON U.PersonalId = P.PersonalId
            WHERE U.UsuarioID = @UsuarioID;
            """;

        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@UsuarioID", id);
        await con.OpenAsync(cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? MapWithPersonal(reader) : null;
    }

    private static UsuarioBd Map(SqlDataReader reader)
    {
        return new UsuarioBd
        {
            UsuarioID = Convert.ToInt32(reader["UsuarioID"]),
            PersonalId = reader["PersonalId"] == DBNull.Value ? null : Convert.ToInt32(reader["PersonalId"]),
            Nombre = reader["Nombre"].ToString(),
            UsuarioAlias = reader["UsuarioAlias"].ToString(),
            UsuarioClave = null,
            Area = reader["Area"] == DBNull.Value ? null : reader["Area"].ToString(),
            UsuarioFechaReg = reader["Fecha"] == DBNull.Value ? null : Convert.ToDateTime(reader["Fecha"]),
            UsuarioEstado = reader["Estado"].ToString()
        };
    }

    private static UsuarioConPersonal MapWithPersonal(SqlDataReader reader)
    {
        var usuario = new UsuarioConPersonal
        {
            UsuarioID = Convert.ToInt32(reader["UsuarioID"]),
            PersonalId = reader["PersonalId"] == DBNull.Value ? null : Convert.ToInt32(reader["PersonalId"]),
            UsuarioAlias = reader["UsuarioAlias"].ToString(),
            UsuarioClave = null,
            UsuarioFechaReg = reader["Fecha"] == DBNull.Value ? null : Convert.ToDateTime(reader["Fecha"]),
            UsuarioEstado = reader["UsuarioEstado"].ToString()
        };

        if (reader["PersonalIdPersonal"] != DBNull.Value)
        {
            usuario.Personal = new Personal
            {
                PersonalId = Convert.ToInt64(reader["PersonalIdPersonal"]),
                PersonalNombres = reader["PersonalNombres"]?.ToString(),
                PersonalApellidos = reader["PersonalApellidos"]?.ToString(),
                AreaId = reader["AreaId"] == DBNull.Value ? null : Convert.ToInt64(reader["AreaId"]),
                PersonalCodigo = reader["PersonalCodigo"]?.ToString(),
                PersonalNacimiento = reader["PersonalNacimiento"] == DBNull.Value ? null : Convert.ToDateTime(reader["PersonalNacimiento"]),
                PersonalIngreso = reader["PersonalIngreso"] == DBNull.Value ? null : Convert.ToDateTime(reader["PersonalIngreso"]),
                PersonalDNI = reader["PersonalDNI"]?.ToString(),
                PersonalDireccion = reader["PersonalDireccion"]?.ToString(),
                PersonalTelefono = reader["PersonalTelefono"]?.ToString(),
                PersonalEmail = reader["PersonalEmail"]?.ToString(),
                PersonalEstado = reader["PersonalEstado"]?.ToString(),
                PersonalImagen = reader["PersonalImagen"]?.ToString(),
                CompaniaId = reader["CompaniaId"] == DBNull.Value ? null : Convert.ToInt32(reader["CompaniaId"])
            };
        }

        return usuario;
    }

    private async Task<(int id, string? status)> EjecutarUpsertAsync(int usuarioId, UsuarioBd usuario, CancellationToken cancellationToken)
    {
        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("uspInsertarUsuario", con)
        {
            CommandTimeout = 300,
            CommandType = CommandType.StoredProcedure
        };
        var dataParam = cmd.Parameters.Add("@Data", SqlDbType.VarChar, -1);
        dataParam.Value = BuildDataString(usuarioId, usuario);

        await con.OpenAsync(cancellationToken);
        var result = await cmd.ExecuteScalarAsync(cancellationToken);

        if (result == null)
        {
            return (0, null);
        }

        var resultString = result.ToString();
        if (int.TryParse(resultString, out var id))
        {
            return (id, null);
        }

        return (0, resultString);
    }

    private static string BuildDataString(int usuarioId, UsuarioBd usuario)
    {
        var personalId = usuario.PersonalId ?? 0;
        var alias = usuario.UsuarioAlias?.Trim() ?? string.Empty;
        var clave = usuario.UsuarioClave?.Trim() ?? string.Empty;
        var estado = usuario.UsuarioEstado?.Trim() ?? string.Empty;
        return $"{usuarioId}|{personalId}|{alias}|{clave}|{estado}";
    }

    private static (int page, int pageSize) NormalizePagination(int page, int pageSize)
    {
        var normalizedPage = page < 1 ? 1 : page;
        var normalizedPageSize = pageSize < 1 ? 1 : Math.Min(pageSize, 100);
        return (normalizedPage, normalizedPageSize);
    }
}
