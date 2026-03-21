using System.Data;
using System.Text;
using Ecommerce.Application.Contracts.NotaPedido;
using Ecommerce.Domain;
using Ecommerce.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class NotaPedidoRepository : INotaPedido
{
    private readonly string _connectionString;
    private readonly AccesoDatos _accesoDatos;

    public NotaPedidoRepository(IConfiguration configuration, AccesoDatos accesoDatos)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing connection string: DefaultConnection");
        _accesoDatos = accesoDatos;
    }

    public async Task<string> RegistrarOrdenAsync(string data, CancellationToken cancellationToken = default)
    {
        var result = await _accesoDatos.EjecutarComandoAsync("uspinsertarNotaB", "@ListaOrden", data, cancellationToken);
        return string.IsNullOrWhiteSpace(result) ? "error" : result;
    }

    public async Task<string> EditarOrdenAsync(string data, CancellationToken cancellationToken = default)
    {
        var result = await _accesoDatos.EjecutarComandoAsync("uspEditarNotaPedido", "@Data", data, cancellationToken);
        return string.IsNullOrWhiteSpace(result) ? "error" : result;
    }

    public async Task<string> ListarDocumentosAsync(string data, CancellationToken cancellationToken = default)
    {
        return await _accesoDatos.EjecutarComandoAsync("uspListaDocumentos", "@Data", data, cancellationToken);
    }

    public async Task<CredencialesSunat?> ObtenerCredencialesSunatAsync(int companiaId, CancellationToken cancellationToken = default)
    {
        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("uspObtenerCredencialesSunat", con)
        {
            CommandTimeout = 300,
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.AddWithValue("@CompaniaId", companiaId);

        await con.OpenAsync(cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return new CredencialesSunat
        {
            UsuarioSOL = reader["UsuarioSOL"] == DBNull.Value ? null : reader["UsuarioSOL"].ToString(),
            ClaveSOL = reader["ClaveSOL"] == DBNull.Value ? null : reader["ClaveSOL"].ToString(),
            CertificadoPFX = reader["CertificadoPFX"] == DBNull.Value ? null : reader["CertificadoPFX"].ToString(),
            ClaveCertificado = reader["ClaveCertificado"] == DBNull.Value ? null : reader["ClaveCertificado"].ToString(),
            Entorno = reader["Entorno"] == DBNull.Value ? null : reader["Entorno"].ToString()
        };
    }

    public async Task<bool> GuardarCredencialesSunatAsync(
        int companiaId,
        string usuarioSol,
        string claveSol,
        string certificadoBase64,
        string claveCertificado,
        int entorno,
        CancellationToken cancellationToken = default)
    {
        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("uspGuardarCredencialesSunat", con)
        {
            CommandTimeout = 300,
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.AddWithValue("@CompaniaId", companiaId);
        cmd.Parameters.AddWithValue("@UsuarioSOL", (object?)usuarioSol ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ClaveSOL", (object?)claveSol ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CertificadoBase64", (object?)certificadoBase64 ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ClaveCertificado", (object?)claveCertificado ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Entorno", entorno);

        await con.OpenAsync(cancellationToken);
        await cmd.ExecuteNonQueryAsync(cancellationToken);
        return true;
    }

    public async Task<string> InsertarAsync(NotaPedido notaPedido, CancellationToken cancellationToken = default)
    {
        await using var con = new SqlConnection(_connectionString);
        await con.OpenAsync(cancellationToken);
        await using var tx = (SqlTransaction)await con.BeginTransactionAsync(cancellationToken);

        var notaId = await InsertOrUpdateNotaAsync(notaPedido, con, tx, cancellationToken);
        if (notaId <= 0)
        {
            await tx.RollbackAsync(cancellationToken);
            return notaPedido.NotaId > 0 ? "NOT_FOUND" : "error";
        }

        await tx.CommitAsync(cancellationToken);
        return notaPedido.NotaId > 0 ? "UPDATED" : notaId.ToString();
    }

    public async Task<string> InsertarConDetalleAsync(NotaPedido notaPedido, IEnumerable<DetalleNota> detalles, CancellationToken cancellationToken = default)
    {
        await using var con = new SqlConnection(_connectionString);
        await con.OpenAsync(cancellationToken);
        await using var tx = (SqlTransaction)await con.BeginTransactionAsync(cancellationToken);

        var notaId = await InsertOrUpdateNotaAsync(notaPedido, con, tx, cancellationToken);
        if (notaId <= 0)
        {
            await tx.RollbackAsync(cancellationToken);
            return notaPedido.NotaId > 0 ? "NOT_FOUND" : "error";
        }

        var detalleList = detalles?.ToList() ?? new List<DetalleNota>();
        foreach (var detalle in detalleList)
        {
            detalle.NotaId = notaId;
        }
        await MergeDetallesNotaAsync(notaId, detalleList, con, tx, cancellationToken);

        await tx.CommitAsync(cancellationToken);
        return notaId.ToString();
    }

    public async Task<bool> EliminarAsync(long id, CancellationToken cancellationToken = default)
    {
        await using var con = new SqlConnection(_connectionString);
        await con.OpenAsync(cancellationToken);
        await using var tx = (SqlTransaction)await con.BeginTransactionAsync(cancellationToken);

        const string sqlDeleteDetalles = "DELETE FROM DetallePedido WHERE NotaId = @NotaId";
        await using var cmdDet = new SqlCommand(sqlDeleteDetalles, con, tx);
        cmdDet.Parameters.AddWithValue("@NotaId", id);
        await cmdDet.ExecuteNonQueryAsync(cancellationToken);

        const string sqlDeleteNota = "DELETE FROM NotaPedido WHERE NotaId = @Id";
        await using var cmd = new SqlCommand(sqlDeleteNota, con, tx);
        cmd.Parameters.AddWithValue("@Id", id);
        var rows = await cmd.ExecuteNonQueryAsync(cancellationToken);
        if (rows > 0)
        {
            await tx.CommitAsync(cancellationToken);
            return true;
        }

        await tx.RollbackAsync(cancellationToken);
        return false;
    }

    public async Task<NotaPedido?> ObtenerPorIdAsync(long id, CancellationToken cancellationToken = default)
    {
        const string sql = @"SELECT NotaId,
                                    NotaDocu,
                                    ClienteId,
                                    NotaFecha,
                                    NotaUsuario,
                                    NotaFormaPago,
                                    NotaCondicion,
                                    NotaFechaPago,
                                    NotaDireccion,
                                    NotaTelefono,
                                    NotaSubtotal,
                                    NotaMovilidad,
                                    NotaDescuento,
                                    NotaTotal,
                                    NotaAcuenta,
                                    NotaSaldo,
                                    NotaAdicional,
                                    NotaTarjeta,
                                    NotaPagar,
                                    NotaEstado,
                                    CompaniaId,
                                    NotaEntrega,
                                    ModificadoPor,
                                    FechaEdita,
                                    NotaConcepto,
                                    NotaSerie,
                                    NotaNumero,
                                    NotaGanancia,
                                    ICBPER,
                                    CajaId,
                                    EntidadBancaria,
                                    NroOperacion,
                                    Efectivo,
                                    Deposito
                             FROM NotaPedido
                             WHERE NotaId = @Id";

        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@Id", id);
        await con.OpenAsync(cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
    }

    public async Task<string> ObtenerNotaPedidoSpAsync(long id, CancellationToken cancellationToken = default)
    {
        var result = await _accesoDatos.EjecutarComandoAsync(
            "uspObtenerNotaPedido",
            "@Valores",
            id.ToString(),
            cancellationToken);

        return string.IsNullOrWhiteSpace(result) ? "~" : result;
    }

    public async Task<IReadOnlyList<NotaPedido>> ListarCrudAsync(string? estado = null, int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        (page, pageSize) = NormalizePagination(page, pageSize);
        const string sql = @"SELECT NotaId,
                                    NotaDocu,
                                    ClienteId,
                                    NotaFecha,
                                    NotaUsuario,
                                    NotaFormaPago,
                                    NotaCondicion,
                                    NotaFechaPago,
                                    NotaDireccion,
                                    NotaTelefono,
                                    NotaSubtotal,
                                    NotaMovilidad,
                                    NotaDescuento,
                                    NotaTotal,
                                    NotaAcuenta,
                                    NotaSaldo,
                                    NotaAdicional,
                                    NotaTarjeta,
                                    NotaPagar,
                                    NotaEstado,
                                    CompaniaId,
                                    NotaEntrega,
                                    ModificadoPor,
                                    FechaEdita,
                                    NotaConcepto,
                                    NotaSerie,
                                    NotaNumero,
                                    NotaGanancia,
                                    ICBPER,
                                    CajaId,
                                    EntidadBancaria,
                                    NroOperacion,
                                    Efectivo,
                                    Deposito
                             FROM NotaPedido
                             WHERE (@Estado IS NULL OR NotaEstado = @Estado)
                             ORDER BY NotaId DESC
                             OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@Estado", (object?)estado ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
        cmd.Parameters.AddWithValue("@PageSize", pageSize);
        await con.OpenAsync(cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

        var lista = new List<NotaPedido>();
        while (await reader.ReadAsync(cancellationToken))
        {
            lista.Add(Map(reader));
        }
        return lista;
    }

    public async Task<IReadOnlyList<DetalleNota>> ListarDetalleAsync(long notaId, int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        (page, pageSize) = NormalizePagination(page, pageSize);
        const string sql = @"SELECT DetalleId,
                                    NotaId,
                                    IdProducto,
                                    DetalleCantidad,
                                    DetalleUm,
                                    DetalleDescripcion,
                                    DetalleCosto,
                                    DetallePrecio,
                                    DetalleImporte,
                                    DetalleEstado,
                                    CantidadSaldo,
                                    ValorUM
                             FROM DetallePedido
                             WHERE NotaId = @NotaId
                             ORDER BY DetalleId
                             OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@NotaId", notaId);
        cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
        cmd.Parameters.AddWithValue("@PageSize", pageSize);
        await con.OpenAsync(cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        var lista = new List<DetalleNota>();
        while (await reader.ReadAsync(cancellationToken))
        {
            lista.Add(MapDetalle(reader));
        }
        return lista;
    }

    public async Task<IReadOnlyList<EListaNota>> ListarAsync(DateTime fechaInicio, DateTime fechaFin, int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        (page, pageSize) = NormalizePagination(page, pageSize);

        const string sp = "listaNotaPedido";
        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sp, con)
        {
            CommandTimeout = 300,
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio.Date);
        cmd.Parameters.AddWithValue("@FechaFin", fechaFin.Date);

        await con.OpenAsync(cancellationToken);
        var scalar = await cmd.ExecuteScalarAsync(cancellationToken);
        var result = scalar?.ToString() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(result))
        {
            return new List<EListaNota>();
        }

        var lista = Cadena.AlistaCamposNota(result);
        return lista.Skip((page - 1) * pageSize).Take(pageSize).ToList();
    }

    private static async Task<long> InsertOrUpdateNotaAsync(NotaPedido notaPedido, SqlConnection con, SqlTransaction tx, CancellationToken cancellationToken)
    {
        if (notaPedido.NotaId > 0)
        {
            const string sqlUpdate = @"UPDATE NotaPedido
                                       SET NotaDocu = @NotaDocu,
                                           ClienteId = @ClienteId,
                                           NotaFecha = @NotaFecha,
                                           NotaUsuario = @NotaUsuario,
                                           NotaFormaPago = @NotaFormaPago,
                                           NotaCondicion = @NotaCondicion,
                                           NotaFechaPago = @NotaFechaPago,
                                           NotaDireccion = @NotaDireccion,
                                           NotaTelefono = @NotaTelefono,
                                           NotaSubtotal = @NotaSubtotal,
                                           NotaMovilidad = @NotaMovilidad,
                                           NotaDescuento = @NotaDescuento,
                                           NotaTotal = @NotaTotal,
                                           NotaAcuenta = @NotaAcuenta,
                                           NotaSaldo = @NotaSaldo,
                                           NotaAdicional = @NotaAdicional,
                                           NotaTarjeta = @NotaTarjeta,
                                           NotaPagar = @NotaPagar,
                                           NotaEstado = @NotaEstado,
                                           CompaniaId = @CompaniaId,
                                           NotaEntrega = @NotaEntrega,
                                           ModificadoPor = @ModificadoPor,
                                           FechaEdita = @FechaEdita,
                                           NotaConcepto = @NotaConcepto,
                                           NotaSerie = @NotaSerie,
                                           NotaNumero = @NotaNumero,
                                           NotaGanancia = @NotaGanancia,
                                           ICBPER = @ICBPER,
                                           CajaId = @CajaId,
                                           EntidadBancaria = @EntidadBancaria,
                                           NroOperacion = @NroOperacion,
                                           Efectivo = @Efectivo,
                                           Deposito = @Deposito
                                       WHERE NotaId = @NotaId";

            await using var cmd = new SqlCommand(sqlUpdate, con, tx);
            AddParameters(cmd, notaPedido);
            cmd.Parameters.AddWithValue("@NotaId", notaPedido.NotaId);
            var rows = await cmd.ExecuteNonQueryAsync(cancellationToken);
            return rows > 0 ? notaPedido.NotaId : 0;
        }

        const string sqlInsert = @"INSERT INTO NotaPedido
                                    (NotaDocu, ClienteId, NotaFecha, NotaUsuario, NotaFormaPago, NotaCondicion,
                                     NotaFechaPago, NotaDireccion, NotaTelefono, NotaSubtotal, NotaMovilidad,
                                     NotaDescuento, NotaTotal, NotaAcuenta, NotaSaldo, NotaAdicional, NotaTarjeta,
                                     NotaPagar, NotaEstado, CompaniaId, NotaEntrega, ModificadoPor, FechaEdita,
                                     NotaConcepto, NotaSerie, NotaNumero, NotaGanancia, ICBPER, CajaId,
                                     EntidadBancaria, NroOperacion, Efectivo, Deposito)
                               VALUES (@NotaDocu, @ClienteId, @NotaFecha, @NotaUsuario, @NotaFormaPago, @NotaCondicion,
                                       @NotaFechaPago, @NotaDireccion, @NotaTelefono, @NotaSubtotal, @NotaMovilidad,
                                       @NotaDescuento, @NotaTotal, @NotaAcuenta, @NotaSaldo, @NotaAdicional, @NotaTarjeta,
                                       @NotaPagar, @NotaEstado, @CompaniaId, @NotaEntrega, @ModificadoPor, @FechaEdita,
                                       @NotaConcepto, @NotaSerie, @NotaNumero, @NotaGanancia, @ICBPER, @CajaId,
                                       @EntidadBancaria, @NroOperacion, @Efectivo, @Deposito);
                               SELECT SCOPE_IDENTITY();";

        await using var insertCmd = new SqlCommand(sqlInsert, con, tx);
        AddParameters(insertCmd, notaPedido);
        var result = await insertCmd.ExecuteScalarAsync(cancellationToken);
        return result == null ? 0 : Convert.ToInt64(result);
    }

    private static async Task MergeDetallesNotaAsync(long notaId, IReadOnlyList<DetalleNota> detalles, SqlConnection con, SqlTransaction tx, CancellationToken cancellationToken)
    {
        if (detalles.Count == 0)
        {
            const string deleteSql = "DELETE FROM DetallePedido WHERE NotaId = @NotaId";
            await using var deleteCmd = new SqlCommand(deleteSql, con, tx);
            deleteCmd.Parameters.AddWithValue("@NotaId", notaId);
            await deleteCmd.ExecuteNonQueryAsync(cancellationToken);
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine("MERGE DetallePedido AS target");
        sb.AppendLine("USING (VALUES");

        for (var i = 0; i < detalles.Count; i++)
        {
            if (i > 0) sb.AppendLine(",");
            sb.Append($"(@NotaId, @DetalleId{i}, @IdProducto{i}, @DetalleCantidad{i}, @DetalleUm{i}, @DetalleDescripcion{i}, @DetalleCosto{i}, @DetallePrecio{i}, @DetalleImporte{i}, @DetalleEstado{i}, @CantidadSaldo{i}, @ValorUM{i})");
        }

        sb.AppendLine(") AS source (NotaId, DetalleId, IdProducto, DetalleCantidad, DetalleUm, DetalleDescripcion, DetalleCosto, DetallePrecio, DetalleImporte, DetalleEstado, CantidadSaldo, ValorUM)");
        sb.AppendLine("ON target.NotaId = source.NotaId AND target.DetalleId = source.DetalleId AND source.DetalleId > 0");
        sb.AppendLine("WHEN MATCHED THEN UPDATE SET");
        sb.AppendLine("    IdProducto = source.IdProducto,");
        sb.AppendLine("    DetalleCantidad = source.DetalleCantidad,");
        sb.AppendLine("    DetalleUm = source.DetalleUm,");
        sb.AppendLine("    DetalleDescripcion = source.DetalleDescripcion,");
        sb.AppendLine("    DetalleCosto = source.DetalleCosto,");
        sb.AppendLine("    DetallePrecio = source.DetallePrecio,");
        sb.AppendLine("    DetalleImporte = source.DetalleImporte,");
        sb.AppendLine("    DetalleEstado = source.DetalleEstado,");
        sb.AppendLine("    CantidadSaldo = source.CantidadSaldo,");
        sb.AppendLine("    ValorUM = source.ValorUM");
        sb.AppendLine("WHEN NOT MATCHED BY TARGET THEN");
        sb.AppendLine("    INSERT (NotaId, IdProducto, DetalleCantidad, DetalleUm, DetalleDescripcion, DetalleCosto, DetallePrecio, DetalleImporte, DetalleEstado, CantidadSaldo, ValorUM)");
        sb.AppendLine("    VALUES (source.NotaId, source.IdProducto, source.DetalleCantidad, source.DetalleUm, source.DetalleDescripcion, source.DetalleCosto, source.DetallePrecio, source.DetalleImporte, source.DetalleEstado, source.CantidadSaldo, source.ValorUM)");
        sb.AppendLine("WHEN NOT MATCHED BY SOURCE AND target.NotaId = @NotaId THEN DELETE;");

        await using var cmd = new SqlCommand(sb.ToString(), con, tx);
        cmd.Parameters.AddWithValue("@NotaId", notaId);

        for (var i = 0; i < detalles.Count; i++)
        {
            var detalle = detalles[i];
            cmd.Parameters.AddWithValue($"@DetalleId{i}", detalle.DetalleId);
            cmd.Parameters.AddWithValue($"@IdProducto{i}", (object?)detalle.IdProducto ?? DBNull.Value);
            cmd.Parameters.AddWithValue($"@DetalleCantidad{i}", (object?)detalle.DetalleCantidad ?? DBNull.Value);
            cmd.Parameters.AddWithValue($"@DetalleUm{i}", (object?)detalle.DetalleUm ?? DBNull.Value);
            cmd.Parameters.AddWithValue($"@DetalleDescripcion{i}", (object?)detalle.DetalleDescripcion ?? DBNull.Value);
            cmd.Parameters.AddWithValue($"@DetalleCosto{i}", (object?)detalle.DetalleCosto ?? DBNull.Value);
            cmd.Parameters.AddWithValue($"@DetallePrecio{i}", (object?)detalle.DetallePrecio ?? DBNull.Value);
            cmd.Parameters.AddWithValue($"@DetalleImporte{i}", (object?)detalle.DetalleImporte ?? DBNull.Value);
            cmd.Parameters.AddWithValue($"@DetalleEstado{i}", (object?)detalle.DetalleEstado ?? DBNull.Value);
            cmd.Parameters.AddWithValue($"@CantidadSaldo{i}", (object?)detalle.CantidadSaldo ?? DBNull.Value);
            cmd.Parameters.AddWithValue($"@ValorUM{i}", (object?)detalle.ValorUM ?? DBNull.Value);
        }

        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    private static void AddParameters(SqlCommand cmd, NotaPedido notaPedido)
    {
        AddParam(cmd, "@NotaDocu", notaPedido.NotaDocu);
        AddParam(cmd, "@ClienteId", notaPedido.ClienteId);
        AddParam(cmd, "@NotaFecha", notaPedido.NotaFecha);
        AddParam(cmd, "@NotaUsuario", notaPedido.NotaUsuario);
        AddParam(cmd, "@NotaFormaPago", notaPedido.NotaFormaPago);
        AddParam(cmd, "@NotaCondicion", notaPedido.NotaCondicion);
        AddParam(cmd, "@NotaFechaPago", notaPedido.NotaFechaPago);
        AddParam(cmd, "@NotaDireccion", notaPedido.NotaDireccion);
        AddParam(cmd, "@NotaTelefono", notaPedido.NotaTelefono);
        AddParam(cmd, "@NotaSubtotal", notaPedido.NotaSubtotal);
        AddParam(cmd, "@NotaMovilidad", notaPedido.NotaMovilidad);
        AddParam(cmd, "@NotaDescuento", notaPedido.NotaDescuento);
        AddParam(cmd, "@NotaTotal", notaPedido.NotaTotal);
        AddParam(cmd, "@NotaAcuenta", notaPedido.NotaAcuenta);
        AddParam(cmd, "@NotaSaldo", notaPedido.NotaSaldo);
        AddParam(cmd, "@NotaAdicional", notaPedido.NotaAdicional);
        AddParam(cmd, "@NotaTarjeta", notaPedido.NotaTarjeta);
        AddParam(cmd, "@NotaPagar", notaPedido.NotaPagar);
        AddParam(cmd, "@NotaEstado", notaPedido.NotaEstado);
        AddParam(cmd, "@CompaniaId", notaPedido.CompaniaId);
        AddParam(cmd, "@NotaEntrega", notaPedido.NotaEntrega);
        AddParam(cmd, "@ModificadoPor", notaPedido.ModificadoPor);
        AddParam(cmd, "@FechaEdita", notaPedido.FechaEdita);
        AddParam(cmd, "@NotaConcepto", notaPedido.NotaConcepto);
        AddParam(cmd, "@NotaSerie", notaPedido.NotaSerie);
        AddParam(cmd, "@NotaNumero", notaPedido.NotaNumero);
        AddParam(cmd, "@NotaGanancia", notaPedido.NotaGanancia);
        AddParam(cmd, "@ICBPER", notaPedido.ICBPER);
        AddParam(cmd, "@CajaId", notaPedido.CajaId);
        AddParam(cmd, "@EntidadBancaria", notaPedido.EntidadBancaria);
        AddParam(cmd, "@NroOperacion", notaPedido.NroOperacion);
        AddParam(cmd, "@Efectivo", notaPedido.Efectivo);
        AddParam(cmd, "@Deposito", notaPedido.Deposito);
    }

    private static void AddParam(SqlCommand cmd, string name, object? value)
    {
        cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
    }

    private static NotaPedido Map(SqlDataReader reader)
    {
        return new NotaPedido
        {
            NotaId = Convert.ToInt64(reader["NotaId"]),
            NotaDocu = reader["NotaDocu"].ToString(),
            ClienteId = reader["ClienteId"] == DBNull.Value ? null : Convert.ToInt64(reader["ClienteId"]),
            NotaFecha = ToNullableDate(reader["NotaFecha"]),
            NotaUsuario = reader["NotaUsuario"].ToString(),
            NotaFormaPago = reader["NotaFormaPago"].ToString(),
            NotaCondicion = reader["NotaCondicion"].ToString(),
            NotaFechaPago = ToNullableDate(reader["NotaFechaPago"]),
            NotaDireccion = reader["NotaDireccion"].ToString(),
            NotaTelefono = reader["NotaTelefono"].ToString(),
            NotaSubtotal = reader["NotaSubtotal"] == DBNull.Value ? null : Convert.ToDecimal(reader["NotaSubtotal"]),
            NotaMovilidad = reader["NotaMovilidad"] == DBNull.Value ? null : Convert.ToDecimal(reader["NotaMovilidad"]),
            NotaDescuento = reader["NotaDescuento"] == DBNull.Value ? null : Convert.ToDecimal(reader["NotaDescuento"]),
            NotaTotal = reader["NotaTotal"] == DBNull.Value ? null : Convert.ToDecimal(reader["NotaTotal"]),
            NotaAcuenta = reader["NotaAcuenta"] == DBNull.Value ? null : Convert.ToDecimal(reader["NotaAcuenta"]),
            NotaSaldo = reader["NotaSaldo"] == DBNull.Value ? null : Convert.ToDecimal(reader["NotaSaldo"]),
            NotaAdicional = reader["NotaAdicional"] == DBNull.Value ? null : Convert.ToDecimal(reader["NotaAdicional"]),
            NotaTarjeta = reader["NotaTarjeta"] == DBNull.Value ? null : Convert.ToDecimal(reader["NotaTarjeta"]),
            NotaPagar = reader["NotaPagar"] == DBNull.Value ? null : Convert.ToDecimal(reader["NotaPagar"]),
            NotaEstado = reader["NotaEstado"].ToString(),
            CompaniaId = reader["CompaniaId"] == DBNull.Value ? null : Convert.ToInt32(reader["CompaniaId"]),
            NotaEntrega = reader["NotaEntrega"].ToString(),
            ModificadoPor = reader["ModificadoPor"].ToString(),
            FechaEdita = ToNullableDate(reader["FechaEdita"]),
            NotaConcepto = reader["NotaConcepto"].ToString(),
            NotaSerie = reader["NotaSerie"].ToString(),
            NotaNumero = reader["NotaNumero"].ToString(),
            NotaGanancia = reader["NotaGanancia"] == DBNull.Value ? null : Convert.ToDecimal(reader["NotaGanancia"]),
            ICBPER = reader["ICBPER"] == DBNull.Value ? null : Convert.ToDecimal(reader["ICBPER"]),
            CajaId = reader["CajaId"] == DBNull.Value ? null : Convert.ToInt32(reader["CajaId"]),
            EntidadBancaria = reader["EntidadBancaria"].ToString(),
            NroOperacion = reader["NroOperacion"].ToString(),
            Efectivo = reader["Efectivo"] == DBNull.Value ? null : Convert.ToDecimal(reader["Efectivo"]),
            Deposito = reader["Deposito"] == DBNull.Value ? null : Convert.ToDecimal(reader["Deposito"])
        };
    }

    private static DateTime? ToNullableDate(object? value)
    {
        if (value == null || value == DBNull.Value) return null;
        if (value is string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            return DateTime.TryParse(s, out var parsed) ? parsed : null;
        }

        try
        {
            return Convert.ToDateTime(value);
        }
        catch
        {
            return null;
        }
    }

    private static DetalleNota MapDetalle(SqlDataReader reader)
    {
        return new DetalleNota
        {
            DetalleId = Convert.ToInt64(reader["DetalleId"]),
            NotaId = Convert.ToInt64(reader["NotaId"]),
            IdProducto = reader["IdProducto"] == DBNull.Value ? null : Convert.ToInt64(reader["IdProducto"]),
            DetalleCantidad = reader["DetalleCantidad"] == DBNull.Value ? null : Convert.ToDecimal(reader["DetalleCantidad"]),
            DetalleUm = reader["DetalleUm"].ToString(),
            DetalleDescripcion = reader["DetalleDescripcion"].ToString(),
            DetalleCosto = reader["DetalleCosto"] == DBNull.Value ? null : Convert.ToDecimal(reader["DetalleCosto"]),
            DetallePrecio = reader["DetallePrecio"] == DBNull.Value ? null : Convert.ToDecimal(reader["DetallePrecio"]),
            DetalleImporte = reader["DetalleImporte"] == DBNull.Value ? null : Convert.ToDecimal(reader["DetalleImporte"]),
            DetalleEstado = reader["DetalleEstado"].ToString(),
            CantidadSaldo = reader["CantidadSaldo"] == DBNull.Value ? null : Convert.ToDecimal(reader["CantidadSaldo"]),
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
