using System.Collections.Generic;
using System.Data;
using System.Linq;
using Ecommerce.Application.Contracts.Compras;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class CompraRepository : ICompra
{
    private readonly string _connectionString;

    public CompraRepository()
    {
        var builder = WebApplication.CreateBuilder();
        _connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    }

    public string Insertar(Compra compra)
    {
        using var con = new SqlConnection(_connectionString);
        con.Open();
        using var tx = con.BeginTransaction();

        var compraId = InsertOrUpdateCompra(compra, con, tx);
        if (compraId <= 0)
        {
            tx.Rollback();
            return compra.CompraId > 0 ? "NOT_FOUND" : "error";
        }

        tx.Commit();
        return compra.CompraId > 0 ? "UPDATED" : compraId.ToString();
    }

    public string InsertarConDetalle(Compra compra, IEnumerable<DetalleCompra> detalles)
    {
        using var con = new SqlConnection(_connectionString);
        con.Open();
        using var tx = con.BeginTransaction();

        var compraId = InsertOrUpdateCompra(compra, con, tx);
        if (compraId <= 0)
        {
            tx.Rollback();
            return compra.CompraId > 0 ? "NOT_FOUND" : "error";
        }

        var detalleList = detalles?.ToList() ?? new List<DetalleCompra>();
        DeleteDetallesByCompra(compraId, con, tx);
        foreach (var detalle in detalleList)
        {
            detalle.CompraId = compraId;
            InsertDetalle(detalle, con, tx);
        }

        tx.Commit();
        return compraId.ToString();
    }

    public bool Eliminar(long id)
    {
        using var con = new SqlConnection(_connectionString);
        con.Open();
        using var tx = con.BeginTransaction();

        DeleteDetallesByCompra(id, con, tx);

        const string sql = "DELETE FROM Compras WHERE CompraId = @Id";
        using var cmd = new SqlCommand(sql, con, tx);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("@Id", id);

        var rows = cmd.ExecuteNonQuery();
        if (rows > 0)
        {
            tx.Commit();
            return true;
        }

        tx.Rollback();
        return false;
    }

    public Compra? ObtenerPorId(long id)
    {
        const string sql = @"SELECT CompraId,
                                    CompraCorrelativo,
                                    ProveedorId,
                                    CompraRegistro,
                                    CompraEmision,
                                    CompraComputo,
                                    TipoCodigo,
                                    CompraSerie,
                                    CompraNumero,
                                    CompraCondicion,
                                    CompraMoneda,
                                    CompraTipoCambio,
                                    CompraDias,
                                    CompraFechaPago,
                                    CompraUsuario,
                                    CompraTipoIgv,
                                    CompraValorVenta,
                                    CompraDescuento,
                                    CompraSubtotal,
                                    CompraIgv,
                                    CompraTotal,
                                    CompraEstado,
                                    CompraAsociado,
                                    CompraSaldo,
                                    CompraOBS,
                                    CompraTipoSunat,
                                    CompraConcepto,
                                    CompraPercepcion
                             FROM Compras
                             WHERE CompraId = @Id";

        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("@Id", id);
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? Map(reader) : null;
    }

    public IReadOnlyList<Compra> ListarCrud(string? estado = null)
    {
        var sql = @"SELECT CompraId,
                           CompraCorrelativo,
                           ProveedorId,
                           CompraRegistro,
                           CompraEmision,
                           CompraComputo,
                           TipoCodigo,
                           CompraSerie,
                           CompraNumero,
                           CompraCondicion,
                           CompraMoneda,
                           CompraTipoCambio,
                           CompraDias,
                           CompraFechaPago,
                           CompraUsuario,
                           CompraTipoIgv,
                           CompraValorVenta,
                           CompraDescuento,
                           CompraSubtotal,
                           CompraIgv,
                           CompraTotal,
                           CompraEstado,
                           CompraAsociado,
                           CompraSaldo,
                           CompraOBS,
                           CompraTipoSunat,
                           CompraConcepto,
                           CompraPercepcion
                    FROM Compras";

        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.Text;
        if (!string.IsNullOrWhiteSpace(estado))
        {
            cmd.CommandText += " WHERE CompraEstado = @Estado";
            cmd.Parameters.AddWithValue("@Estado", estado);
        }

        cmd.CommandText += " ORDER BY CompraId DESC";

        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        using var reader = cmd.ExecuteReader();
        var lista = new List<Compra>();
        while (reader.Read())
        {
            lista.Add(Map(reader));
        }
        return lista;
    }

    public IReadOnlyList<DetalleCompra> ListarDetalle(long compraId)
    {
        const string sql = @"SELECT DetalleId,
                                    CompraId,
                                    IdProducto,
                                    DetalleCodigo,
                                    Descripcion,
                                    DetalleUM,
                                    DetalleCantidad,
                                    PrecioCosto,
                                    DetalleImporte,
                                    DetalleDescuento,
                                    DetalleEstado,
                                    DescuentoB,
                                    EstadoB,
                                    ValorUM
                             FROM DetalleCompra
                             WHERE CompraId = @CompraId
                             ORDER BY DetalleId";

        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("@CompraId", compraId);
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        using var reader = cmd.ExecuteReader();
        var lista = new List<DetalleCompra>();
        while (reader.Read())
        {
            lista.Add(MapDetalle(reader));
        }
        return lista;
    }

    private static long InsertOrUpdateCompra(Compra compra, SqlConnection con, SqlTransaction tx)
    {
        if (compra.CompraId > 0)
        {
            const string sqlUpdate = @"UPDATE Compras
                                       SET CompraCorrelativo = @CompraCorrelativo,
                                           ProveedorId = @ProveedorId,
                                           CompraRegistro = @CompraRegistro,
                                           CompraEmision = @CompraEmision,
                                           CompraComputo = @CompraComputo,
                                           TipoCodigo = @TipoCodigo,
                                           CompraSerie = @CompraSerie,
                                           CompraNumero = @CompraNumero,
                                           CompraCondicion = @CompraCondicion,
                                           CompraMoneda = @CompraMoneda,
                                           CompraTipoCambio = @CompraTipoCambio,
                                           CompraDias = @CompraDias,
                                           CompraFechaPago = @CompraFechaPago,
                                           CompraUsuario = @CompraUsuario,
                                           CompraTipoIgv = @CompraTipoIgv,
                                           CompraValorVenta = @CompraValorVenta,
                                           CompraDescuento = @CompraDescuento,
                                           CompraSubtotal = @CompraSubtotal,
                                           CompraIgv = @CompraIgv,
                                           CompraTotal = @CompraTotal,
                                           CompraEstado = @CompraEstado,
                                           CompraAsociado = @CompraAsociado,
                                           CompraSaldo = @CompraSaldo,
                                           CompraOBS = @CompraObs,
                                           CompraTipoSunat = @CompraTipoSunat,
                                           CompraConcepto = @CompraConcepto,
                                           CompraPercepcion = @CompraPercepcion
                                       WHERE CompraId = @CompraId";

            using var cmd = new SqlCommand(sqlUpdate, con, tx);
            cmd.CommandTimeout = 300;
            cmd.CommandType = CommandType.Text;
            AddParameters(cmd, compra);
            cmd.Parameters.AddWithValue("@CompraId", compra.CompraId);
            var rows = cmd.ExecuteNonQuery();
            return rows > 0 ? compra.CompraId : 0;
        }
        else
        {
            const string sqlInsert = @"INSERT INTO Compras
                                            (CompraCorrelativo,
                                             ProveedorId,
                                             CompraRegistro,
                                             CompraEmision,
                                             CompraComputo,
                                             TipoCodigo,
                                             CompraSerie,
                                             CompraNumero,
                                             CompraCondicion,
                                             CompraMoneda,
                                             CompraTipoCambio,
                                             CompraDias,
                                             CompraFechaPago,
                                             CompraUsuario,
                                             CompraTipoIgv,
                                             CompraValorVenta,
                                             CompraDescuento,
                                             CompraSubtotal,
                                             CompraIgv,
                                             CompraTotal,
                                             CompraEstado,
                                             CompraAsociado,
                                             CompraSaldo,
                                             CompraOBS,
                                             CompraTipoSunat,
                                             CompraConcepto,
                                             CompraPercepcion)
                                       VALUES (@CompraCorrelativo,
                                               @ProveedorId,
                                               @CompraRegistro,
                                               @CompraEmision,
                                               @CompraComputo,
                                               @TipoCodigo,
                                               @CompraSerie,
                                               @CompraNumero,
                                               @CompraCondicion,
                                               @CompraMoneda,
                                               @CompraTipoCambio,
                                               @CompraDias,
                                               @CompraFechaPago,
                                               @CompraUsuario,
                                               @CompraTipoIgv,
                                               @CompraValorVenta,
                                               @CompraDescuento,
                                               @CompraSubtotal,
                                               @CompraIgv,
                                               @CompraTotal,
                                               @CompraEstado,
                                               @CompraAsociado,
                                               @CompraSaldo,
                                               @CompraObs,
                                               @CompraTipoSunat,
                                               @CompraConcepto,
                                               @CompraPercepcion);
                                       SELECT SCOPE_IDENTITY();";

            using var cmd = new SqlCommand(sqlInsert, con, tx);
            cmd.CommandTimeout = 300;
            cmd.CommandType = CommandType.Text;
            AddParameters(cmd, compra);
            var result = cmd.ExecuteScalar();
            return result == null ? 0 : Convert.ToInt64(result);
        }
    }

    private static void InsertDetalle(DetalleCompra detalle, SqlConnection con, SqlTransaction tx)
    {
        const string sql = @"INSERT INTO DetalleCompra
                                    (CompraId,
                                     IdProducto,
                                     DetalleCodigo,
                                     Descripcion,
                                     DetalleUM,
                                     DetalleCantidad,
                                     PrecioCosto,
                                     DetalleImporte,
                                     DetalleDescuento,
                                     DetalleEstado,
                                     DescuentoB,
                                     EstadoB,
                                     ValorUM)
                             VALUES (@CompraId,
                                     @IdProducto,
                                     @DetalleCodigo,
                                     @Descripcion,
                                     @DetalleUM,
                                     @DetalleCantidad,
                                     @PrecioCosto,
                                     @DetalleImporte,
                                     @DetalleDescuento,
                                     @DetalleEstado,
                                     @DescuentoB,
                                     @EstadoB,
                                     @ValorUM)";

        using var cmd = new SqlCommand(sql, con, tx);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.Text;
        AddParam(cmd, "@CompraId", detalle.CompraId);
        AddParam(cmd, "@IdProducto", detalle.IdProducto);
        AddParam(cmd, "@DetalleCodigo", detalle.DetalleCodigo);
        AddParam(cmd, "@Descripcion", detalle.Descripcion);
        AddParam(cmd, "@DetalleUM", detalle.DetalleUm);
        AddParam(cmd, "@DetalleCantidad", detalle.DetalleCantidad);
        AddParam(cmd, "@PrecioCosto", detalle.PrecioCosto);
        AddParam(cmd, "@DetalleImporte", detalle.DetalleImporte);
        AddParam(cmd, "@DetalleDescuento", detalle.DetalleDescuento);
        AddParam(cmd, "@DetalleEstado", detalle.DetalleEstado);
        AddParam(cmd, "@DescuentoB", detalle.DescuentoB);
        AddParam(cmd, "@EstadoB", detalle.EstadoB);
        AddParam(cmd, "@ValorUM", detalle.ValorUM);
        cmd.ExecuteNonQuery();
    }

    private static void DeleteDetallesByCompra(long compraId, SqlConnection con, SqlTransaction tx)
    {
        const string sql = "DELETE FROM DetalleCompra WHERE CompraId = @CompraId";
        using var cmd = new SqlCommand(sql, con, tx);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("@CompraId", compraId);
        cmd.ExecuteNonQuery();
    }

    private static void AddParameters(SqlCommand cmd, Compra compra)
    {
        AddParam(cmd, "@CompraCorrelativo", compra.CompraCorrelativo);
        AddParam(cmd, "@ProveedorId", compra.ProveedorId);
        AddParam(cmd, "@CompraRegistro", compra.CompraRegistro);
        AddParam(cmd, "@CompraEmision", compra.CompraEmision);
        AddParam(cmd, "@CompraComputo", compra.CompraComputo);
        AddParam(cmd, "@TipoCodigo", compra.TipoCodigo);
        AddParam(cmd, "@CompraSerie", compra.CompraSerie);
        AddParam(cmd, "@CompraNumero", compra.CompraNumero);
        AddParam(cmd, "@CompraCondicion", compra.CompraCondicion);
        AddParam(cmd, "@CompraMoneda", compra.CompraMoneda);
        AddParam(cmd, "@CompraTipoCambio", compra.CompraTipoCambio);
        AddParam(cmd, "@CompraDias", compra.CompraDias);
        AddParam(cmd, "@CompraFechaPago", compra.CompraFechaPago);
        AddParam(cmd, "@CompraUsuario", compra.CompraUsuario);
        AddParam(cmd, "@CompraTipoIgv", compra.CompraTipoIgv);
        AddParam(cmd, "@CompraValorVenta", compra.CompraValorVenta);
        AddParam(cmd, "@CompraDescuento", compra.CompraDescuento);
        AddParam(cmd, "@CompraSubtotal", compra.CompraSubtotal);
        AddParam(cmd, "@CompraIgv", compra.CompraIgv);
        AddParam(cmd, "@CompraTotal", compra.CompraTotal);
        AddParam(cmd, "@CompraEstado", compra.CompraEstado);
        AddParam(cmd, "@CompraAsociado", compra.CompraAsociado);
        AddParam(cmd, "@CompraSaldo", compra.CompraSaldo);
        AddParam(cmd, "@CompraObs", compra.CompraObs);
        AddParam(cmd, "@CompraTipoSunat", compra.CompraTipoSunat);
        AddParam(cmd, "@CompraConcepto", compra.CompraConcepto);
        AddParam(cmd, "@CompraPercepcion", compra.CompraPercepcion);
    }

    private static void AddParam(SqlCommand cmd, string name, object? value)
    {
        cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
    }

    private static Compra Map(SqlDataReader reader)
    {
        return new Compra
        {
            CompraId = Convert.ToInt64(reader["CompraId"]),
            CompraCorrelativo = reader["CompraCorrelativo"]?.ToString(),
            ProveedorId = reader["ProveedorId"] == DBNull.Value ? null : Convert.ToInt64(reader["ProveedorId"]),
            CompraRegistro = reader["CompraRegistro"] == DBNull.Value ? null : Convert.ToDateTime(reader["CompraRegistro"]),
            CompraEmision = reader["CompraEmision"] == DBNull.Value ? null : Convert.ToDateTime(reader["CompraEmision"]),
            CompraComputo = reader["CompraComputo"] == DBNull.Value ? null : Convert.ToDateTime(reader["CompraComputo"]),
            TipoCodigo = reader["TipoCodigo"]?.ToString(),
            CompraSerie = reader["CompraSerie"]?.ToString(),
            CompraNumero = reader["CompraNumero"]?.ToString(),
            CompraCondicion = reader["CompraCondicion"]?.ToString(),
            CompraMoneda = reader["CompraMoneda"]?.ToString(),
            CompraTipoCambio = reader["CompraTipoCambio"] == DBNull.Value ? null : Convert.ToDecimal(reader["CompraTipoCambio"]),
            CompraDias = reader["CompraDias"] == DBNull.Value ? null : Convert.ToInt32(reader["CompraDias"]),
            CompraFechaPago = reader["CompraFechaPago"] == DBNull.Value ? null : Convert.ToDateTime(reader["CompraFechaPago"]),
            CompraUsuario = reader["CompraUsuario"]?.ToString(),
            CompraTipoIgv = reader["CompraTipoIgv"]?.ToString(),
            CompraValorVenta = reader["CompraValorVenta"] == DBNull.Value ? null : Convert.ToDecimal(reader["CompraValorVenta"]),
            CompraDescuento = reader["CompraDescuento"] == DBNull.Value ? null : Convert.ToDecimal(reader["CompraDescuento"]),
            CompraSubtotal = reader["CompraSubtotal"] == DBNull.Value ? null : Convert.ToDecimal(reader["CompraSubtotal"]),
            CompraIgv = reader["CompraIgv"] == DBNull.Value ? null : Convert.ToDecimal(reader["CompraIgv"]),
            CompraTotal = reader["CompraTotal"] == DBNull.Value ? null : Convert.ToDecimal(reader["CompraTotal"]),
            CompraEstado = reader["CompraEstado"]?.ToString(),
            CompraAsociado = reader["CompraAsociado"]?.ToString(),
            CompraSaldo = reader["CompraSaldo"] == DBNull.Value ? null : Convert.ToDecimal(reader["CompraSaldo"]),
            CompraObs = reader["CompraOBS"]?.ToString(),
            CompraTipoSunat = reader["CompraTipoSunat"] == DBNull.Value ? null : Convert.ToDecimal(reader["CompraTipoSunat"]),
            CompraConcepto = reader["CompraConcepto"]?.ToString(),
            CompraPercepcion = reader["CompraPercepcion"] == DBNull.Value ? null : Convert.ToDecimal(reader["CompraPercepcion"])
        };
    }

    private static DetalleCompra MapDetalle(SqlDataReader reader)
    {
        return new DetalleCompra
        {
            DetalleId = Convert.ToInt64(reader["DetalleId"]),
            CompraId = Convert.ToInt64(reader["CompraId"]),
            IdProducto = reader["IdProducto"] == DBNull.Value ? null : Convert.ToInt64(reader["IdProducto"]),
            DetalleCodigo = reader["DetalleCodigo"]?.ToString(),
            Descripcion = reader["Descripcion"]?.ToString(),
            DetalleUm = reader["DetalleUM"]?.ToString(),
            DetalleCantidad = reader["DetalleCantidad"] == DBNull.Value ? null : Convert.ToDecimal(reader["DetalleCantidad"]),
            PrecioCosto = reader["PrecioCosto"] == DBNull.Value ? null : Convert.ToDecimal(reader["PrecioCosto"]),
            DetalleImporte = reader["DetalleImporte"] == DBNull.Value ? null : Convert.ToDecimal(reader["DetalleImporte"]),
            DetalleDescuento = reader["DetalleDescuento"] == DBNull.Value ? null : Convert.ToDecimal(reader["DetalleDescuento"]),
            DetalleEstado = reader["DetalleEstado"]?.ToString(),
            DescuentoB = reader["DescuentoB"] == DBNull.Value ? null : Convert.ToDecimal(reader["DescuentoB"]),
            EstadoB = reader["EstadoB"]?.ToString(),
            ValorUM = reader["ValorUM"] == DBNull.Value ? null : Convert.ToDecimal(reader["ValorUM"])
        };
    }
}
