using System.Data;
using Ecommerce.Application.Contracts.Lineas;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class LineaRepository : ILinea
{
    private readonly string _connectionString;
    AccesoDatos daSQL = new AccesoDatos();

    public LineaRepository()
    {
        var builder = WebApplication.CreateBuilder();
        _connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    }

    public bool Eliminar(int id)
    {
        const string sql = "uspEliminarCategoria";
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

    public string Insertar(Linea linea)
    {
        string rpt = string.Empty;
        string xvalue=string.Empty;
        xvalue=linea.IdSubLinea+"|"+linea.NombreSublinea+"|"+linea.CodigoSunat;
        rpt = daSQL.ejecutarComando("uspInsertarCategoria", "@Data",xvalue);
        if (string.IsNullOrEmpty(rpt)) rpt = "error";
        return rpt;
    }

    public bool Editar(int id, Linea linea)
    {
        const string sql = "UPDATE Sublinea SET NombreSublinea = @Nombre, CodigoSunat = @Codigo WHERE IdSubLinea = @Id";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@Nombre", linea.NombreSublinea ?? string.Empty);
        cmd.Parameters.AddWithValue("@Codigo", linea.CodigoSunat ?? string.Empty);
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        var rows = cmd.ExecuteNonQuery();
        return rows > 0;
    }
    public IReadOnlyList<EGeneral> Listar()
    {
        var lista = new List<EGeneral>();
        const string sql = "SELECT IdSubLinea, NombreSublinea, CodigoSunat FROM Sublinea order by NombreSublinea asc";

        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        if (con.State == ConnectionState.Open) con.Close();
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
