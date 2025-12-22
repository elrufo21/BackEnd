using System.Data;
using Ecommerce.Application.Contracts.Proveedores;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class ProveedorRepository : IProveedor
{
    private readonly string _connectionString;
    AccesoDatos daSQL = new AccesoDatos();
    public ProveedorRepository()
    {
        var builder = WebApplication.CreateBuilder();
        _connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    }

    public string Insertar(Proveedor proveedor)
    {
        string rpt = string.Empty;
        string xvalue = string.Empty;
        xvalue = proveedor.ProveedorId + "|" + proveedor.ProveedorRazon?.Trim() + "|" +
        proveedor.ProveedorRuc?.Trim() + "|" + proveedor.ProveedorContacto?.Trim() + "|" + 
        proveedor.ProveedorCelular?.Trim() + "|" +proveedor.ProveedorTelefono+ "|" +
        proveedor.ProveedorCorreo?.Trim()+ "|" +proveedor.ProveedorDireccion+ "|" +
        proveedor.ProveedorEstado;
        rpt = daSQL.ejecutarComando("uspInsertarProducto", "@Data", xvalue);
        if (string.IsNullOrEmpty(rpt)) rpt = "error";
        return rpt;
    }

    public bool Actualizar(Proveedor proveedor)
    {
        const string sql = @"
UPDATE Proveedor
SET ProveedorRazon = @ProveedorRazon,
    ProveedorRuc = @ProveedorRuc,
    ProveedorContacto = @ProveedorContacto,
    ProveedorCelular = @ProveedorCelular,
    ProveedorTelefono = @ProveedorTelefono,
    ProveedorCorreo = @ProveedorCorreo,
    ProveedorDireccion = @ProveedorDireccion,
    ProveedorEstado = @ProveedorEstado
WHERE ProveedorId = @ProveedorId;";

        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("@ProveedorId", proveedor.ProveedorId);
        AddParameters(cmd, proveedor);
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        return cmd.ExecuteNonQuery() > 0;
    }
    public bool Eliminar(long id)
    {
        const string sql = "DELETE FROM Proveedor WHERE ProveedorId = @Id;";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("@Id", id);
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        return cmd.ExecuteNonQuery() > 0;
    }

    public Proveedor? ObtenerPorId(long id)
    {
        const string sql = @"
SELECT TOP 1
    ProveedorId,
    ProveedorRazon,
    ProveedorRuc,
    ProveedorContacto,
    ProveedorCelular,
    ProveedorTelefono,
    ProveedorCorreo,
    ProveedorDireccion,
    ProveedorEstado
FROM Proveedor WITH (NOLOCK)
WHERE ProveedorId = @Id;";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("@Id", id);
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? MapProveedor(reader) : null;
    }

    public IReadOnlyList<Proveedor> Listar()
    {
        var lista = new List<Proveedor>();
        const string sql = @"
SELECT
    ProveedorId,
    ProveedorRazon,
    ProveedorRuc,
    ProveedorContacto,
    ProveedorCelular,
    ProveedorTelefono,
    ProveedorCorreo,
    ProveedorDireccion,
    ProveedorEstado
FROM Proveedor WITH (NOLOCK);";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.Text;
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(MapProveedor(reader));
        }

        return lista;
    }

    private static void AddParameters(SqlCommand cmd, Proveedor proveedor)
    {
        cmd.Parameters.AddWithValue("@ProveedorRazon", (object?)proveedor.ProveedorRazon ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ProveedorRuc", (object?)proveedor.ProveedorRuc ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ProveedorContacto", (object?)proveedor.ProveedorContacto ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ProveedorCelular", (object?)proveedor.ProveedorCelular ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ProveedorTelefono", (object?)proveedor.ProveedorTelefono ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ProveedorCorreo", (object?)proveedor.ProveedorCorreo ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ProveedorDireccion", (object?)proveedor.ProveedorDireccion ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ProveedorEstado", (object?)proveedor.ProveedorEstado ?? DBNull.Value);
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
}
