using System.Data;
using Ecommerce.Application.Contracts.Areas;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class AreaRepository : IArea
{
    private readonly string _connectionString;

    public AreaRepository()
    {
        var builder = WebApplication.CreateBuilder();
        _connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    }

    public bool Insertar(Area area)
    {
        const string sql = "INSERT INTO Area (AreaNombre) VALUES (@Nombre)";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@Nombre", area.AreaNombre ?? string.Empty);
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        var rows = cmd.ExecuteNonQuery();
        return rows > 0;
    }

    public bool Editar(int id, Area area)
    {
        const string sql = "UPDATE Area SET AreaNombre = @Nombre WHERE AreaId = @Id";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@Nombre", area.AreaNombre ?? string.Empty);
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        var rows = cmd.ExecuteNonQuery();
        return rows > 0;
    }

    public bool Eliminar(int id)
    {
        const string sql = "uspEliminarArea";
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

    public IReadOnlyList<EGeneral> Listar()
    {
        var lista = new List<EGeneral>();
        const string sql = "SELECT AreaId, AreaNombre FROM Area order by 1 asc";

        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(new EGeneral
            {
                Id = reader["AreaId"].ToString() ?? string.Empty,
                Nombre = reader["AreaNombre"].ToString() ?? string.Empty
            });
        }

        return lista;
    }
}
