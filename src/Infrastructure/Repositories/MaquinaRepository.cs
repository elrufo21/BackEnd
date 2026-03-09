using Ecommerce.Application.Contracts.Maquinas;
using Ecommerce.Domain;
using Ecommerce.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class MaquinaRepository : IMaquina
{
    private readonly string _connectionString;
    private readonly AccesoDatos _accesoDatos;

    public MaquinaRepository(IConfiguration configuration, AccesoDatos accesoDatos)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing connection string: DefaultConnection");
        _accesoDatos = accesoDatos;
    }

    public async Task<string> InsertarAsync(Maquina maquina, CancellationToken cancellationToken = default)
    {
        var data = $"{maquina.IdMaquina}|{maquina.NombreMaquina?.Trim()}|{maquina.SerieFactura?.Trim()}|{maquina.SerieNC?.Trim()}|{maquina.SerieBoleta?.Trim()}|{maquina.Tiketera?.Trim()}";
        var result = await _accesoDatos.EjecutarComandoAsync("uspInsertarMaquina", "@Data", data, cancellationToken);
        return string.IsNullOrWhiteSpace(result) ? "error" : result;
    }

    public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM MAQUINAS WHERE IdMaquina = @Id";
        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@Id", id);
        await con.OpenAsync(cancellationToken);
        var rows = await cmd.ExecuteNonQueryAsync(cancellationToken);
        return rows > 0;
    }

    public async Task<IReadOnlyList<Maquina>> ListarAsync(int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        (page, pageSize) = NormalizePagination(page, pageSize);

        const string sql = """
            SELECT IdMaquina, Maquina, Registro, SerieFactura, SerieNC, SerieBoleta, Tiketera
            FROM MAQUINAS
            ORDER BY IdMaquina DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, con)
        {
            CommandTimeout = 300,
            CommandType = CommandType.Text
        };
        cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
        cmd.Parameters.AddWithValue("@PageSize", pageSize);
        await con.OpenAsync(cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

        var lista = new List<Maquina>();
        while (await reader.ReadAsync(cancellationToken))
        {
            lista.Add(new Maquina
            {
                IdMaquina = Convert.ToInt32(reader["IdMaquina"]),
                NombreMaquina = reader["Maquina"].ToString(),
                Registro = reader["Registro"].ToString(),
                SerieFactura = reader["SerieFactura"].ToString(),
                SerieNC = reader["SerieNC"].ToString(),
                SerieBoleta = reader["SerieBoleta"].ToString(),
                Tiketera = reader["Tiketera"].ToString()
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
