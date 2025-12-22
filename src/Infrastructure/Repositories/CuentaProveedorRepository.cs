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

    public CuentaProveedorRepository()
    {
        var builder = WebApplication.CreateBuilder();
        _connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    }

    public long Insertar(CuentaProveedor cuenta)
    {
        const string sql = @"
INSERT INTO CuentaProveedor (ProveedorId, Entidad, TipoCuenta, Moneda, NroCuenta)
VALUES (@ProveedorId, @Entidad, @TipoCuenta, @Moneda, @NroCuenta);
SELECT CAST(SCOPE_IDENTITY() AS bigint);";

        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.Text;
        AddParameters(cmd, cuenta);
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        var result = cmd.ExecuteScalar();
        return result is null ? 0 : Convert.ToInt64(result);
    }

    public bool Actualizar(long proveedorId, long cuentaId, CuentaProveedor cuenta)
    {
        const string sql = @"
UPDATE CuentaProveedor
SET Entidad = @Entidad,
    TipoCuenta = @TipoCuenta,
    Moneda = @Moneda,
    NroCuenta = @NroCuenta
WHERE CuentaId = @CuentaId AND ProveedorId = @ProveedorId;";

        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.Text;
        AddParameters(cmd, cuenta);
        cmd.Parameters.AddWithValue("@CuentaId", cuentaId);
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        return cmd.ExecuteNonQuery() > 0;
    }

    public bool Eliminar(long proveedorId, long cuentaId)
    {
        const string sql = "DELETE FROM CuentaProveedor WHERE CuentaId = @CuentaId AND ProveedorId = @ProveedorId;";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("@CuentaId", cuentaId);
        cmd.Parameters.AddWithValue("@ProveedorId", proveedorId);
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        return cmd.ExecuteNonQuery() > 0;
    }

    public IReadOnlyList<CuentaProveedor> ListarPorProveedor(long proveedorId)
    {
        var lista = new List<CuentaProveedor>();
        const string sql = @"
SELECT CuentaId, ProveedorId, Entidad, TipoCuenta, Moneda, NroCuenta
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

    private static void AddParameters(SqlCommand cmd, CuentaProveedor cuenta)
    {
        cmd.Parameters.AddWithValue("@ProveedorId", cuenta.ProveedorId);
        cmd.Parameters.AddWithValue("@Entidad", (object?)cuenta.Entidad ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@TipoCuenta", (object?)cuenta.TipoCuenta ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Moneda", (object?)cuenta.Moneda ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@NroCuenta", (object?)cuenta.NroCuenta ?? DBNull.Value);
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
