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
    AccesoDatos daSQL = new AccesoDatos();
    public PersonalRepository()
    {
        var builder = WebApplication.CreateBuilder();
        _connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    }
    public string Insertar(Personal personal)
    {
        string rpt = string.Empty;
        string xvalue = string.Empty;
        xvalue = personal.PersonalId + "|" + personal.PersonalNombres?.Trim() + "|" +
        personal.PersonalApellidos?.Trim() + "|" + personal.AreaId + "|" + personal.PersonalCodigo?.Trim() + "|" +
        personal.PersonalNacimiento?.Date.ToString("MM-dd-yyyy") + "|" + personal.PersonalIngreso?.Date.ToString("MM-dd-yyyy") + "|" +
        personal.PersonalDNI?.Trim() + "|" + personal.PersonalDireccion?.Trim() + "|" + 
        personal.PersonalTelefono?.Trim() + "|" + personal.PersonalEmail?.Trim() + "|" +personal.PersonalEstado + "|" +
         personal.PersonalImagen + "|" + personal.CompaniaId;
        rpt = daSQL.ejecutarComando("uspIngresarPersonal", "@Data", xvalue);
        if (string.IsNullOrEmpty(rpt)) rpt = "error";
        return rpt;
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
                PersonalIngreso = ReadNullableDate(reader, "PersonalIngreso"),
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
        if (DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var exact))
        {
            return exact;
        }

        // Fallback to broader parse (e.g., includes time parts)
        return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed)
            ? parsed
            : (DateTime?)null;
    }
}
