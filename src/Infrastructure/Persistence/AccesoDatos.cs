using System.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Infrastructure.Persistence;

public class AccesoDatos
{
    string? CadenaConexion { get; set; }
    public AccesoDatos()
    {
        var builder = WebApplication.CreateBuilder();
        CadenaConexion = builder.Configuration.GetConnectionString("DefaultConnection");
    }
    public string ejecutarComando(string NombreSP, string parametroNombre = "", string parametroValor = "")
    {
        string? rpta = "";
        using (SqlConnection con = new SqlConnection(CadenaConexion))
        {
            try
            {
                if (con.State == ConnectionState.Open) con.Close();
                con.Open();
                SqlCommand cmd = new SqlCommand(NombreSP, con);
                cmd.CommandTimeout = 300;
                cmd.CommandType = CommandType.StoredProcedure;
                if (!String.IsNullOrEmpty(parametroNombre) && !String.IsNullOrEmpty(parametroValor))
                {
                    cmd.Parameters.AddWithValue(parametroNombre, parametroValor);
                }
                object data = cmd.ExecuteScalar();
                con.Close();
                if (data != null) rpta = data.ToString();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
        return rpta!;
    }

    public string ejecutarConsulta(string consulta)
    {
        string? rpta = "";
        using (SqlConnection con = new SqlConnection(CadenaConexion))
        {
            try
            {
                if (con.State == ConnectionState.Open) con.Close();
                con.Open();
                SqlCommand cmd = new SqlCommand(consulta, con);
                cmd.CommandType = CommandType.Text;
                object data = cmd.ExecuteScalar();
                if (data != null) rpta = data.ToString();
            }
            catch (Exception ex) { ex.ToString(); }
        }
        return rpta!;
    }
}
