using System.Data;
using Ecommerce.Application.Contracts.Clientes;
using Ecommerce.Domain;
using Ecommerce.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class ClienteRepository : ICliente
{
    private readonly string _connectionString;
    private readonly AccesoDatos _accesoDatos;

    public ClienteRepository(IConfiguration configuration, AccesoDatos accesoDatos)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing connection string: DefaultConnection");
        _accesoDatos = accesoDatos;
    }

    public async Task<string> InsertarAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        var data = $"{cliente.ClienteId}|{cliente.ClienteRazon?.Trim()}|{cliente.ClienteRuc?.Trim()}|{cliente.ClienteDni?.Trim()}|{cliente.ClienteDireccion?.Trim()}|{cliente.ClienteTelefono}|{cliente.ClienteCorreo?.Trim()}|{cliente.ClienteEstado}|{cliente.ClienteDespacho?.Trim()}|{cliente.ClienteUsuario}";
        var result = await _accesoDatos.EjecutarComandoAsync("uspInsertarCliente", "@Data", data, cancellationToken);
        return string.IsNullOrWhiteSpace(result) ? "error" : result;
    }

    public async Task<bool> EliminarAsync(long id, CancellationToken cancellationToken = default)
    {
        const string sql = "uspEliminarCliente";
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

    public async Task<IReadOnlyList<Cliente>> ListarAsync(string? estado = "ACTIVO", int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        (page, pageSize) = NormalizePagination(page, pageSize);

        const string sql = """
            SELECT ClienteId, ClienteRazon, ClienteRuc, ClienteDni, ClienteDireccion, ClienteTelefono,
                   ClienteCorreo, ClienteEstado, ClienteDespacho, ClienteUsuario, ClienteFecha
            FROM Cliente
            WHERE (@Estado IS NULL OR ClienteEstado = @Estado)
            ORDER BY ClienteId DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@Estado", (object?)estado ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
        cmd.Parameters.AddWithValue("@PageSize", pageSize);
        await con.OpenAsync(cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

        var lista = new List<Cliente>();
        while (await reader.ReadAsync(cancellationToken))
        {
            lista.Add(new Cliente
            {
                ClienteId = Convert.ToInt64(reader["ClienteId"]),
                ClienteRazon = reader["ClienteRazon"].ToString(),
                ClienteRuc = reader["ClienteRuc"].ToString(),
                ClienteDni = reader["ClienteDni"].ToString(),
                ClienteDireccion = reader["ClienteDireccion"].ToString(),
                ClienteTelefono = reader["ClienteTelefono"].ToString(),
                ClienteCorreo = reader["ClienteCorreo"].ToString(),
                ClienteEstado = reader["ClienteEstado"].ToString(),
                ClienteDespacho = reader["ClienteDespacho"].ToString(),
                ClienteUsuario = reader["ClienteUsuario"].ToString(),
                ClienteFecha = reader["ClienteFecha"] == DBNull.Value ? null : Convert.ToDateTime(reader["ClienteFecha"])
            });
        }

        return lista;
    }

    public async Task<string> ListarComboAsync(CancellationToken cancellationToken = default)
    {
        var result = await _accesoDatos.EjecutarComandoAsync("uspListaComboClienteWeb", cancellationToken: cancellationToken);
        return string.IsNullOrWhiteSpace(result) ? string.Empty : result;
    }

    private static (int page, int pageSize) NormalizePagination(int page, int pageSize)
    {
        var normalizedPage = page < 1 ? 1 : page;
        var normalizedPageSize = pageSize < 1 ? 1 : Math.Min(pageSize, 100);
        return (normalizedPage, normalizedPageSize);
    }
}
