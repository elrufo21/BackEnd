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

    public async Task<string> ListarAsync(CancellationToken cancellationToken = default)
    {
        var result = await _accesoDatos.EjecutarComandoAsync("dbo.uspTraerFeriados", cancellationToken: cancellationToken);
        return string.IsNullOrWhiteSpace(result) ? string.Empty : result;
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

}
