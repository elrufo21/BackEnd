using System.Data;
using Ecommerce.Application.Contracts.Clientes;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class ClienteRepository : ICliente
{
    private readonly string _connectionString;
    private readonly AccesoDatos _accesoDatos = new();

    public ClienteRepository()
    {
        var builder = WebApplication.CreateBuilder();
        _connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    }

    public bool Insertar(Cliente cliente)
    {
        const string sql = "uspInsertarCliente";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameters(cmd, cliente);
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        var rows = cmd.ExecuteNonQuery();
        return rows > 0;
    }
    
    public bool Eliminar(long id)
    {
        const string sql = "uspEliminarCliente";
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

    public IReadOnlyList<Cliente> Listar()
    {
        var lista = new List<Cliente>();
        const string sql = "usppListarClientes";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.StoredProcedure;
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
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

    public string ListarCombo()
    {
        var rpt = _accesoDatos.ejecutarComando("uspListaComboClienteWeb");
        return string.IsNullOrWhiteSpace(rpt) ? string.Empty : rpt;
    }

    private static void AddParameters(SqlCommand cmd, Cliente cliente)
    {
        cmd.Parameters.AddWithValue("@Id", (object?)cliente.ClienteId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ClienteRazon", (object?)cliente.ClienteRazon ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ClienteRuc", (object?)cliente.ClienteRuc ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ClienteDni", (object?)cliente.ClienteDni ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ClienteDireccion", (object?)cliente.ClienteDireccion ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ClienteTelefono", (object?)cliente.ClienteTelefono ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ClienteCorreo", (object?)cliente.ClienteCorreo ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ClienteEstado", (object?)cliente.ClienteEstado ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ClienteDespacho", (object?)cliente.ClienteDespacho ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ClienteUsuario", (object?)cliente.ClienteUsuario ?? DBNull.Value);
    }
}
