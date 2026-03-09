using System.Data;
using Ecommerce.Application.Contracts.Proveedores;
using Ecommerce.Domain;
using Ecommerce.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class CuentaProveedorRepository : ICuentaProveedor
{
    private readonly string _connectionString;
    private readonly AccesoDatos _accesoDatos;

    public CuentaProveedorRepository(IConfiguration configuration, AccesoDatos accesoDatos)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing connection string: DefaultConnection");
        _accesoDatos = accesoDatos;
    }

    public async Task<string> InsertarAsync(CuentaProveedor cuenta, CancellationToken cancellationToken = default)
    {
        var data = $"{cuenta.CuentaId}|{cuenta.ProveedorId}|{cuenta.Entidad}|{cuenta.TipoCuenta}|{cuenta.Moneda}|{cuenta.NroCuenta?.Trim()}";
        var result = await _accesoDatos.EjecutarComandoAsync("uspInsertarCuentaProveedor", "@Data", data, cancellationToken);
        return string.IsNullOrWhiteSpace(result) ? "error" : result;
    }

    public async Task<bool> EliminarAsync(long cuentaId, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM CuentaProveedor WHERE CuentaId = @CuentaId";
        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, con)
        {
            CommandTimeout = 300,
            CommandType = CommandType.Text
        };
        cmd.Parameters.AddWithValue("@CuentaId", cuentaId);
        await con.OpenAsync(cancellationToken);
        return await cmd.ExecuteNonQueryAsync(cancellationToken) > 0;
    }

    public async Task<IReadOnlyList<CuentaProveedor>> ListarPorProveedorAsync(long proveedorId, int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        (page, pageSize) = NormalizePagination(page, pageSize);

        const string sql = """
            SELECT CuentaId, ProveedorId, Entidad, TipoCuenta, Moneda, NroCuenta
            FROM CuentaProveedor
            WHERE ProveedorId = @ProveedorId
            ORDER BY CuentaId DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, con)
        {
            CommandTimeout = 300,
            CommandType = CommandType.Text
        };
        cmd.Parameters.AddWithValue("@ProveedorId", proveedorId);
        cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
        cmd.Parameters.AddWithValue("@PageSize", pageSize);
        await con.OpenAsync(cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

        var lista = new List<CuentaProveedor>();
        while (await reader.ReadAsync(cancellationToken))
        {
            lista.Add(MapCuenta(reader));
        }

        return lista;
    }

    private static CuentaProveedor MapCuenta(SqlDataReader reader)
    {
        return new CuentaProveedor
        {
            CuentaId = Convert.ToInt64(reader["CuentaId"]),
            ProveedorId = Convert.ToInt64(reader["ProveedorId"]),
            Entidad = reader["Entidad"]?.ToString(),
            TipoCuenta = reader["TipoCuenta"]?.ToString(),
            Moneda = reader["Moneda"]?.ToString(),
            NroCuenta = reader["NroCuenta"]?.ToString()
        };
    }

    private static (int page, int pageSize) NormalizePagination(int page, int pageSize)
    {
        var normalizedPage = page < 1 ? 1 : page;
        var normalizedPageSize = pageSize < 1 ? 1 : Math.Min(pageSize, 100);
        return (normalizedPage, normalizedPageSize);
    }
}
