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
        proveedor.ProveedorCelular?.Trim() + "|" + proveedor.ProveedorTelefono?.Trim() + "|" +
        proveedor.ProveedorCorreo?.Trim() + "|" + proveedor.ProveedorDireccion?.Trim() + "|" +
        proveedor.ProveedorEstado;
        rpt = daSQL.ejecutarComando("uspInsertarProveedor", "@Data", xvalue);
        if (string.IsNullOrEmpty(rpt)) rpt = "error";
        return rpt;
    }

    public bool Eliminar(long id)
    {
        const string sql = "uspEliminarProveedor";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Id", id);
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        var rows = cmd.ExecuteNonQuery();
        return rows > 0;
    }

    public Proveedor? ObtenerPorId(long id)
    {
        const string sql ="uspObtenerProveedorPorId";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Id", id);
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? MapProveedor(reader) : null;
    }

    public IReadOnlyList<Proveedor> Listar()
    {
        var lista = new List<Proveedor>();
        const string sql ="uspListarProveedor";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.StoredProcedure;
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
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
}
