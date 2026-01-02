using System.Data;
using System.Linq;
using Ecommerce.Application.Contracts.NotaPedido;
using Ecommerce.Domain;
using Ecommerce.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class NotaPedidoRepository : INotaPedido
{
    private readonly string _connectionString;
    private readonly AccesoDatos _accesoDatos = new();

    public NotaPedidoRepository()
    {
        var builder = WebApplication.CreateBuilder();
        _connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    }

    public string RegistrarOrden(string data)
    {
        var rpt = _accesoDatos.ejecutarComando("uspinsertarNotaB", "@ListaOrden", data);
        return string.IsNullOrEmpty(rpt) ? "error" : rpt;
    }

    public string EditarOrden(string data)
    {
        var rpt = _accesoDatos.ejecutarComando("uspEditarNotaPedido", "@ListaOrden", data);
        return string.IsNullOrEmpty(rpt) ? "error" : rpt;
    }

    public string Insertar(NotaPedido notaPedido)
    {
        using var con = new SqlConnection(_connectionString);
        con.Open();
        using var tx = con.BeginTransaction();

        var notaId = InsertOrUpdateNota(notaPedido, con, tx);
        if (notaId <= 0)
        {
            tx.Rollback();
            return notaPedido.NotaId > 0 ? "NOT_FOUND" : "error";
        }

        tx.Commit();
        return notaPedido.NotaId > 0 ? "UPDATED" : notaId.ToString();
    }

    public string InsertarConDetalle(NotaPedido notaPedido, IEnumerable<DetalleNota> detalles)
    {
        using var con = new SqlConnection(_connectionString);
        con.Open();
        using var tx = con.BeginTransaction();

        var notaId = InsertOrUpdateNota(notaPedido, con, tx);
        if (notaId <= 0)
        {
            tx.Rollback();
            return notaPedido.NotaId > 0 ? "NOT_FOUND" : "error";
        }

        var detalleList = detalles?.ToList() ?? new List<DetalleNota>();
        DeleteDetallesByNota(notaId, con, tx);
        foreach (var detalle in detalleList)
        {
            detalle.NotaId = notaId;
            InsertDetalle(detalle, con, tx);
        }

        tx.Commit();
        return notaId.ToString();
    }

    public bool Eliminar(long id)
    {
        using var con = new SqlConnection(_connectionString);
        con.Open();
        using var tx = con.BeginTransaction();

        DeleteDetallesByNota(id, con, tx);

        const string sql = "DELETE FROM NotaPedido WHERE NotaId = @Id";
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

    public NotaPedido? ObtenerPorId(long id)
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

    public IReadOnlyList<NotaPedido> ListarCrud(string? estado = null)
    {
        var sql = @"SELECT NotaId,
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
                    FROM NotaPedido";

        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.Text;
        if (!string.IsNullOrWhiteSpace(estado))
        {
            cmd.CommandText += " WHERE NotaEstado = @Estado";
            cmd.Parameters.AddWithValue("@Estado", estado);
        }

        cmd.CommandText += " ORDER BY NotaId DESC";

        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        using var reader = cmd.ExecuteReader();
        var lista = new List<NotaPedido>();
        while (reader.Read())
        {
            lista.Add(Map(reader));
        }
        return lista;
    }

    public IReadOnlyList<DetalleNota> ListarDetalle(long notaId)
    {
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
                             ORDER BY DetalleId";

        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("@NotaId", notaId);
        if (con.State == ConnectionState.Open) con.Close();
        con.Open();
        using var reader = cmd.ExecuteReader();
        var lista = new List<DetalleNota>();
        while (reader.Read())
        {
            lista.Add(MapDetalle(reader));
        }
        return lista;
    }

    public IReadOnlyList<EListaNota> Listar()
    {
        var lista = new List<EListaNota>();
        var rpt = _accesoDatos.ejecutarComando("uspListaOrdenWeb");
        if (!string.IsNullOrEmpty(rpt))
        {
            lista = Cadena.AlistaCamposNota(rpt);
        }
        return lista;
    }

    private long InsertOrUpdateNota(NotaPedido notaPedido, SqlConnection con, SqlTransaction tx)
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

            using var cmd = new SqlCommand(sqlUpdate, con, tx);
            cmd.CommandTimeout = 300;
            cmd.CommandType = CommandType.Text;
            AddParameters(cmd, notaPedido);
            cmd.Parameters.AddWithValue("@NotaId", notaPedido.NotaId);
            var rows = cmd.ExecuteNonQuery();
            return rows > 0 ? notaPedido.NotaId : 0;
        }
        else
        {
            const string sqlInsert = @"INSERT INTO NotaPedido
                                            (NotaDocu,
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
                                             Deposito)
                                       VALUES (@NotaDocu,
                                               @ClienteId,
                                               @NotaFecha,
                                                @NotaUsuario,
                                                @NotaFormaPago,
                                                @NotaCondicion,
                                                @NotaFechaPago,
                                                @NotaDireccion,
                                                @NotaTelefono,
                                               @NotaSubtotal,
                                               @NotaMovilidad,
                                               @NotaDescuento,
                                               @NotaTotal,
                                               @NotaAcuenta,
                                               @NotaSaldo,
                                               @NotaAdicional,
                                               @NotaTarjeta,
                                               @NotaPagar,
                                               @NotaEstado,
                                               @CompaniaId,
                                               @NotaEntrega,
                                               @ModificadoPor,
                                               @FechaEdita,
                                               @NotaConcepto,
                                               @NotaSerie,
                                               @NotaNumero,
                                               @NotaGanancia,
                                               @ICBPER,
                                               @CajaId,
                                               @EntidadBancaria,
                                               @NroOperacion,
                                               @Efectivo,
                                               @Deposito);
                                       SELECT SCOPE_IDENTITY();";

            using var cmd = new SqlCommand(sqlInsert, con, tx);
            cmd.CommandTimeout = 300;
            cmd.CommandType = CommandType.Text;
            AddParameters(cmd, notaPedido);
            var result = cmd.ExecuteScalar();
            return result == null ? 0 : Convert.ToInt64(result);
        }
    }

    private static void InsertDetalle(DetalleNota detalle, SqlConnection con, SqlTransaction tx)
    {
        const string sql = @"INSERT INTO DetallePedido
                                    (NotaId,
                                     IdProducto,
                                     DetalleCantidad,
                                     DetalleUm,
                                     DetalleDescripcion,
                                     DetalleCosto,
                                     DetallePrecio,
                                     DetalleImporte,
                                     DetalleEstado,
                                     CantidadSaldo,
                                     ValorUM)
                             VALUES (@NotaId,
                                     @IdProducto,
                                     @DetalleCantidad,
                                     @DetalleUm,
                                     @DetalleDescripcion,
                                     @DetalleCosto,
                                     @DetallePrecio,
                                     @DetalleImporte,
                                     @DetalleEstado,
                                     @CantidadSaldo,
                                     @ValorUM)";

        using var cmd = new SqlCommand(sql, con, tx);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.Text;
        AddParam(cmd, "@NotaId", detalle.NotaId);
        AddParam(cmd, "@IdProducto", detalle.IdProducto);
        AddParam(cmd, "@DetalleCantidad", detalle.DetalleCantidad);
        AddParam(cmd, "@DetalleUm", detalle.DetalleUm);
        AddParam(cmd, "@DetalleDescripcion", detalle.DetalleDescripcion);
        AddParam(cmd, "@DetalleCosto", detalle.DetalleCosto);
        AddParam(cmd, "@DetallePrecio", detalle.DetallePrecio);
        AddParam(cmd, "@DetalleImporte", detalle.DetalleImporte);
        AddParam(cmd, "@DetalleEstado", detalle.DetalleEstado);
        AddParam(cmd, "@CantidadSaldo", detalle.CantidadSaldo);
        AddParam(cmd, "@ValorUM", detalle.ValorUM);
        cmd.ExecuteNonQuery();
    }

    private static void DeleteDetallesByNota(long notaId, SqlConnection con, SqlTransaction tx)
    {
        const string sql = "DELETE FROM DetallePedido WHERE NotaId = @NotaId";
        using var cmd = new SqlCommand(sql, con, tx);
        cmd.CommandTimeout = 300;
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("@NotaId", notaId);
        cmd.ExecuteNonQuery();
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
            if (DateTime.TryParse(s, out var parsed)) return parsed;
            return null;
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
}
