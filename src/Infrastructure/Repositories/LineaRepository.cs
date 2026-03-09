using System.Data;
using Ecommerce.Application.Contracts.Lineas;
using Ecommerce.Domain;
using Ecommerce.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class LineaRepository : ILinea
{
    private readonly string _connectionString;
    private readonly AccesoDatos _accesoDatos;

    public LineaRepository(IConfiguration configuration, AccesoDatos accesoDatos)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing connection string: DefaultConnection");
        _accesoDatos = accesoDatos;
    }

    public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "uspEliminarCategoria";
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

    public async Task<string> InsertarAsync(Linea linea, CancellationToken cancellationToken = default)
    {
        var data = $"{linea.IdSubLinea}|{linea.NombreSublinea?.Trim()}|{linea.CodigoSunat?.Trim()}";
        var result = await _accesoDatos.EjecutarComandoAsync("uspInsertarCategoria", "@Data", data, cancellationToken);
        return string.IsNullOrWhiteSpace(result) ? "error" : result;
    }

    public async Task<IReadOnlyList<EGeneral>> ListarAsync(int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        (page, pageSize) = NormalizePagination(page, pageSize);

        const string sql = """
            SELECT IdSubLinea, NombreSublinea, CodigoSunat
            FROM Sublinea
            ORDER BY IdSubLinea DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
        cmd.Parameters.AddWithValue("@PageSize", pageSize);
        await con.OpenAsync(cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

        var lista = new List<EGeneral>();
        while (await reader.ReadAsync(cancellationToken))
        {
            lista.Add(new EGeneral
            {
                Id = reader["IdSubLinea"].ToString() ?? string.Empty,
                nombreSublinea = reader["NombreSublinea"].ToString() ?? string.Empty,
                CodigoSunat = reader["CodigoSunat"].ToString() ?? string.Empty
            });
        }

        return lista;
    }

    private static (int page, int pageSize) NormalizePagination(int page, int pageSize)
    {
        var normalizedPage = page < 1 ? 1 : page;
        var normalizedPageSize = pageSize < 1 ? 1 : Math.Min(pageSize, 100);
        return (normalizedPage, normalizedPageSize);
    }
}
