using System.Data;
using System.Text;
using Ecommerce.Application.Contracts.Compras;
using Ecommerce.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class CompraRepository : ICompra
{
    private readonly string _connectionString;

    public CompraRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing connection string: DefaultConnection");
    }

    public async Task<string> InsertarAsync(Compra compra, CancellationToken cancellationToken = default)
    {
        await using var con = new SqlConnection(_connectionString);
        await con.OpenAsync(cancellationToken);
        await using var tx = (SqlTransaction)await con.BeginTransactionAsync(cancellationToken);

        var compraId = await InsertOrUpdateCompraAsync(compra, con, tx, cancellationToken);
        if (compraId <= 0)
        {
            await tx.RollbackAsync(cancellationToken);
            return compra.CompraId > 0 ? "NOT_FOUND" : "error";
        }

        await tx.CommitAsync(cancellationToken);
        return compra.CompraId > 0 ? "UPDATED" : compraId.ToString();
    }

    public async Task<string> InsertarConDetalleAsync(Compra compra, IEnumerable<DetalleCompra> detalles, CancellationToken cancellationToken = default)
    {
        await using var con = new SqlConnection(_connectionString);
        await con.OpenAsync(cancellationToken);
        await using var tx = (SqlTransaction)await con.BeginTransactionAsync(cancellationToken);

        var compraId = await InsertOrUpdateCompraAsync(compra, con, tx, cancellationToken);
        if (compraId <= 0)
        {
            await tx.RollbackAsync(cancellationToken);
            return compra.CompraId > 0 ? "NOT_FOUND" : "error";
        }

        var detalleList = detalles?.ToList() ?? new List<DetalleCompra>();
        foreach (var detalle in detalleList)
        {
            detalle.CompraId = compraId;
        }
        await MergeDetallesCompraAsync(compraId, detalleList, con, tx, cancellationToken);

        await tx.CommitAsync(cancellationToken);
        return compraId.ToString();
    }

    public async Task<bool> EliminarAsync(long id, CancellationToken cancellationToken = default)
    {
        await using var con = new SqlConnection(_connectionString);
        await con.OpenAsync(cancellationToken);
        await using var tx = (SqlTransaction)await con.BeginTransactionAsync(cancellationToken);

        const string sqlDeleteCompra = "DELETE FROM Compras WHERE CompraId = @Id";
        await using var cmd = new SqlCommand(sqlDeleteCompra, con, tx)
        {
            CommandTimeout = 300,
            CommandType = CommandType.Text
        };
        cmd.Parameters.AddWithValue("@Id", id);

        const string sqlDeleteDetalles = "DELETE FROM DetalleCompra WHERE CompraId = @CompraId";
        await using var cmdDet = new SqlCommand(sqlDeleteDetalles, con, tx);
        cmdDet.Parameters.AddWithValue("@CompraId", id);
        await cmdDet.ExecuteNonQueryAsync(cancellationToken);

        var rows = await cmd.ExecuteNonQueryAsync(cancellationToken);
        if (rows > 0)
        {
            await tx.CommitAsync(cancellationToken);
            return true;
        }

        await tx.RollbackAsync(cancellationToken);
        return false;
    }

    public async Task<Compra?> ObtenerPorIdAsync(long id, CancellationToken cancellationToken = default)
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

        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, con)
        {
            CommandTimeout = 300,
            CommandType = CommandType.Text
        };
        cmd.Parameters.AddWithValue("@Id", id);
        await con.OpenAsync(cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
    }

    public async Task<IReadOnlyList<Compra>> ListarCrudAsync(string? estado = null, int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        (page, pageSize) = NormalizePagination(page, pageSize);
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
                    FROM Compras
                    WHERE (@Estado IS NULL OR CompraEstado = @Estado)
                    ORDER BY CompraId DESC
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, con)
        {
            CommandTimeout = 300,
            CommandType = CommandType.Text
        };
        cmd.Parameters.AddWithValue("@Estado", (object?)estado ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
        cmd.Parameters.AddWithValue("@PageSize", pageSize);
        await con.OpenAsync(cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

        var lista = new List<Compra>();
        while (await reader.ReadAsync(cancellationToken))
        {
            lista.Add(Map(reader));
        }
        return lista;
    }

    public async Task<IReadOnlyList<DetalleCompra>> ListarDetalleAsync(long compraId, int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        (page, pageSize) = NormalizePagination(page, pageSize);
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
                             ORDER BY DetalleId
                             OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, con)
        {
            CommandTimeout = 300,
            CommandType = CommandType.Text
        };
        cmd.Parameters.AddWithValue("@CompraId", compraId);
        cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
        cmd.Parameters.AddWithValue("@PageSize", pageSize);
        await con.OpenAsync(cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

        var lista = new List<DetalleCompra>();
        while (await reader.ReadAsync(cancellationToken))
        {
            lista.Add(MapDetalle(reader));
        }
        return lista;
    }

    private static async Task<long> InsertOrUpdateCompraAsync(Compra compra, SqlConnection con, SqlTransaction tx, CancellationToken cancellationToken)
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

            await using var cmd = new SqlCommand(sqlUpdate, con, tx)
            {
                CommandTimeout = 300,
                CommandType = CommandType.Text
            };
            AddParameters(cmd, compra);
            cmd.Parameters.AddWithValue("@CompraId", compra.CompraId);
            var rows = await cmd.ExecuteNonQueryAsync(cancellationToken);
            return rows > 0 ? compra.CompraId : 0;
        }

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

        await using var insertCmd = new SqlCommand(sqlInsert, con, tx)
        {
            CommandTimeout = 300,
            CommandType = CommandType.Text
        };
        AddParameters(insertCmd, compra);
        var result = await insertCmd.ExecuteScalarAsync(cancellationToken);
        return result == null ? 0 : Convert.ToInt64(result);
    }

    private static async Task MergeDetallesCompraAsync(long compraId, IReadOnlyList<DetalleCompra> detalles, SqlConnection con, SqlTransaction tx, CancellationToken cancellationToken)
    {
        if (detalles.Count == 0)
        {
            const string deleteSql = "DELETE FROM DetalleCompra WHERE CompraId = @CompraId";
            await using var deleteCmd = new SqlCommand(deleteSql, con, tx);
            deleteCmd.Parameters.AddWithValue("@CompraId", compraId);
            await deleteCmd.ExecuteNonQueryAsync(cancellationToken);
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine("MERGE DetalleCompra AS target");
        sb.AppendLine("USING (VALUES");

        for (var i = 0; i < detalles.Count; i++)
        {
            if (i > 0) sb.AppendLine(",");
            sb.Append($"(@CompraId, @DetalleId{i}, @IdProducto{i}, @DetalleCodigo{i}, @Descripcion{i}, @DetalleUM{i}, @DetalleCantidad{i}, @PrecioCosto{i}, @DetalleImporte{i}, @DetalleDescuento{i}, @DetalleEstado{i}, @DescuentoB{i}, @EstadoB{i}, @ValorUM{i})");
        }

        sb.AppendLine(") AS source (CompraId, DetalleId, IdProducto, DetalleCodigo, Descripcion, DetalleUM, DetalleCantidad, PrecioCosto, DetalleImporte, DetalleDescuento, DetalleEstado, DescuentoB, EstadoB, ValorUM)");
        sb.AppendLine("ON target.CompraId = source.CompraId AND target.DetalleId = source.DetalleId AND source.DetalleId > 0");
        sb.AppendLine("WHEN MATCHED THEN UPDATE SET");
        sb.AppendLine("    IdProducto = source.IdProducto,");
        sb.AppendLine("    DetalleCodigo = source.DetalleCodigo,");
        sb.AppendLine("    Descripcion = source.Descripcion,");
        sb.AppendLine("    DetalleUM = source.DetalleUM,");
        sb.AppendLine("    DetalleCantidad = source.DetalleCantidad,");
        sb.AppendLine("    PrecioCosto = source.PrecioCosto,");
        sb.AppendLine("    DetalleImporte = source.DetalleImporte,");
        sb.AppendLine("    DetalleDescuento = source.DetalleDescuento,");
        sb.AppendLine("    DetalleEstado = source.DetalleEstado,");
        sb.AppendLine("    DescuentoB = source.DescuentoB,");
        sb.AppendLine("    EstadoB = source.EstadoB,");
        sb.AppendLine("    ValorUM = source.ValorUM");
        sb.AppendLine("WHEN NOT MATCHED BY TARGET THEN");
        sb.AppendLine("    INSERT (CompraId, IdProducto, DetalleCodigo, Descripcion, DetalleUM, DetalleCantidad, PrecioCosto, DetalleImporte, DetalleDescuento, DetalleEstado, DescuentoB, EstadoB, ValorUM)");
        sb.AppendLine("    VALUES (source.CompraId, source.IdProducto, source.DetalleCodigo, source.Descripcion, source.DetalleUM, source.DetalleCantidad, source.PrecioCosto, source.DetalleImporte, source.DetalleDescuento, source.DetalleEstado, source.DescuentoB, source.EstadoB, source.ValorUM)");
        sb.AppendLine("WHEN NOT MATCHED BY SOURCE AND target.CompraId = @CompraId THEN DELETE;");

        await using var cmd = new SqlCommand(sb.ToString(), con, tx)
        {
            CommandTimeout = 300
        };
        cmd.Parameters.AddWithValue("@CompraId", compraId);

        for (var i = 0; i < detalles.Count; i++)
        {
            var detalle = detalles[i];
            cmd.Parameters.AddWithValue($"@DetalleId{i}", detalle.DetalleId);
            cmd.Parameters.AddWithValue($"@IdProducto{i}", (object?)detalle.IdProducto ?? DBNull.Value);
            cmd.Parameters.AddWithValue($"@DetalleCodigo{i}", (object?)detalle.DetalleCodigo ?? DBNull.Value);
            cmd.Parameters.AddWithValue($"@Descripcion{i}", (object?)detalle.Descripcion ?? DBNull.Value);
            cmd.Parameters.AddWithValue($"@DetalleUM{i}", (object?)detalle.DetalleUm ?? DBNull.Value);
            cmd.Parameters.AddWithValue($"@DetalleCantidad{i}", (object?)detalle.DetalleCantidad ?? DBNull.Value);
            cmd.Parameters.AddWithValue($"@PrecioCosto{i}", (object?)detalle.PrecioCosto ?? DBNull.Value);
            cmd.Parameters.AddWithValue($"@DetalleImporte{i}", (object?)detalle.DetalleImporte ?? DBNull.Value);
            cmd.Parameters.AddWithValue($"@DetalleDescuento{i}", (object?)detalle.DetalleDescuento ?? DBNull.Value);
            cmd.Parameters.AddWithValue($"@DetalleEstado{i}", (object?)detalle.DetalleEstado ?? DBNull.Value);
            cmd.Parameters.AddWithValue($"@DescuentoB{i}", (object?)detalle.DescuentoB ?? DBNull.Value);
            cmd.Parameters.AddWithValue($"@EstadoB{i}", (object?)detalle.EstadoB ?? DBNull.Value);
            cmd.Parameters.AddWithValue($"@ValorUM{i}", (object?)detalle.ValorUM ?? DBNull.Value);
        }

        await cmd.ExecuteNonQueryAsync(cancellationToken);
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

    private static (int page, int pageSize) NormalizePagination(int page, int pageSize)
    {
        var normalizedPage = page < 1 ? 1 : page;
        var normalizedPageSize = pageSize < 1 ? 1 : Math.Min(pageSize, 100);
        return (normalizedPage, normalizedPageSize);
    }
}
