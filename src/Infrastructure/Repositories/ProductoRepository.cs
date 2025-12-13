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
    private readonly AccesoDatos _accesoDatos = new();

    public ProductoRepository()
    {
        var builder = WebApplication.CreateBuilder();
        _connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    }

    public bool Insertar(Producto producto)
    {
        const string sql = @"
INSERT INTO Producto (
    IdSubLinea,
    ProductoCodigo,
    ProductoNombre,
    ProductoTipoCambio,
    ProductoCostoDolar,
    ProductoUM,
    ProductoCosto,
    ProductoVenta,
    ProductoVentaB,
    ProductoCantidad,
    ProductoObs,
    ProductoEstado,
    ProductoUsuario,
    ProductoFecha,
    ProductoImagen,
    ValorCritico,
    AplicaTC,
    FechaVencimiento,
    AplicaFechaV,
    AplicaINV,
    CantidadANT,
    FechaModCant
) VALUES (
    @IdSubLinea,
    @ProductoCodigo,
    @ProductoNombre,
    @ProductoTipoCambio,
    @ProductoCostoDolar,
    @ProductoUM,
    @ProductoCosto,
    @ProductoVenta,
    @ProductoVentaB,
    @ProductoCantidad,
    @ProductoObs,
    @ProductoEstado,
    @ProductoUsuario,
    @ProductoFecha,
    @ProductoImagen,
    @ValorCritico,
    @AplicaTC,
    @FechaVencimiento,
    @AplicaFechaV,
    @AplicaINV,
    @CantidadANT,
    @FechaModCant
);";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.Text;
        AddParameters(cmd, producto);
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        return cmd.ExecuteNonQuery() > 0;
    }

    public bool Editar(long id, Producto producto)
    {
        const string sql = @"
UPDATE Producto SET
    IdSubLinea = @IdSubLinea,
    ProductoCodigo = @ProductoCodigo,
    ProductoNombre = @ProductoNombre,
    ProductoTipoCambio = @ProductoTipoCambio,
    ProductoCostoDolar = @ProductoCostoDolar,
    ProductoUM = @ProductoUM,
    ProductoCosto = @ProductoCosto,
    ProductoVenta = @ProductoVenta,
    ProductoVentaB = @ProductoVentaB,
    ProductoCantidad = @ProductoCantidad,
    ProductoObs = @ProductoObs,
    ProductoEstado = @ProductoEstado,
    ProductoUsuario = @ProductoUsuario,
    ProductoFecha = @ProductoFecha,
    ProductoImagen = @ProductoImagen,
    ValorCritico = @ValorCritico,
    AplicaTC = @AplicaTC,
    FechaVencimiento = @FechaVencimiento,
    AplicaFechaV = @AplicaFechaV,
    AplicaINV = @AplicaINV,
    CantidadANT = @CantidadANT,
    FechaModCant = @FechaModCant
WHERE IdProducto = @Id;";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("@Id", id);
        AddParameters(cmd, producto);
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        return cmd.ExecuteNonQuery() > 0;
    }

    public bool Eliminar(long id)
    {
        const string sql = "DELETE FROM Producto WHERE IdProducto = @Id;";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("@Id", id);
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        return cmd.ExecuteNonQuery() > 0;
    }

    public Producto? ObtenerPorId(long id)
    {
        const string sql = @"
SELECT TOP 1
    IdProducto,
    IdSubLinea,
    ProductoCodigo,
    ProductoNombre,
    ProductoTipoCambio,
    ProductoCostoDolar,
    ProductoUM,
    ProductoCosto,
    ProductoVenta,
    ProductoVentaB,
    ProductoCantidad,
    ProductoObs,
    ProductoEstado,
    ProductoUsuario,
    ProductoFecha,
    ProductoImagen,
    ValorCritico,
    AplicaTC,
    FechaVencimiento,
    AplicaFechaV,
    AplicaINV,
    CantidadANT,
    FechaModCant
FROM Producto WITH (NOLOCK)
WHERE IdProducto = @Id;";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("@Id", id);
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? MapProducto(reader) : null;
    }

    public IReadOnlyList<Producto> ListarCrud()
    {
        var lista = new List<Producto>();
        const string sql = @"
SELECT
    IdProducto,
    IdSubLinea,
    ProductoCodigo,
    ProductoNombre,
    ProductoTipoCambio,
    ProductoCostoDolar,
    ProductoUM,
    ProductoCosto,
    ProductoVenta,
    ProductoVentaB,
    ProductoCantidad,
    ProductoObs,
    ProductoEstado,
    ProductoUsuario,
    ProductoFecha,
    ProductoImagen,
    ValorCritico,
    AplicaTC,
    FechaVencimiento,
    AplicaFechaV,
    AplicaINV,
    CantidadANT,
    FechaModCant
FROM Producto WITH (NOLOCK);";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.Text;
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

    private static void AddParameters(SqlCommand cmd, Producto producto)
    {
        cmd.Parameters.AddWithValue("@IdSubLinea", (object?)producto.IdSubLinea ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ProductoCodigo", (object?)producto.ProductoCodigo ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ProductoNombre", (object?)producto.ProductoNombre ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ProductoTipoCambio", (object?)producto.ProductoTipoCambio ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ProductoCostoDolar", (object?)producto.ProductoCostoDolar ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ProductoUM", (object?)producto.ProductoUM ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ProductoCosto", (object?)producto.ProductoCosto ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ProductoVenta", (object?)producto.ProductoVenta ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ProductoVentaB", (object?)producto.ProductoVentaB ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ProductoCantidad", (object?)producto.ProductoCantidad ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ProductoObs", (object?)producto.ProductoObs ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ProductoEstado", (object?)producto.ProductoEstado ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ProductoUsuario", (object?)producto.ProductoUsuario ?? DBNull.Value);
        cmd.Parameters.Add("@ProductoFecha", SqlDbType.DateTime).Value = (object?)producto.ProductoFecha ?? DBNull.Value;
        cmd.Parameters.AddWithValue("@ProductoImagen", (object?)producto.ProductoImagen ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ValorCritico", (object?)producto.ValorCritico ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@AplicaTC", (object?)producto.AplicaTC ?? DBNull.Value);
        cmd.Parameters.Add("@FechaVencimiento", SqlDbType.Date).Value = (object?)producto.FechaVencimiento ?? DBNull.Value;
        cmd.Parameters.AddWithValue("@AplicaFechaV", (object?)producto.AplicaFechaV ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@AplicaINV", (object?)producto.AplicaINV ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CantidadANT", (object?)producto.CantidadANT ?? DBNull.Value);
        cmd.Parameters.Add("@FechaModCant", SqlDbType.DateTime).Value = (object?)producto.FechaModCant ?? DBNull.Value;
    }

    private static Producto MapProducto(SqlDataReader reader)
    {
        return new Producto
        {
            IdProducto = Convert.ToInt64(reader["IdProducto"]),
            IdSubLinea = reader["IdSubLinea"] == DBNull.Value ? null : Convert.ToInt64(reader["IdSubLinea"]),
            ProductoCodigo = reader["ProductoCodigo"]?.ToString(),
            ProductoNombre = reader["ProductoNombre"]?.ToString(),
            ProductoTipoCambio = reader["ProductoTipoCambio"] == DBNull.Value ? null : Convert.ToDecimal(reader["ProductoTipoCambio"]),
            ProductoCostoDolar = reader["ProductoCostoDolar"] == DBNull.Value ? null : Convert.ToDecimal(reader["ProductoCostoDolar"]),
            ProductoUM = reader["ProductoUM"]?.ToString(),
            ProductoCosto = reader["ProductoCosto"] == DBNull.Value ? null : Convert.ToDecimal(reader["ProductoCosto"]),
            ProductoVenta = reader["ProductoVenta"] == DBNull.Value ? null : Convert.ToDecimal(reader["ProductoVenta"]),
            ProductoVentaB = reader["ProductoVentaB"] == DBNull.Value ? null : Convert.ToDecimal(reader["ProductoVentaB"]),
            ProductoCantidad = reader["ProductoCantidad"] == DBNull.Value ? null : Convert.ToDecimal(reader["ProductoCantidad"]),
            ProductoObs = reader["ProductoObs"]?.ToString(),
            ProductoEstado = reader["ProductoEstado"]?.ToString(),
            ProductoUsuario = reader["ProductoUsuario"]?.ToString(),
            ProductoFecha = reader["ProductoFecha"] == DBNull.Value ? null : Convert.ToDateTime(reader["ProductoFecha"]),
            ProductoImagen = reader["ProductoImagen"]?.ToString(),
            ValorCritico = reader["ValorCritico"] == DBNull.Value ? null : Convert.ToDecimal(reader["ValorCritico"]),
            AplicaTC = reader["AplicaTC"]?.ToString(),
            FechaVencimiento = reader["FechaVencimiento"] == DBNull.Value ? null : Convert.ToDateTime(reader["FechaVencimiento"]),
            AplicaFechaV = reader["AplicaFechaV"] == DBNull.Value ? null : Convert.ToBoolean(reader["AplicaFechaV"]),
            AplicaINV = reader["AplicaINV"]?.ToString(),
            CantidadANT = reader["CantidadANT"] == DBNull.Value ? null : Convert.ToDecimal(reader["CantidadANT"]),
            FechaModCant = reader["FechaModCant"] == DBNull.Value ? null : Convert.ToDateTime(reader["FechaModCant"])
        };
    }
}
