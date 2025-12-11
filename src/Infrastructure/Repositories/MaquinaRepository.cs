using Ecommerce.Application.Contracts.Maquinas;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class MaquinaRepository : IMaquina
{
    private readonly string _connectionString;

    public MaquinaRepository()
    {
        var builder = WebApplication.CreateBuilder();
        _connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    }

    public bool Insertar(Maquina maquina)
    {
        const string sql = @"INSERT INTO MAQUINAS (Maquina, Registro, SerieFactura, SerieNC, SerieBoleta, Tiketera)
                             VALUES (@Maquina, @Registro, @SerieFactura, @SerieNC, @SerieBoleta, @Tiketera)";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@Maquina", maquina.NombreMaquina ?? string.Empty);
        cmd.Parameters.AddWithValue("@Registro", maquina.Registro ?? string.Empty);
        cmd.Parameters.AddWithValue("@SerieFactura", maquina.SerieFactura ?? string.Empty);
        cmd.Parameters.AddWithValue("@SerieNC", maquina.SerieNC ?? string.Empty);
        cmd.Parameters.AddWithValue("@SerieBoleta", maquina.SerieBoleta ?? string.Empty);
        cmd.Parameters.AddWithValue("@Tiketera", maquina.Tiketera ?? string.Empty);
        con.Open();
        var rows = cmd.ExecuteNonQuery();
        return rows > 0;
    }

    public bool Editar(int id, Maquina maquina)
    {
        const string sql = @"UPDATE MAQUINAS
                             SET Maquina = @Maquina,
                                 Registro = @Registro,
                                 SerieFactura = @SerieFactura,
                                 SerieNC = @SerieNC,
                                 SerieBoleta = @SerieBoleta,
                                 Tiketera = @Tiketera
                             WHERE IdMaquina = @Id";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@Maquina", maquina.NombreMaquina ?? string.Empty);
        cmd.Parameters.AddWithValue("@Registro", maquina.Registro ?? string.Empty);
        cmd.Parameters.AddWithValue("@SerieFactura", maquina.SerieFactura ?? string.Empty);
        cmd.Parameters.AddWithValue("@SerieNC", maquina.SerieNC ?? string.Empty);
        cmd.Parameters.AddWithValue("@SerieBoleta", maquina.SerieBoleta ?? string.Empty);
        cmd.Parameters.AddWithValue("@Tiketera", maquina.Tiketera ?? string.Empty);
        con.Open();
        var rows = cmd.ExecuteNonQuery();
        return rows > 0;
    }

    public bool Eliminar(int id)
    {
        const string sql = "DELETE FROM MAQUINAS WHERE IdMaquina = @Id";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@Id", id);
        con.Open();
        var rows = cmd.ExecuteNonQuery();
        return rows > 0;
    }

    public IReadOnlyList<Maquina> Listar()
    {
        var lista = new List<Maquina>();
        const string sql = @"SELECT IdMaquina, Maquina, Registro, SerieFactura, SerieNC, SerieBoleta, Tiketera
                             FROM MAQUINAS";

        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
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
