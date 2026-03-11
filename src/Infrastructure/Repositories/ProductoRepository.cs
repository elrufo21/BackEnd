using System.Data;
using Ecommerce.Application.Contracts.Productos;
using Ecommerce.Domain;
using Ecommerce.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class ProductoRepository : IProducto
{
    private readonly string _connectionString;
    private readonly AccesoDatos _accesoDatos;

    public ProductoRepository(IConfiguration configuration, AccesoDatos accesoDatos)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing connection string: DefaultConnection");
        _accesoDatos = accesoDatos;
    }

    public async Task<string> InsertarAsync(Producto producto, CancellationToken cancellationToken = default)
    {
        var data = $"{producto.IdProducto}|{producto.IdSubLinea}|{producto.ProductoCodigo?.Trim()}|{producto.ProductoNombre?.Trim()}|{producto.ProductoUM?.Trim()}|{Convert.ToDecimal(producto.ProductoCosto)}|{Convert.ToDecimal(producto.ProductoVenta)}|{Convert.ToDecimal(producto.ProductoVentaB)}|{Convert.ToDecimal(producto.ProductoCantidad)}|{producto.ProductoEstado}|{producto.ProductoUsuario}|{producto.ProductoImagen}|{Convert.ToDecimal(producto.ValorCritico)}|{producto.AplicaINV}";
        var result = await _accesoDatos.EjecutarComandoAsync("uspIngresarProducto", "@Data", data, cancellationToken);
        return string.IsNullOrWhiteSpace(result) ? "error" : result;
    }

    public async Task<bool> EliminarAsync(long id, CancellationToken cancellationToken = default)
    {
        const string sql = "uspEliminarProducto";
        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, con)
        {
            CommandTimeout = 300,
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.AddWithValue("@Id", id);
        await con.OpenAsync(cancellationToken);
        return await cmd.ExecuteNonQueryAsync(cancellationToken) > 0;
    }

    public async Task<Producto?> ObtenerPorIdAsync(long id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT IdProducto, IdSubLinea, ProductoCodigo, ProductoNombre, ProductoUM, ProductoCosto,
                   ProductoVenta, ProductoVentaB, ProductoCantidad, ProductoEstado, ProductoUsuario,
                   ProductoFecha, ProductoImagen, ValorCritico, AplicaINV
            FROM Producto
            WHERE IdProducto = @Id;
            """;

        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@Id", id);
        await con.OpenAsync(cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? MapProducto(reader) : null;
    }

    public async Task<string> ListarCrudRawAsync(string? estado = "ACTIVO", CancellationToken cancellationToken = default)
    {
        return estado is null
            ? await _accesoDatos.EjecutarComandoAsync("uspListarProducto", cancellationToken: cancellationToken)
            : await _accesoDatos.EjecutarComandoAsync("uspListarProducto", "@Estado", estado, cancellationToken);
    }

    public async Task<IReadOnlyList<Producto>> ListarCrudAsync(string? estado = "ACTIVO", int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var result = await ListarCrudRawAsync(estado, cancellationToken);

        var lista = string.IsNullOrWhiteSpace(result) ? new List<Producto>() : ParseProductosCrud(result);
        return ApplyPagination(lista, page, pageSize);
    }

    public async Task<IReadOnlyList<EListaProducto>> ListarAsync(int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var result = await _accesoDatos.EjecutarComandoAsync("uspListaWebProducto", cancellationToken: cancellationToken);
        var lista = string.IsNullOrWhiteSpace(result) ? new List<EListaProducto>() : Cadena.AlistaCamposPro(result);
        return ApplyPagination(lista, page, pageSize);
    }

    public async Task<IReadOnlyList<EListaProducto>> BuscarProductoAsync(string nombre, int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var result = await _accesoDatos.EjecutarComandoAsync("uspBuscaWebProducto", "@Descripcion", nombre, cancellationToken);
        var lista = string.IsNullOrWhiteSpace(result) ? new List<EListaProducto>() : Cadena.AlistaCamposPro(result);
        return ApplyPagination(lista, page, pageSize);
    }

    private static Producto MapProducto(SqlDataReader reader)
    {
        return new Producto
        {
            IdProducto = Convert.ToInt64(reader["IdProducto"]),
            IdSubLinea = reader["IdSubLinea"] == DBNull.Value ? null : Convert.ToInt64(reader["IdSubLinea"]),
            ProductoCodigo = reader["ProductoCodigo"]?.ToString(),
            ProductoNombre = reader["ProductoNombre"]?.ToString(),
            ProductoUM = reader["ProductoUM"]?.ToString(),
            ProductoCosto = reader["ProductoCosto"] == DBNull.Value ? null : Convert.ToDecimal(reader["ProductoCosto"]),
            ProductoVenta = reader["ProductoVenta"] == DBNull.Value ? null : Convert.ToDecimal(reader["ProductoVenta"]),
            ProductoVentaB = reader["ProductoVentaB"] == DBNull.Value ? null : Convert.ToDecimal(reader["ProductoVentaB"]),
            ProductoCantidad = reader["ProductoCantidad"] == DBNull.Value ? null : Convert.ToDecimal(reader["ProductoCantidad"]),
            ProductoEstado = reader["ProductoEstado"]?.ToString(),
            ProductoUsuario = reader["ProductoUsuario"]?.ToString(),
            ProductoFecha = reader["ProductoFecha"] == DBNull.Value ? null : Convert.ToDateTime(reader["ProductoFecha"]),
            ProductoImagen = reader["ProductoImagen"]?.ToString(),
            ValorCritico = reader["ValorCritico"] == DBNull.Value ? null : Convert.ToDecimal(reader["ValorCritico"]),
            AplicaINV = reader["AplicaINV"]?.ToString()
        };
    }

    private static IReadOnlyList<EListaProducto> ApplyPagination(IReadOnlyList<EListaProducto> source, int page, int pageSize)
    {
        (page, pageSize) = NormalizePagination(page, pageSize);
        return source.Skip((page - 1) * pageSize).Take(pageSize).ToList();
    }

    private static IReadOnlyList<Producto> ApplyPagination(IReadOnlyList<Producto> source, int page, int pageSize)
    {
        (page, pageSize) = NormalizePagination(page, pageSize);
        return source.Skip((page - 1) * pageSize).Take(pageSize).ToList();
    }

    private static List<Producto> ParseProductosCrud(string data)
    {
        var lista = new List<Producto>();
        var registros = data.Split('¬');

        foreach (var registro in registros)
        {
            var campos = registro.Split('|');
            if (campos.Length == 0 || campos[0] == "~")
            {
                break;
            }

            lista.Add(new Producto
            {
                IdProducto = ToLong(campos, 0),
                IdSubLinea = ToNullableLong(campos, 1),
                ProductoCodigo = ToNullableString(campos, 2),
                ProductoNombre = ToNullableString(campos, 3),
                ProductoUM = ToNullableString(campos, 4),
                ProductoCosto = ToNullableDecimal(campos, 5),
                ProductoVenta = ToNullableDecimal(campos, 6),
                ProductoVentaB = ToNullableDecimal(campos, 7),
                ProductoCantidad = ToNullableDecimal(campos, 8),
                ProductoEstado = ToNullableString(campos, 9),
                ProductoUsuario = ToNullableString(campos, 10),
                ProductoFecha = ToNullableDate(campos, 11),
                ProductoImagen = ToNullableString(campos, 12),
                ValorCritico = ToNullableDecimal(campos, 13),
                AplicaINV = ToNullableString(campos, 14)
            });
        }

        return lista;
    }

    private static string? ToNullableString(string[] campos, int index)
    {
        if (index >= campos.Length) return null;
        var value = campos[index];
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    private static long ToLong(string[] campos, int index)
    {
        var value = ToNullableString(campos, index);
        return long.TryParse(value, out var parsed) ? parsed : 0;
    }

    private static long? ToNullableLong(string[] campos, int index)
    {
        var value = ToNullableString(campos, index);
        return long.TryParse(value, out var parsed) ? parsed : null;
    }

    private static decimal? ToNullableDecimal(string[] campos, int index)
    {
        var value = ToNullableString(campos, index);
        return decimal.TryParse(value, out var parsed) ? parsed : null;
    }

    private static DateTime? ToNullableDate(string[] campos, int index)
    {
        var value = ToNullableString(campos, index);
        return DateTime.TryParse(value, out var parsed) ? parsed : null;
    }

    private static (int page, int pageSize) NormalizePagination(int page, int pageSize)
    {
        var normalizedPage = page < 1 ? 1 : page;
        var normalizedPageSize = pageSize < 1 ? 1 : Math.Min(pageSize, 100);
        return (normalizedPage, normalizedPageSize);
    }
}
