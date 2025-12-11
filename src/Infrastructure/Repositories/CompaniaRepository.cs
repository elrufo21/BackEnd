using Ecommerce.Application.Contracts.Companias;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class CompaniaRepository : ICompania
{
    private readonly string _connectionString;

    public CompaniaRepository()
    {
        var builder = WebApplication.CreateBuilder();
        _connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    }

    public bool Insertar(Compania compania)
    {
        const string sql = @"INSERT INTO Compania (
                                CompaniaRazonSocial,
                                CompaniaRUC,
                                CompaniaDireccion,
                                CompaniaTelefono,
                                CompaniaEmail,
                                CompaniaIniFecha,
                                CompaniaComercial,
                                CompaniaUserSecun,
                                ComapaniaPWD,
                                CompaniaPFX,
                                CompaniaClave,
                                CompaniaNomUBG,
                                CompaniaCodigoUBG,
                                CompaniaDistrito,
                                CompaniaDirecSunat,
                                ICBPER,
                                TokenApi,
                                ClienIdToken,
                                DescuentoMax,
                                DiasMaxDep,
                                RenovacionOSE,
                                RenovacionFirma,
                                RenovacionSome,
                                CorreoSGO,
                                PasswordCorreo,
                                CorreosAdmin)
                              VALUES (
                                @CompaniaRazonSocial,
                                @CompaniaRUC,
                                @CompaniaDireccion,
                                @CompaniaTelefono,
                                @CompaniaEmail,
                                @CompaniaIniFecha,
                                @CompaniaComercial,
                                @CompaniaUserSecun,
                                @ComapaniaPWD,
                                @CompaniaPFX,
                                @CompaniaClave,
                                @CompaniaNomUBG,
                                @CompaniaCodigoUBG,
                                @CompaniaDistrito,
                                @CompaniaDirecSunat,
                                @ICBPER,
                                @TokenApi,
                                @ClienIdToken,
                                @DescuentoMax,
                                @DiasMaxDep,
                                @RenovacionOSE,
                                @RenovacionFirma,
                                @RenovacionSome,
                                @CorreoSGO,
                                @PasswordCorreo,
                                @CorreosAdmin)";

        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        AddParameters(cmd, compania);
        con.Open();
        var rows = cmd.ExecuteNonQuery();
        return rows > 0;
    }

    public bool Editar(int id, Compania compania)
    {
        const string sql = @"UPDATE Compania SET
                                CompaniaRazonSocial = @CompaniaRazonSocial,
                                CompaniaRUC = @CompaniaRUC,
                                CompaniaDireccion = @CompaniaDireccion,
                                CompaniaTelefono = @CompaniaTelefono,
                                CompaniaEmail = @CompaniaEmail,
                                CompaniaIniFecha = @CompaniaIniFecha,
                                CompaniaComercial = @CompaniaComercial,
                                CompaniaUserSecun = @CompaniaUserSecun,
                                ComapaniaPWD = @ComapaniaPWD,
                                CompaniaPFX = @CompaniaPFX,
                                CompaniaClave = @CompaniaClave,
                                CompaniaNomUBG = @CompaniaNomUBG,
                                CompaniaCodigoUBG = @CompaniaCodigoUBG,
                                CompaniaDistrito = @CompaniaDistrito,
                                CompaniaDirecSunat = @CompaniaDirecSunat,
                                ICBPER = @ICBPER,
                                TokenApi = @TokenApi,
                                ClienIdToken = @ClienIdToken,
                                DescuentoMax = @DescuentoMax,
                                DiasMaxDep = @DiasMaxDep,
                                RenovacionOSE = @RenovacionOSE,
                                RenovacionFirma = @RenovacionFirma,
                                RenovacionSome = @RenovacionSome,
                                CorreoSGO = @CorreoSGO,
                                PasswordCorreo = @PasswordCorreo,
                                CorreosAdmin = @CorreosAdmin
                              WHERE CompaniaId = @Id";

        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@Id", id);
        AddParameters(cmd, compania);
        con.Open();
        var rows = cmd.ExecuteNonQuery();
        return rows > 0;
    }

    public bool Eliminar(int id)
    {
        const string sql = "DELETE FROM Compania WHERE CompaniaId = @Id";
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@Id", id);
        con.Open();
        var rows = cmd.ExecuteNonQuery();
        return rows > 0;
    }

    public IReadOnlyList<Compania> Listar()
    {
        var lista = new List<Compania>();
        const string sql = @"SELECT CompaniaId,
                                    CompaniaRazonSocial,
                                    CompaniaRUC,
                                    CompaniaDireccion,
                                    CompaniaTelefono,
                                    CompaniaEmail,
                                    CompaniaIniFecha,
                                    CompaniaComercial,
                                    CompaniaUserSecun,
                                    ComapaniaPWD,
                                    CompaniaPFX,
                                    CompaniaClave,
                                    CompaniaNomUBG,
                                    CompaniaCodigoUBG,
                                    CompaniaDistrito,
                                    CompaniaDirecSunat,
                                    ICBPER,
                                    TokenApi,
                                    ClienIdToken,
                                    DescuentoMax,
                                    DiasMaxDep,
                                    RenovacionOSE,
                                    RenovacionFirma,
                                    RenovacionSome,
                                    CorreoSGO,
                                    PasswordCorreo,
                                    CorreosAdmin
                             FROM Compania";

        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        con.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(new Compania
            {
                CompaniaId = Convert.ToInt32(reader["CompaniaId"]),
                CompaniaRazonSocial = reader["CompaniaRazonSocial"].ToString(),
                CompaniaRUC = reader["CompaniaRUC"].ToString(),
                CompaniaDireccion = reader["CompaniaDireccion"].ToString(),
                CompaniaTelefono = reader["CompaniaTelefono"].ToString(),
                CompaniaEmail = reader["CompaniaEmail"].ToString(),
                CompaniaIniFecha = reader["CompaniaIniFecha"].ToString(),
                CompaniaComercial = reader["CompaniaComercial"].ToString(),
                CompaniaUserSecun = reader["CompaniaUserSecun"].ToString(),
                ComapaniaPWD = reader["ComapaniaPWD"].ToString(),
                CompaniaPFX = reader["CompaniaPFX"].ToString(),
                CompaniaClave = reader["CompaniaClave"].ToString(),
                CompaniaNomUBG = reader["CompaniaNomUBG"].ToString(),
                CompaniaCodigoUBG = reader["CompaniaCodigoUBG"].ToString(),
                CompaniaDistrito = reader["CompaniaDistrito"].ToString(),
                CompaniaDirecSunat = reader["CompaniaDirecSunat"].ToString(),
                ICBPER = reader["ICBPER"] == DBNull.Value ? null : Convert.ToDecimal(reader["ICBPER"]),
                TokenApi = reader["TokenApi"].ToString(),
                ClienIdToken = reader["ClienIdToken"].ToString(),
                DescuentoMax = reader["DescuentoMax"] == DBNull.Value ? null : Convert.ToDecimal(reader["DescuentoMax"]),
                DiasMaxDep = reader["DiasMaxDep"] == DBNull.Value ? null : Convert.ToInt32(reader["DiasMaxDep"]),
                RenovacionOSE = reader["RenovacionOSE"] == DBNull.Value ? null : Convert.ToDateTime(reader["RenovacionOSE"]),
                RenovacionFirma = reader["RenovacionFirma"] == DBNull.Value ? null : Convert.ToDateTime(reader["RenovacionFirma"]),
                RenovacionSome = reader["RenovacionSome"] == DBNull.Value ? null : Convert.ToDateTime(reader["RenovacionSome"]),
                CorreoSGO = reader["CorreoSGO"].ToString(),
                PasswordCorreo = reader["PasswordCorreo"].ToString(),
                CorreosAdmin = reader["CorreosAdmin"].ToString()
            });
        }

        return lista;
    }

    public IReadOnlyList<EGeneral> ListarCombo()
    {
        var lista = new List<EGeneral>();
        const string sql = "SELECT CompaniaId, CompaniaRazonSocial FROM Compania";

        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        con.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(new EGeneral
            {
                Id = reader["CompaniaId"].ToString(),
                Nombre = reader["CompaniaRazonSocial"].ToString()
            });
        }

        return lista;
    }

    private static void AddParameters(SqlCommand cmd, Compania compania)
    {
        cmd.Parameters.AddWithValue("@CompaniaRazonSocial", (object?)compania.CompaniaRazonSocial ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CompaniaRUC", (object?)compania.CompaniaRUC ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CompaniaDireccion", (object?)compania.CompaniaDireccion ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CompaniaTelefono", (object?)compania.CompaniaTelefono ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CompaniaEmail", (object?)compania.CompaniaEmail ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CompaniaIniFecha", (object?)compania.CompaniaIniFecha ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CompaniaComercial", (object?)compania.CompaniaComercial ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CompaniaUserSecun", (object?)compania.CompaniaUserSecun ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ComapaniaPWD", (object?)compania.ComapaniaPWD ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CompaniaPFX", (object?)compania.CompaniaPFX ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CompaniaClave", (object?)compania.CompaniaClave ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CompaniaNomUBG", (object?)compania.CompaniaNomUBG ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CompaniaCodigoUBG", (object?)compania.CompaniaCodigoUBG ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CompaniaDistrito", (object?)compania.CompaniaDistrito ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CompaniaDirecSunat", (object?)compania.CompaniaDirecSunat ?? DBNull.Value);

        var icbperParam = cmd.Parameters.Add("@ICBPER", System.Data.SqlDbType.Decimal);
        icbperParam.Precision = 18;
        icbperParam.Scale = 2;
        icbperParam.Value = (object?)compania.ICBPER ?? DBNull.Value;

        cmd.Parameters.AddWithValue("@TokenApi", (object?)compania.TokenApi ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ClienIdToken", (object?)compania.ClienIdToken ?? DBNull.Value);

        var descuentoParam = cmd.Parameters.Add("@DescuentoMax", System.Data.SqlDbType.Decimal);
        descuentoParam.Precision = 18;
        descuentoParam.Scale = 2;
        descuentoParam.Value = (object?)compania.DescuentoMax ?? DBNull.Value;

        cmd.Parameters.AddWithValue("@DiasMaxDep", (object?)compania.DiasMaxDep ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@RenovacionOSE", (object?)compania.RenovacionOSE ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@RenovacionFirma", (object?)compania.RenovacionFirma ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@RenovacionSome", (object?)compania.RenovacionSome ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CorreoSGO", (object?)compania.CorreoSGO ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PasswordCorreo", (object?)compania.PasswordCorreo ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CorreosAdmin", (object?)compania.CorreosAdmin ?? DBNull.Value);
    }
}
