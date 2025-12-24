using System.Data;
using Ecommerce.Application.Contracts.Proveedores;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class CuentaProveedorRepository : ICuentaProveedor
{
    private readonly string _connectionString;
    AccesoDatos daSQL = new AccesoDatos();
    public CuentaProveedorRepository()
    {
        var builder = WebApplication.CreateBuilder();
        _connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    }

    public string Insertar(CuentaProveedor cuenta)
    {
        string rpt = string.Empty;
        string xvalue = string.Empty;
        xvalue = cuenta.CuentaId + "|" +cuenta.ProveedorId +"|" +
        cuenta.Entidad+ "|" + cuenta.TipoCuenta + "|" + 
        cuenta.Moneda + "|" +cuenta.NroCuenta?.Trim();
        rpt = daSQL.ejecutarComando("uspInsertarCuentaProveedor", "@Data", xvalue);
        if (string.IsNullOrEmpty(rpt)) rpt = "error";
        return rpt;
    }

    public bool Eliminar(long cuentaId)
    {
        const string sql = "DELETE FROM CuentaProveedor WHERE CuentaId = @CuentaId";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("@CuentaId", cuentaId);
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        return cmd.ExecuteNonQuery() > 0;
    }

    public IReadOnlyList<CuentaProveedor> ListarPorProveedor(long proveedorId)
    {
        var lista = new List<CuentaProveedor>();
        const string sql = @"SELECT CuentaId, ProveedorId, Entidad, TipoCuenta, Moneda, NroCuenta
        FROM CuentaProveedor WITH (NOLOCK)
        WHERE ProveedorId = @ProveedorId
        ORDER BY CuentaId DESC;";

        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("@ProveedorId", proveedorId);
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
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
}
