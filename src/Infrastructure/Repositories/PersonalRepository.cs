using System.Data;
using System.Globalization;
using Ecommerce.Application.Contracts.Personales;
using Ecommerce.Domain;
using Ecommerce.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class PersonalRepository : IPersonal
{
    private readonly string _connectionString;
    private readonly AccesoDatos _accesoDatos;

    public PersonalRepository(IConfiguration configuration, AccesoDatos accesoDatos)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing connection string: DefaultConnection");
        _accesoDatos = accesoDatos;
    }

    public async Task<string> InsertarAsync(Personal personal, CancellationToken cancellationToken = default)
    {
        var data = $"{personal.PersonalId}|{personal.PersonalNombres?.Trim()}|{personal.PersonalApellidos?.Trim()}|{personal.AreaId}|{personal.PersonalCodigo?.Trim()}|{FormatDate(personal.PersonalNacimiento)}|{FormatDate(personal.PersonalIngreso)}|{personal.PersonalDNI?.Trim()}|{personal.PersonalDireccion?.Trim()}|{personal.PersonalTelefono?.Trim()}|{personal.PersonalEmail?.Trim()}|{personal.PersonalEstado}|{personal.PersonalImagen}|{personal.CompaniaId}";
        var result = await _accesoDatos.EjecutarComandoAsync("uspIngresarPersonal", "@Data", data, cancellationToken);
        return string.IsNullOrWhiteSpace(result) ? "error" : result;
    }

    public async Task<bool> EliminarAsync(long id, CancellationToken cancellationToken = default)
    {
        const string sql = "uspEliminarPersonal";
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

    public async Task<Personal?> ObtenerPorIdAsync(long id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT PersonalId, PersonalNombres, PersonalApellidos, AreaId, PersonalCodigo,
                   PersonalNacimiento, PersonalIngreso, PersonalDNI, PersonalDireccion,
                   PersonalTelefono, PersonalEmail, PersonalEstado, PersonalImagen, CompaniaId
            FROM Personal
            WHERE PersonalId = @Id;
            """;

        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@Id", id);
        await con.OpenAsync(cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? MapPersonal(reader) : null;
    }

    public async Task<IReadOnlyList<Personal>> ListarAsync(string? estado = "ACTIVO", int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        (page, pageSize) = NormalizePagination(page, pageSize);

        const string sql = """
            SELECT PersonalId, PersonalNombres, PersonalApellidos, AreaId, PersonalCodigo,
                   PersonalNacimiento, PersonalIngreso, PersonalDNI, PersonalDireccion,
                   PersonalTelefono, PersonalEmail, PersonalEstado, PersonalImagen, CompaniaId
            FROM Personal
            WHERE (@Estado IS NULL OR PersonalEstado = @Estado)
            ORDER BY PersonalId DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, con)
        {
            CommandTimeout = 300,
            CommandType = CommandType.Text
        };
        cmd.Parameters.AddWithValue("@Estado", (object?)estado ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
        cmd.Parameters.AddWithValue("@PageSize", pageSize);
        await con.OpenAsync(cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

        var lista = new List<Personal>();
        while (await reader.ReadAsync(cancellationToken))
        {
            lista.Add(MapPersonal(reader));
        }

        return lista;
    }

    private static Personal MapPersonal(SqlDataReader reader)
    {
        return new Personal
        {
            PersonalId = Convert.ToInt64(reader["PersonalId"]),
            PersonalNombres = reader["PersonalNombres"].ToString(),
            PersonalApellidos = reader["PersonalApellidos"].ToString(),
            AreaId = reader["AreaId"] == DBNull.Value ? null : Convert.ToInt64(reader["AreaId"]),
            PersonalCodigo = reader["PersonalCodigo"].ToString(),
            PersonalNacimiento = ReadNullableDate(reader, "PersonalNacimiento"),
            PersonalIngreso = ReadNullableDate(reader, "PersonalIngreso"),
            PersonalDNI = reader["PersonalDNI"].ToString(),
            PersonalDireccion = reader["PersonalDireccion"].ToString(),
            PersonalTelefono = reader["PersonalTelefono"].ToString(),
            PersonalEmail = reader["PersonalEmail"].ToString(),
            PersonalEstado = reader["PersonalEstado"].ToString(),
            PersonalImagen = reader["PersonalImagen"].ToString(),
            CompaniaId = reader["CompaniaId"] == DBNull.Value ? null : Convert.ToInt32(reader["CompaniaId"])
        };
    }

    private static DateTime? ReadNullableDate(SqlDataReader reader, string columnName)
    {
        if (reader[columnName] == DBNull.Value) return null;

        if (reader[columnName] is DateTime dtValue)
        {
            return dtValue;
        }

        var value = reader[columnName]?.ToString();
        if (string.IsNullOrWhiteSpace(value)) return null;

        if (DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var exact))
        {
            return exact;
        }

        return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed)
            ? parsed
            : (DateTime?)null;
    }

    private static string? FormatDate(DateTime? date)
    {
        return date?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    private static (int page, int pageSize) NormalizePagination(int page, int pageSize)
    {
        var normalizedPage = page < 1 ? 1 : page;
        var normalizedPageSize = pageSize < 1 ? 1 : Math.Min(pageSize, 100);
        return (normalizedPage, normalizedPageSize);
    }
}
