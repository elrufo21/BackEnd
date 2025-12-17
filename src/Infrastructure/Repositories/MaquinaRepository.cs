using System.Data;
using Ecommerce.Application.Contracts.Maquinas;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class MaquinaRepository : IMaquina
{
    private readonly string _connectionString;
    AccesoDatos daSQL = new AccesoDatos();

    public MaquinaRepository()
    {
        var builder = WebApplication.CreateBuilder();
        _connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    }

    public string Insertar(Maquina maquina)
    {
        string rpt = string.Empty;
        string xvalue = string.Empty;
        xvalue = maquina.IdMaquina + "|" + maquina.NombreMaquina + "|" + maquina.SerieFactura + "|" +
        maquina.SerieNC + "|" + maquina.SerieBoleta + "|" + maquina.Tiketera;
        rpt = daSQL.ejecutarComando("uspInsertarMaquina", "@Data", xvalue);
        if (string.IsNullOrEmpty(rpt)) rpt = "error";
        return rpt;
    }
    
    public bool Eliminar(int id)
    {
        const string sql = "DELETE FROM MAQUINAS WHERE IdMaquina = @Id";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@Id", id);
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        var rows = cmd.ExecuteNonQuery();
        return rows > 0;
    }

    public IReadOnlyList<Maquina> Listar()
    {
        var lista = new List<Maquina>();
        const string sql = "uspListarMaquinas";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.StoredProcedure;
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(new Maquina
            {
                IdMaquina = Convert.ToInt32(reader["IdMaquina"]),
                NombreMaquina = reader["Maquina"].ToString(),
                Registro = reader["Registro"].ToString(),
                SerieFactura = reader["SerieFactura"].ToString(),
                SerieNC = reader["SerieNC"].ToString(),
                SerieBoleta = reader["SerieBoleta"].ToString(),
                Tiketera = reader["Tiketera"].ToString()
            });
        }

        return lista;
    }
}
