using System.Data;
using Ecommerce.Application.Contracts.Usuarios;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class UsuariosCrudRepository : IUsuariosCrud
{
    private readonly string _connectionString;

    public UsuariosCrudRepository()
    {
        var builder = WebApplication.CreateBuilder();
        _connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    }

    public int Insertar(UsuarioBd usuario)
    {
        const string sql = @"INSERT INTO Usuarios (
                                PersonalId,
                                UsuarioAlias,
                                UsuarioClave,
                                UsuarioFechaReg,
                                UsuarioEstado)
                              VALUES (
                                @PersonalId,
                                @UsuarioAlias,
                                @UsuarioClave,
                                @UsuarioFechaReg,
                                @UsuarioEstado);
                              SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        AddParameters(cmd, usuario);
        con.Open();
        var result = cmd.ExecuteScalar();
        return result == null ? 0 : Convert.ToInt32(result);
    }

    public bool Editar(int id, UsuarioBd usuario)
    {
        const string sql = @"UPDATE Usuarios SET
                                PersonalId = @PersonalId,
                                UsuarioAlias = @UsuarioAlias,
                                UsuarioClave = @UsuarioClave,
                                UsuarioFechaReg = @UsuarioFechaReg,
                                UsuarioEstado = @UsuarioEstado
                             WHERE UsuarioID = @UsuarioID";

        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        AddParameters(cmd, usuario);
        cmd.Parameters.AddWithValue("@UsuarioID", id);
        con.Open();
        var rows = cmd.ExecuteNonQuery();
        return rows > 0;
    }

    public bool Eliminar(int id)
    {
        const string sql = "DELETE FROM Usuarios WHERE UsuarioID = @UsuarioID";

        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@UsuarioID", id);
        con.Open();
        var rows = cmd.ExecuteNonQuery();
        return rows > 0;
    }

    public IReadOnlyList<UsuarioBd> Listar()
    {
        const string sql ="ListarUsuario";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.StoredProcedure;
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        using var reader = cmd.ExecuteReader();
        var lista = new List<UsuarioBd>();
        while (reader.Read())
        {
            lista.Add(Map(reader));
        }
        return lista;
    }

    public UsuarioBd? ObtenerPorId(int id)
    {
        const string sql = @"SELECT UsuarioID,
                                    PersonalId,
                                    UsuarioAlias,
                                    UsuarioClave,
                                    UsuarioFechaReg,
                                    UsuarioEstado
                             FROM Usuarios
                             WHERE UsuarioID = @UsuarioID";

        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@UsuarioID", id);
        con.Open();
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? Map(reader) : null;
    }

    public IReadOnlyList<UsuarioConPersonal> ListarConPersonal()
    {
        const string sql = @"SELECT U.UsuarioID,
                                    U.PersonalId,
                                    U.UsuarioAlias,
                                    U.UsuarioClave,
                                    U.UsuarioFechaReg,
                                    U.UsuarioEstado,
                                    P.PersonalId AS PersonalIdPersonal,
                                    P.PersonalNombres,
                                    P.PersonalApellidos,
                                    P.AreaId,
                                    P.PersonalCodigo,
                                    P.PersonalNacimiento,
                                    P.PersonalIngreso,
                                    P.PersonalDNI,
                                    P.PersonalDireccion,
                                    P.PersonalTelefono,
                                    P.PersonalEmail,
                                    P.PersonalEstado,
                                    P.PersonalImagen,
                                    P.CompaniaId
                             FROM Usuarios U
                             LEFT JOIN Personal P ON U.PersonalId = P.PersonalId";

        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        con.Open();
        using var reader = cmd.ExecuteReader();
        var lista = new List<UsuarioConPersonal>();
        while (reader.Read())
        {
            lista.Add(MapWithPersonal(reader));
        }
        return lista;
    }

    public UsuarioConPersonal? ObtenerPorIdConPersonal(int id)
    {
        const string sql = @"SELECT U.UsuarioID,
                                    U.PersonalId,
                                    U.UsuarioAlias,
                                    U.UsuarioClave,
                                    U.UsuarioFechaReg,
                                    U.UsuarioEstado,
                                    P.PersonalId AS PersonalIdPersonal,
                                    P.PersonalNombres,
                                    P.PersonalApellidos,
                                    P.AreaId,
                                    P.PersonalCodigo,
                                    P.PersonalNacimiento,
                                    P.PersonalIngreso,
                                    P.PersonalDNI,
                                    P.PersonalDireccion,
                                    P.PersonalTelefono,
                                    P.PersonalEmail,
                                    P.PersonalEstado,
                                    P.PersonalImagen,
                                    P.CompaniaId
                             FROM Usuarios U
                             LEFT JOIN Personal P ON U.PersonalId = P.PersonalId
                             WHERE U.UsuarioID = @UsuarioID";

        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@UsuarioID", id);
        con.Open();
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? MapWithPersonal(reader) : null;
    }

    private static UsuarioBd Map(SqlDataReader reader)
    {
        return new UsuarioBd
        {
            UsuarioID = Convert.ToInt32(reader["UsuarioID"]),
            PersonalId = reader["PersonalId"] == DBNull.Value ? null : Convert.ToInt32(reader["PersonalId"]),
            Nombre = reader["Nombre"].ToString(),
            UsuarioAlias = reader["UsuarioAlias"].ToString(),
            UsuarioClave = reader["UsuarioClave"].ToString(),
            Area = reader["Area"].ToString(),
            UsuarioFechaReg = reader["Fecha"] == DBNull.Value ? null : Convert.ToDateTime(reader["Fecha"]),
            UsuarioEstado = reader["Estado"].ToString()
        };
    }

    private static UsuarioConPersonal MapWithPersonal(SqlDataReader reader)
    {
        var usuario = new UsuarioConPersonal
        {
            UsuarioID = Convert.ToInt32(reader["UsuarioID"]),
            PersonalId = reader["PersonalId"] == DBNull.Value ? null : Convert.ToInt32(reader["PersonalId"]),
            UsuarioAlias = reader["UsuarioAlias"].ToString(),
            UsuarioClave = reader["UsuarioClave"].ToString(),
            UsuarioFechaReg = reader["Fecha"] == DBNull.Value ? null : Convert.ToDateTime(reader["Fecha"]),
            UsuarioEstado = reader["UsuarioEstado"].ToString()
        };

        if (reader["PersonalIdPersonal"] != DBNull.Value)
        {
            usuario.Personal = new Personal
            {
                PersonalId = Convert.ToInt64(reader["PersonalIdPersonal"]),
                PersonalNombres = reader["PersonalNombres"]?.ToString(),
                PersonalApellidos = reader["PersonalApellidos"]?.ToString(),
                AreaId = reader["AreaId"] == DBNull.Value ? null : Convert.ToInt64(reader["AreaId"]),
                PersonalCodigo = reader["PersonalCodigo"]?.ToString(),
                PersonalNacimiento = reader["PersonalNacimiento"] == DBNull.Value ? null : Convert.ToDateTime(reader["PersonalNacimiento"]),
                PersonalIngreso = reader["PersonalIngreso"] == DBNull.Value ? null : Convert.ToDateTime(reader["PersonalIngreso"]),
                PersonalDNI = reader["PersonalDNI"]?.ToString(),
                PersonalDireccion = reader["PersonalDireccion"]?.ToString(),
                PersonalTelefono = reader["PersonalTelefono"]?.ToString(),
                PersonalEmail = reader["PersonalEmail"]?.ToString(),
                PersonalEstado = reader["PersonalEstado"]?.ToString(),
                PersonalImagen = reader["PersonalImagen"]?.ToString(),
                CompaniaId = reader["CompaniaId"] == DBNull.Value ? null : Convert.ToInt32(reader["CompaniaId"])
            };
        }

        return usuario;
    }

    private static void AddParameters(SqlCommand cmd, UsuarioBd usuario)
    {
        var personalParam = cmd.Parameters.Add("@PersonalId", SqlDbType.Decimal);
        personalParam.Precision = 18;
        personalParam.Scale = 0;
        personalParam.Value = (object?)usuario.PersonalId ?? DBNull.Value;

        cmd.Parameters.AddWithValue("@UsuarioAlias", (object?)usuario.UsuarioAlias ?? DBNull.Value);

        var claveParam = cmd.Parameters.Add("@UsuarioClave", SqlDbType.VarBinary, 500);
        claveParam.Value = (object?)usuario.UsuarioClave ?? DBNull.Value;

        cmd.Parameters.AddWithValue("@UsuarioFechaReg", (object?)usuario.UsuarioFechaReg ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@UsuarioEstado", (object?)usuario.UsuarioEstado ?? DBNull.Value);
    }
}
