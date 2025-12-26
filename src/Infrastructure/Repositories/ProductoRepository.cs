using System.Data;
using Ecommerce.Application.Contracts.Productos;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class ProductoRepository : IProducto
{
    private readonly string _connectionString;
    AccesoDatos daSQL = new AccesoDatos();
    private readonly AccesoDatos _accesoDatos = new();

    public ProductoRepository()
    {
        var builder = WebApplication.CreateBuilder();
        _connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    }

    public string Insertar(Producto producto)
    {
        string rpt = string.Empty;
        string xvalue = string.Empty;
        xvalue = producto.IdProducto + "|" + producto.IdSubLinea + "|" +
        producto.ProductoCodigo?.Trim() + "|" + producto.ProductoNombre?.Trim() + "|" + 
        producto.ProductoUM?.Trim() + "|" +Convert.ToDecimal(producto.ProductoCosto)+ "|" +Convert.ToDecimal(producto.ProductoVenta)+ "|" +
        Convert.ToDecimal(producto.ProductoVentaB) + "|" + Convert.ToDecimal(producto.ProductoCantidad) + "|" +
        producto.ProductoEstado + "|" +producto.ProductoUsuario+ "|" +
        producto.ProductoImagen+ "|" + Convert.ToDecimal(producto.ValorCritico) + "|" +producto.AplicaINV;
        rpt = daSQL.ejecutarComando("uspIngresarProducto", "@Data", xvalue);
        if (string.IsNullOrEmpty(rpt)) rpt = "error";
        return rpt;
    }

    public bool Eliminar(long id)
    {
        const string sql = "uspEliminarProducto";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Id", id);
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        return cmd.ExecuteNonQuery() > 0;
    }

    public Producto? ObtenerPorId(long id)
    {
        const string sql = "uspObtenerProductoPorId";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Id", id);
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? MapProducto(reader) : null;
    }

    public IReadOnlyList<Producto> ListarCrud(string? estado = "ACTIVO")
    {
        var lista = new List<Producto>();
        const string sql = "uspListarProducto";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Estado", (object?)estado ?? DBNull.Value);
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(MapProducto(reader));
        }
        return lista;
    }

    public IReadOnlyList<EListaProducto> Listar()
    {
        var lista = new List<EListaProducto>();
        var rpt = _accesoDatos.ejecutarComando("uspListaWebProducto");
        if (!string.IsNullOrEmpty(rpt))
        {
            lista = Cadena.AlistaCamposPro(rpt);
        }
        return lista;
    }

    public IReadOnlyList<EListaProducto> BuscarProducto(string nombre)
    {
        var lista = new List<EListaProducto>();
        var rpt = _accesoDatos.ejecutarComando("uspBuscaWebProducto", "@Descripcion", nombre);
        if (!string.IsNullOrEmpty(rpt))
        {
            lista = Cadena.AlistaCamposPro(rpt);
        }
        return lista;
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
}
