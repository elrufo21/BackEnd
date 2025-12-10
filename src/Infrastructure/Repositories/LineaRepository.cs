using Ecommerce.Application.Contracts.Lineas;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class LineaRepository : ILinea
{
    private readonly string _connectionString;

    public LineaRepository()
    {
        var builder = WebApplication.CreateBuilder();
        _connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    }

    public bool Eliminar(int id)
    {
        const string sql = "DELETE FROM Sublinea WHERE IdSubLinea = @Id";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@Id", id);
        con.Open();
        var rows = cmd.ExecuteNonQuery();
        return rows > 0;
    }

    public bool Insertar(Linea linea)
    {
        const string sql = "INSERT INTO Sublinea (NombreSublinea, CodigoSunat) VALUES (@Nombre, @Codigo)";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@Nombre", linea.NombreSublinea ?? string.Empty);
        cmd.Parameters.AddWithValue("@Codigo", linea.CodigoSunat ?? string.Empty);
        con.Open();
        var rows = cmd.ExecuteNonQuery();
        return rows > 0;
    }

    public IReadOnlyList<EGeneral> Listar()
    {
        var lista = new List<EGeneral>();
        const string sql = "SELECT IdSubLinea, NombreSublinea, CodigoSunat FROM Sublinea";

        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        con.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(new EGeneral
            {
                Id = reader["IdSubLinea"].ToString() ?? string.Empty,
                nombreSublinea = reader["NombreSublinea"].ToString() ?? string.Empty,
                CodigoSunat = reader["CodigoSunat"].ToString() ?? string.Empty
            });
        }

        return lista;
    }
}
