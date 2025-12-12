using System.Data;
using Ecommerce.Application.Contracts.Personales;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class PersonalRepository : IPersonal
{
    private readonly string _connectionString;
    public PersonalRepository()
    {
        var builder = WebApplication.CreateBuilder();
        _connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    }
    public bool Insertar(Personal personal)
    {
        const string sql = "ingresarPersonal";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameters(cmd, personal);
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        var rows = cmd.ExecuteNonQuery();
        return rows > 0;
    }
    public bool Editar(long id, Personal personal)
    {
        const string sql = "editarPersonal";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Id", id);
        AddParameters(cmd, personal);
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        var rows = cmd.ExecuteNonQuery();
        return rows > 0;
    }
    public bool Eliminar(long id)
    {
        const string sql = "uspEliminarPersonal";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.StoredProcedure;
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        var rows = cmd.ExecuteNonQuery();
        return rows > 0;
    }
    public IReadOnlyList<Personal> Listar()
    {
        var lista = new List<Personal>();
        const string sql = "listarPersonal";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.StoredProcedure;
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(new Personal
            {
                PersonalId = Convert.ToInt64(reader["PersonalId"]),
                PersonalNombres = reader["PersonalNombres"].ToString(),
                PersonalApellidos = reader["PersonalApellidos"].ToString(),
                AreaId = reader["AreaId"] == DBNull.Value ? null : Convert.ToInt64(reader["AreaId"]),
                PersonalCodigo = reader["PersonalCodigo"].ToString(),
                PersonalNacimiento = ReadNullableDate(reader, "PersonalNacimiento"),
                PersonalIngreso = reader["PersonalIngreso"].ToString(),
                PersonalDNI = reader["PersonalDNI"].ToString(),
                PersonalDireccion = reader["PersonalDireccion"].ToString(),
                PersonalTelefono = reader["PersonalTelefono"].ToString(),
                PersonalEmail = reader["PersonalEmail"].ToString(),
                PersonalEstado = reader["PersonalEstado"].ToString(),
                PersonalImagen = reader["PersonalImagen"].ToString(),
                CompaniaId = reader["CompaniaId"] == DBNull.Value ? null : Convert.ToInt32(reader["CompaniaId"])
            });
        }
        return lista;
    }
    private static void AddParameters(SqlCommand cmd, Personal personal)
    {
        cmd.Parameters.AddWithValue("@PersonalNombres", (object?)personal.PersonalNombres ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PersonalApellidos", (object?)personal.PersonalApellidos ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@AreaId", (object?)personal.AreaId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PersonalCodigo", (object?)personal.PersonalCodigo ?? DBNull.Value);
        cmd.Parameters.Add("@PersonalNacimiento", SqlDbType.Date).Value = personal.PersonalNacimiento?.Date ?? (object)DBNull.Value;
        cmd.Parameters.AddWithValue("@PersonalIngreso", (object?)personal.PersonalIngreso ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PersonalDNI", (object?)personal.PersonalDNI ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PersonalDireccion", (object?)personal.PersonalDireccion ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PersonalTelefono", (object?)personal.PersonalTelefono ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PersonalEmail", (object?)personal.PersonalEmail ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PersonalEstado", (object?)personal.PersonalEstado ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PersonalImagen", (object?)personal.PersonalImagen ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CompaniaId", (object?)personal.CompaniaId ?? DBNull.Value);
    }

    private static DateTime? ReadNullableDate(SqlDataReader reader, string columnName)
    {
        if (reader[columnName] == DBNull.Value) return null;

        // If the value is already a DateTime, return it directly.
        if (reader[columnName] is DateTime dtValue)
        {
            return dtValue;
        }

        var value = reader[columnName]?.ToString();
        if (string.IsNullOrWhiteSpace(value)) return null;

        // First try ISO date (yyyy-MM-dd)
        if (DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var exact))
        {
            return exact;
        }

        // Fallback to broader parse (e.g., includes time parts)
        return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed)
            ? parsed
            : (DateTime?)null;
    }
}
