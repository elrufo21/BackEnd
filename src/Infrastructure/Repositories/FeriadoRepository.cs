using System.Data;
using Ecommerce.Application.Contracts.Feriados;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class FeriadoRepository : IFeriado
{
    private readonly string _connectionString;
    private readonly AccesoDatos _accesoDatos = new();

    public FeriadoRepository()
    {
        var builder = WebApplication.CreateBuilder();
        _connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    }

    public string Insertar(Feriado feriado)
    {
        var xvalue = $"{feriado.IdFeriado}|{feriado.Fecha?.ToString("MM-dd-yyyy")}|{feriado.Motivo?.Trim()}";
        var rpt = _accesoDatos.ejecutarComando("uspIngresarFeriado", "@Data", xvalue);
        if (string.IsNullOrEmpty(rpt)) rpt = "error";
        return rpt;
    }

    public bool Eliminar(int id)
    {
        const string sql = "uspEliminarFeriado";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Id", id);
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        return cmd.ExecuteNonQuery() > 0;
    }

    public Feriado? ObtenerPorId(int id)
    {
        const string sql = "uspObtenerFeriadoPorId";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Id", id);
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? MapFeriado(reader) : null;
    }

    public IReadOnlyList<Feriado> Listar()
    {
        var lista = new List<Feriado>();
        const string sql = "uspListarFeriados";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.StoredProcedure;
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(MapFeriado(reader));
        }
        return lista;
    }

    private static Feriado MapFeriado(SqlDataReader reader)
    {
        return new Feriado
        {
            IdFeriado = Convert.ToInt32(reader["IdFeriado"]),
            Fecha = reader["Fecha"] == DBNull.Value ? null : Convert.ToDateTime(reader["Fecha"]),
            Motivo = reader["Motivo"]?.ToString()
        };
    }
}
