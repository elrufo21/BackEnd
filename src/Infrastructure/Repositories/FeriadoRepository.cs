using System.Data;
using Ecommerce.Application.Contracts.Feriados;
using Ecommerce.Domain;
using Ecommerce.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class FeriadoRepository : IFeriado
{
    private readonly string _connectionString;
    private readonly AccesoDatos _accesoDatos;

    public FeriadoRepository(IConfiguration configuration, AccesoDatos accesoDatos)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing connection string: DefaultConnection");
        _accesoDatos = accesoDatos;
    }

    public async Task<string> InsertarAsync(Feriado feriado, CancellationToken cancellationToken = default)
    {
        var data = $"{feriado.IdFeriado}|{feriado.Fecha?.ToString("MM-dd-yyyy")}|{feriado.Motivo?.Trim()}";
        var result = await _accesoDatos.EjecutarComandoAsync("uspIngresarFeriado", "@Data", data, cancellationToken);
        return string.IsNullOrWhiteSpace(result) ? "error" : result;
    }

    public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "uspEliminarFeriado";
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

    public async Task<Feriado?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT IdFeriado, Fecha, Motivo
            FROM Feriado
            WHERE IdFeriado = @Id;
            """;
        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@Id", id);
        await con.OpenAsync(cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? MapFeriado(reader) : null;
    }

    public async Task<IReadOnlyList<Feriado>> ListarAsync(int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        (page, pageSize) = NormalizePagination(page, pageSize);

        const string sql = """
            SELECT IdFeriado, Fecha, Motivo
            FROM Feriado
            ORDER BY IdFeriado DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
        cmd.Parameters.AddWithValue("@PageSize", pageSize);
        await con.OpenAsync(cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

        var lista = new List<Feriado>();
        while (await reader.ReadAsync(cancellationToken))
        {
            lista.Add(MapFeriado(reader));
        }

        return lista;
    }

    private static Feriado MapFeriado(SqlDataReader reader)
    {
        return new Feriado
        {
            IdFeriado = Convert.ToInt32(reader["IdFeriado"]),
            Fecha = reader["Fecha"] == DBNull.Value ? null : Convert.ToDateTime(reader["Fecha"]),
            Motivo = reader["Motivo"]?.ToString()
        };
    }

    private static (int page, int pageSize) NormalizePagination(int page, int pageSize)
    {
        var normalizedPage = page < 1 ? 1 : page;
        var normalizedPageSize = pageSize < 1 ? 1 : Math.Min(pageSize, 100);
        return (normalizedPage, normalizedPageSize);
    }
}
