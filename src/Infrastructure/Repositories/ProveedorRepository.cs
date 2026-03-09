using System.Data;
using Ecommerce.Application.Contracts.Proveedores;
using Ecommerce.Domain;
using Ecommerce.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class ProveedorRepository : IProveedor
{
    private readonly string _connectionString;
    private readonly AccesoDatos _accesoDatos;

    public ProveedorRepository(IConfiguration configuration, AccesoDatos accesoDatos)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing connection string: DefaultConnection");
        _accesoDatos = accesoDatos;
    }

    public async Task<string> InsertarAsync(Proveedor proveedor, CancellationToken cancellationToken = default)
    {
        var data = $"{proveedor.ProveedorId}|{proveedor.ProveedorRazon?.Trim()}|{proveedor.ProveedorRuc?.Trim()}|{proveedor.ProveedorContacto?.Trim()}|{proveedor.ProveedorCelular?.Trim()}|{proveedor.ProveedorTelefono?.Trim()}|{proveedor.ProveedorCorreo?.Trim()}|{proveedor.ProveedorDireccion?.Trim()}|{proveedor.ProveedorEstado}";
        var result = await _accesoDatos.EjecutarComandoAsync("uspInsertarProveedor", "@Data", data, cancellationToken);
        return string.IsNullOrWhiteSpace(result) ? "error" : result;
    }

    public async Task<bool> EliminarAsync(long id, CancellationToken cancellationToken = default)
    {
        const string sql = "uspEliminarProveedor";
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

    public async Task<Proveedor?> ObtenerPorIdAsync(long id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT ProveedorId, ProveedorRazon, ProveedorRuc, ProveedorContacto, ProveedorCelular,
                   ProveedorTelefono, ProveedorCorreo, ProveedorDireccion, ProveedorEstado
            FROM Proveedor
            WHERE ProveedorId = @Id;
            """;

        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@Id", id);
        await con.OpenAsync(cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? MapProveedor(reader) : null;
    }

    public async Task<IReadOnlyList<Proveedor>> ListarAsync(string? estado = "ACTIVO", int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        (page, pageSize) = NormalizePagination(page, pageSize);

        const string sql = """
            SELECT ProveedorId, ProveedorRazon, ProveedorRuc, ProveedorContacto, ProveedorCelular,
                   ProveedorTelefono, ProveedorCorreo, ProveedorDireccion, ProveedorEstado
            FROM Proveedor
            WHERE (@Estado IS NULL OR ProveedorEstado = @Estado)
            ORDER BY ProveedorId DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@Estado", (object?)estado ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
        cmd.Parameters.AddWithValue("@PageSize", pageSize);
        await con.OpenAsync(cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

        var lista = new List<Proveedor>();
        while (await reader.ReadAsync(cancellationToken))
        {
            lista.Add(MapProveedor(reader));
        }

        return lista;
    }

    private static Proveedor MapProveedor(SqlDataReader reader)
    {
        return new Proveedor
        {
            ProveedorId = Convert.ToInt64(reader["ProveedorId"]),
            ProveedorRazon = reader["ProveedorRazon"]?.ToString(),
            ProveedorRuc = reader["ProveedorRuc"]?.ToString(),
            ProveedorContacto = reader["ProveedorContacto"]?.ToString(),
            ProveedorCelular = reader["ProveedorCelular"]?.ToString(),
            ProveedorTelefono = reader["ProveedorTelefono"]?.ToString(),
            ProveedorCorreo = reader["ProveedorCorreo"]?.ToString(),
            ProveedorDireccion = reader["ProveedorDireccion"]?.ToString(),
            ProveedorEstado = reader["ProveedorEstado"]?.ToString()
        };
    }

    private static (int page, int pageSize) NormalizePagination(int page, int pageSize)
    {
        var normalizedPage = page < 1 ? 1 : page;
        var normalizedPageSize = pageSize < 1 ? 1 : Math.Min(pageSize, 100);
        return (normalizedPage, normalizedPageSize);
    }
}
