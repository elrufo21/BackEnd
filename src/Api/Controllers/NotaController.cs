using System.Net;
using Ecommerce.Application.Contracts.NotaPedido;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class NotaController : ControllerBase
{
    private readonly INotaPedido _mediator;
    public NotaController(INotaPedido mediador)
    {
        _mediator = mediador;
    }

    [AllowAnonymous]
    [HttpGet("list", Name = "GetNotaList")]
    [ProducesResponseType(typeof(IReadOnlyList<EListaNota>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<EListaNota>>> ListarNota(
        [FromQuery] DateTime? fechaInicio,
        [FromQuery] DateTime? fechaFin,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        if (!fechaInicio.HasValue || !fechaFin.HasValue)
        {
            return BadRequest("Debe enviar fechaInicio y fechaFin en formato YYYY-MM-DD.");
        }

        if (fechaInicio.Value.Date > fechaFin.Value.Date)
        {
            return BadRequest("fechaInicio no puede ser mayor que fechaFin.");
        }

        return Ok(await _mediator.ListarAsync(fechaInicio.Value, fechaFin.Value, page, pageSize, cancellationToken));
    }

    [AllowAnonymous]
    [HttpGet("crud", Name = "GetNotaPedidoCrud")]
    [ProducesResponseType(typeof(IReadOnlyList<NotaPedido>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<NotaPedido>>> ListarNotaCrud(
        [FromQuery] string? estado = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.ListarCrudAsync(estado, page, pageSize, cancellationToken));
    }

    [AllowAnonymous]
    [HttpGet("{id:long}", Name = "GetNotaPedidoById")]
    [ProducesResponseType(typeof(NotaPedido), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<NotaPedido?>> ObtenerNotaPedido(long id, CancellationToken cancellationToken)
    {
        var nota = await _mediator.ObtenerPorIdAsync(id, cancellationToken);
        if (nota is null) return NotFound();
        return Ok(nota);
    }

    [AllowAnonymous]
    [HttpGet("sp/{id:long}", Name = "GetNotaPedidoByStoredProcedure")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> ObtenerNotaPedidoSp(long id, CancellationToken cancellationToken)
    {
        var resultado = await _mediator.ObtenerNotaPedidoSpAsync(id, cancellationToken);
        if (string.Equals(resultado, "FORMATO_INVALIDO", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { resultado });
        }

        if (string.IsNullOrWhiteSpace(resultado) || resultado == "~")
        {
            return NotFound(new { resultado = "~" });
        }

        return Ok(new { resultado });
    }

    [AllowAnonymous]
    [HttpGet("{id:long}/detalles", Name = "GetNotaPedidoDetalles")]
    [ProducesResponseType(typeof(IReadOnlyList<DetalleNota>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<DetalleNota>>> ObtenerDetalles(
        long id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.ListarDetalleAsync(id, page, pageSize, cancellationToken));
    }

    [Authorize]
    [HttpPost("register", Name = "RegisterNotaPedido")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> RegistrarNotaPedido([FromBody] NotaPedido notaPedido, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.InsertarAsync(notaPedido, cancellationToken));
    }

    [Authorize]
    [HttpPost("register-with-detail", Name = "RegisterNotaPedidoConDetalle")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> RegistrarNotaPedidoConDetalle([FromBody] JsonElement body, CancellationToken cancellationToken)
    {
        if (body.ValueKind != JsonValueKind.Object)
        {
            return BadRequest("Se requiere un JSON objeto con Nota/nota o requestDetalle.");
        }

        var hasNotaObject = body.TryGetProperty("Nota", out _) || body.TryGetProperty("nota", out _);
        if (hasNotaObject)
        {
            var request = JsonSerializer.Deserialize<NotaPedidoConDetalleRequest>(body.GetRawText(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (request?.Nota is null)
            {
                return BadRequest("NotaPedido requerida.");
            }
            var detalles = request.Detalles ?? new List<DetalleNota>();
            var vdataNota = BuildOrdenPayload(request.Nota, detalles);
            return Ok(await _mediator.RegistrarOrdenAsync(vdataNota, cancellationToken));
        }

        var vdata = BuildOrdenPayload(body);
        return Ok(await _mediator.RegistrarOrdenAsync(vdata, cancellationToken));
    }

    [Authorize]
    [HttpDelete("{id:long}", Name = "EliminarNotaPedido")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> EliminarNotaPedido(long id, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.EliminarAsync(id, cancellationToken));
    }

    [AllowAnonymous]
    [HttpPost("crearOrden", Name = "CrearOrden")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> RegistrarOrden(JsonElement body, CancellationToken cancellationToken)
    {
        if (body.ValueKind != JsonValueKind.Object)
        {
            return BadRequest("Se requiere un JSON objeto con Nota/nota o requestDetalle.");
        }

        var hasNotaObject = body.TryGetProperty("Nota", out _) || body.TryGetProperty("nota", out _);
        if (hasNotaObject)
        {
            var request = JsonSerializer.Deserialize<NotaPedidoConDetalleRequest>(body.GetRawText(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (request?.Nota is null)
            {
                return BadRequest("NotaPedido requerida.");
            }
            var detalles = request.Detalles ?? new List<DetalleNota>();
            var vdataNota = BuildOrdenPayload(request.Nota, detalles);
            return Ok(await _mediator.RegistrarOrdenAsync(vdataNota, cancellationToken));
        }

        var vdata = BuildOrdenPayload(body);
        return Ok(await _mediator.RegistrarOrdenAsync(vdata, cancellationToken));
    }

    [Authorize]
    [HttpPut("editarOrden", Name = "EditarOrden")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> EditarOrden(JsonElement body, CancellationToken cancellationToken)
    {
        if (body.ValueKind != JsonValueKind.Object)
        {
            return BadRequest("Se requiere un JSON objeto con Nota/nota o requestDetalle.");
        }

        var hasNotaObject = body.TryGetProperty("Nota", out _) || body.TryGetProperty("nota", out _);
        if (hasNotaObject)
        {
            var request = JsonSerializer.Deserialize<NotaPedidoConDetalleRequest>(body.GetRawText(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (request?.Nota is null)
            {
                return BadRequest("NotaPedido requerida.");
            }
            var detalles = request.Detalles ?? new List<DetalleNota>();
            var vdataNota = BuildEditarPayload(request.Nota, detalles);
            return Ok(await _mediator.EditarOrdenAsync(vdataNota, cancellationToken));
        }

        var vdata = BuildEditarPayload(body);
        return Ok(await _mediator.EditarOrdenAsync(vdata, cancellationToken));
    }

    private string BuildOrdenPayload(NotaPedido nota, IEnumerable<DetalleNota> detalles)
    {
        var detalleList = detalles == null ? new List<DetalleNota>() : new List<DetalleNota>(detalles);

        var total = nota.NotaTotal ?? 0m;
        var subtotal = nota.NotaSubtotal ?? 0m;
        var movilidad = nota.NotaMovilidad ?? 0m;
        var descuento = nota.NotaDescuento ?? 0m;
        var acuenta = nota.NotaAcuenta ?? 0m;
        var saldo = nota.NotaSaldo ?? 0m;
        var adicional = nota.NotaAdicional ?? 0m;
        var tarjeta = nota.NotaTarjeta ?? 0m;
        var pagar = nota.NotaPagar ?? total;
        var ganancia = nota.NotaGanancia ?? 0m;
        var icbper = nota.ICBPER ?? 0m;
        var igv = total - subtotal;

        var xDocumento = string.IsNullOrWhiteSpace(nota.NotaDocu) ? "BOLETA" : nota.NotaDocu!;
        var xserie = nota.NotaSerie ?? string.Empty;
        var numero = nota.NotaNumero ?? string.Empty;
        // Defaults for uspinsertarNotaB fields not present in NotaPedido payloads
        var docuAdicional = 0m;
        var docuHash = string.Empty;
        var estadoSunat = "PENDIENTE";
        var docuSubtotal = subtotal;
        var docuIgv = igv;
        var usuarioId = "7";
        var docuGravada = subtotal;
        var docuDescuento = 0m;
        var notaIdbr = nota.NotaId > 0 ? nota.NotaId.ToString() : "0";
        var entidadBancaria = nota.EntidadBancaria ?? "-";
        var nroOperacion = nota.NroOperacion ?? string.Empty;
        var efectivo = nota.Efectivo ?? pagar;
        var deposito = nota.Deposito ?? 0m;
        var clienteRazon = string.Empty;
        var clienteRuc = string.Empty;
        var clienteDni = string.Empty;
        var direccionFiscal = string.Empty;

        var headerFields = new List<string?>
        {
            xDocumento,
            nota.ClienteId?.ToString(),
            nota.NotaUsuario,
            nota.NotaFormaPago,
            nota.NotaCondicion,
            nota.NotaDireccion,
            nota.NotaTelefono,
            Format2(subtotal),
            Format2(movilidad),
            Format2(descuento),
            Format2(total),
            Format2(acuenta),
            Format2(saldo),
            Format2(adicional),
            Format2(tarjeta),
            Format2(pagar),
            nota.NotaEstado ?? "PENDIENTE",
            nota.CompaniaId?.ToString(),
            nota.NotaEntrega,
            nota.NotaConcepto,
            xserie,
            numero,
            Format2(ganancia),
            Letras.enletras(total.ToString("N2")) + "  SOLES",
            Format2(docuAdicional),
            docuHash,
            estadoSunat,
            Format2(docuSubtotal),
            Format2(docuIgv),
            usuarioId,
            Format2(icbper),
            Format2(docuGravada),
            Format2(docuDescuento),
            notaIdbr,
            entidadBancaria,
            nroOperacion,
            Format2(efectivo),
            Format2(deposito),
            clienteRazon,
            clienteRuc,
            clienteDni,
            direccionFiscal
        };

        var vdata = string.Join("|", headerFields) + "[";

        for (int i = 0; i < detalleList.Count; i++)
        {
            var item = detalleList[i];
            var detailFields = new[]
            {
                Convert.ToInt32(item.IdProducto ?? 0).ToString(),
                string.Empty, // CodigoPro (no disponible en modelo)
                Format2(item.DetalleCantidad ?? 0m),
                item.DetalleUm ?? string.Empty,
                item.DetalleDescripcion ?? string.Empty,
                Format4(item.DetalleCosto ?? 0m),
                Format2(item.DetallePrecio ?? 0m),
                Format2(item.DetalleImporte ?? 0m),
                item.DetalleEstado ?? "PENDIENTE",
                Format4(item.ValorUM ?? 0m),
                "S" // AplicaINV
            };
            vdata += string.Join("|", detailFields);
            if (i < detalleList.Count - 1) vdata += ";";
        }
        vdata += "[";
        return vdata;
    }

    private string BuildEditarPayload(NotaPedido nota, IEnumerable<DetalleNota> detalles)
    {
        var detalleList = detalles == null ? new List<DetalleNota>() : new List<DetalleNota>(detalles);

        var headerFields = new List<string?>
        {
            nota.NotaId > 0 ? nota.NotaId.ToString() : "0",
            string.IsNullOrWhiteSpace(nota.NotaDocu) ? "BOLETA" : nota.NotaDocu!,
            nota.ClienteId?.ToString() ?? "0",
            FormatDateForSql(nota.NotaFecha),
            nota.NotaUsuario ?? string.Empty,
            nota.NotaFormaPago ?? string.Empty,
            nota.NotaCondicion ?? string.Empty
        };

        var detailParts = new List<string>();

        foreach (var item in detalleList)
        {
            var detailFields = new[]
            {
                Convert.ToInt32(item.IdProducto ?? 0).ToString(),
                Format2(item.DetalleCantidad ?? 0m),
                item.DetalleUm ?? string.Empty,
                item.DetalleDescripcion ?? string.Empty,
                Format2(item.DetalleCosto ?? 0m),
                Format2(item.DetallePrecio ?? 0m),
                Format2(item.DetalleImporte ?? 0m),
                item.DetalleEstado ?? "PENDIENTE",
                "0"
            };
            detailParts.Add(string.Join("|", detailFields));
        }

        return string.Join("|", headerFields) + "[" + string.Join(";", detailParts);
    }

    private string BuildOrdenPayload(JsonElement body)
    {
        string json = JsonSerializer.Serialize(body);
        dynamic res = JObject.Parse(json);
        var productoToken = res["requestDetalle"] ?? res["requestdetalle"] ?? res["detalles"] ?? res["Detalles"];
        var productoArray = productoToken as JArray;
        var producto = productoToken as JObject ?? new JObject();


        var docu = GetFirstString(res, "Documento", "NotaDocu", "Docu");
        if (string.IsNullOrWhiteSpace(docu)) docu = "BOLETA";
        var clienteId = GetFirstString(res, "ClienteId");
        var usuario = GetFirstString(res, "Usuario", "NotaUsuario");
        var formaPago = GetFirstString(res, "FormaPago", "NotaFormaPago");
        var condicion = GetFirstString(res, "Condicion", "NotaCondicion");
        var direccion = GetFirstString(res, "Direccion", "NotaDireccion");
        var telefono = GetFirstString(res, "Telefono", "NotaTelefono");
        var subtotal = GetFirstDecimal(res, 0m, "SubTotal", "NotaSubtotal", "DocuSubtotal");
        var movilidad = GetFirstDecimal(res, 0m, "Movilidad", "NotaMovilidad");
        var descuento = GetFirstDecimal(res, 0m, "Descuento", "NotaDescuento", "DocuDescuento");
        var total = GetFirstDecimal(res, 0m, "Total", "NotaTotal");
        var acuenta = GetFirstDecimal(res, 0m, "Acuenta", "NotaAcuenta");
        var saldo = GetFirstDecimal(res, 0m, "Saldo", "NotaSaldo");
        var adicional = GetFirstDecimal(res, 0m, "Adicional", "NotaAdicional", "DocuAdicional");
        var tarjeta = GetFirstDecimal(res, 0m, "Tarjeta", "NotaTarjeta");
        var pagar = GetFirstDecimal(res, total, "Pagar", "NotaPagar", "PagoTotal");
        var estado = GetFirstString(res, "Estado", "NotaEstado", "EstadoSunat");
        if (string.IsNullOrWhiteSpace(estado)) estado = "PENDIENTE";
        var companiaId = GetFirstString(res, "CompaniaId");
        var entrega = GetFirstString(res, "Entrega", "NotaEntrega");
        var concepto = GetFirstString(res, "Concepto", "NotaConcepto");
        var serie = GetFirstString(res, "NotaSerie", "Serie");
        var numero = GetFirstString(res, "NotaNumero", "Numero");
        var ganancia = GetFirstDecimal(res, 0m, "Ganancia", "NotaGanancia");
        var letra = Letras.enletras(total.ToString("N2")) + "  SOLES";
        var docuAdicional = GetFirstDecimal(res, 0m, "DocuAdicional", "AdicionalDoc");
        var docuHash = GetFirstString(res, "DocuHash", "Hash");
        var estadoSunat = GetFirstString(res, "EstadoSunat");
        if (string.IsNullOrWhiteSpace(estadoSunat)) estadoSunat = "PENDIENTE";
        var docuSubtotal = GetFirstDecimal(res, subtotal, "DocuSubtotal", "DocuSubTotal");
        var igv = GetFirstDecimal(res, total - subtotal, "IGV", "DocuIGV");
        var usuarioId = GetFirstString(res, "UsuarioId");
        var icbper = GetFirstDecimal(res, 0m, "ICBPER");
        var docuGravada = GetFirstDecimal(res, subtotal, "DocuGravada");
        var docuDescuento = GetFirstDecimal(res, 0m, "DocuDescuento");
        var notaIdbr = GetFirstString(res, "NotaIDBR", "NotaIdbr", "IDBR");
        if (string.IsNullOrWhiteSpace(notaIdbr)) notaIdbr = "0";
        var entidadBancaria = GetFirstString(res, "EntidadBancaria");
        if (string.IsNullOrWhiteSpace(entidadBancaria)) entidadBancaria = "-";
        var nroOperacion = GetFirstString(res, "NroOperacion", "NumeroOperacion");
        var efectivo = GetFirstDecimal(res, pagar, "Efectivo");
        var deposito = GetFirstDecimal(res, 0m, "Deposito");
        var clienteRazon = GetFirstString(res, "ClienteRazon", "ClienteRazonSocial", "RazonSocial");
        var clienteRuc = GetFirstString(res, "ClienteRuc", "Ruc");
        var clienteDni = GetFirstString(res, "ClienteDni", "Dni");
        var direccionFiscal = GetFirstString(res, "DireccionFiscal", "ClienteDireccion", "Direccion");

        var headerFields = new List<string?>
        {
            docu,
            clienteId,
            usuario,
            formaPago,
            condicion,
            direccion,
            telefono,
            Format2(subtotal),
            Format2(movilidad),
            Format2(descuento),
            Format2(total),
            Format2(acuenta),
            Format2(saldo),
            Format2(adicional),
            Format2(tarjeta),
            Format2(pagar),
            estado,
            companiaId,
            entrega,
            concepto,
            serie,
            numero,
            Format2(ganancia),
            letra,
            Format2(docuAdicional),
            docuHash,
            estadoSunat,
            Format2(docuSubtotal),
            Format2(igv),
            usuarioId,
            Format2(icbper),
            Format2(docuGravada),
            Format2(docuDescuento),
            notaIdbr,
            entidadBancaria,
            nroOperacion,
            Format2(efectivo),
            Format2(deposito),
            clienteRazon,
            clienteRuc,
            clienteDni,
            direccionFiscal
        };

        var vdata = string.Join("|", headerFields) + "[";

        var count = Convert.ToInt32(GetFirstDecimal(res, 0m, "Items"));
        if (count == 0)
        {
            if (productoArray != null) count = productoArray.Count;
            else count = producto.Properties().Count();
        }
        for (int i = 0; i < count; i++)
        {
            JToken? itemToken = null;
            if (productoArray != null)
            {
                if (i < productoArray.Count) itemToken = productoArray[i];
            }
            else
            {
                var fila = i.ToString();
                itemToken = producto[fila];
            }
            if (itemToken == null) continue;
            var item = itemToken;
            var detailFields = new[]
            {
                Convert.ToInt32(GetFirstDecimal(item, 0m, "productId", "IdProducto")).ToString(),
                GetFirstString(item, "codigoPro", "CodigoPro", "codigo", "Codigo"),
                Format2(GetFirstDecimal(item, 0m, "cantidad", "DetalleCantidad")),
                GetFirstString(item, "unidad", "DetalleUm"),
                GetFirstString(item, "producto", "DetalleDescripcion"),
                Format4(GetFirstDecimal(item, 0m, "costo", "DetalleCosto")),
                Format2(GetFirstDecimal(item, 0m, "precio", "DetallePrecio")),
                Format2(GetFirstDecimal(item, 0m, "importe", "DetalleImporte")),
                string.IsNullOrWhiteSpace(GetFirstString(item, "DetalleEstado", "estado"))
                    ? "PENDIENTE"
                    : GetFirstString(item, "DetalleEstado", "estado"),
                Format4(GetFirstDecimal(item, 0m, "valorUM", "ValorUM")),
                string.IsNullOrWhiteSpace(GetFirstString(item, "AplicaINV", "aplicaInv", "aplicaINV"))
                    ? "E"
                    : GetFirstString(item, "AplicaINV", "aplicaInv", "aplicaINV")
            };
            vdata += string.Join("|", detailFields);
            if (i < count - 1) vdata += ";";
        }

        vdata += "[";
        return vdata;
    }

    private string BuildEditarPayload(JsonElement body)
    {
        string json = JsonSerializer.Serialize(body);
        dynamic res = JObject.Parse(json);
        var productoToken = res["requestDetalle"] ?? res["requestdetalle"] ?? res["detalles"] ?? res["Detalles"];
        var productoArray = productoToken as JArray;
        var producto = productoToken as JObject ?? new JObject();

        var notaId = Convert.ToInt32(GetFirstDecimal(res, 0m, "NotaId", "NotaIDBR", "NotaIdbr", "IDBR"));
        var docu = GetFirstString(res, "Documento", "NotaDocu", "Docu");
        if (string.IsNullOrWhiteSpace(docu)) docu = "BOLETA";
        var clienteId = GetFirstString(res, "ClienteId");
        if (string.IsNullOrWhiteSpace(clienteId)) clienteId = "0";
        var notaFecha = NormalizeDateValue(GetFirstString(res, "NotaFecha", "Fecha", "fecha", "notaFecha"));
        var usuario = GetFirstString(res, "Usuario", "NotaUsuario");
        var formaPago = GetFirstString(res, "FormaPago", "NotaFormaPago");
        var condicion = GetFirstString(res, "Condicion", "NotaCondicion");

        var headerFields = new List<string?>
        {
            notaId.ToString(),
            docu,
            clienteId,
            notaFecha,
            usuario,
            formaPago,
            condicion
        };

        var detailParts = new List<string>();

        var count = Convert.ToInt32(GetFirstDecimal(res, 0m, "Items"));
        if (count == 0)
        {
            if (productoArray != null) count = productoArray.Count;
            else count = producto.Properties().Count();
        }
        for (int i = 0; i < count; i++)
        {
            JToken? itemToken = null;
            if (productoArray != null)
            {
                if (i < productoArray.Count) itemToken = productoArray[i];
            }
            else
            {
                var fila = i.ToString();
                itemToken = producto[fila];
            }
            if (itemToken == null) continue;
            var item = itemToken;

            var detailFields = new[]
            {
                Convert.ToInt32(GetFirstDecimal(item, 0m, "productId", "IdProducto")).ToString(),
                Format2(GetFirstDecimal(item, 0m, "cantidad", "DetalleCantidad")),
                GetFirstString(item, "unidad", "DetalleUm"),
                GetFirstString(item, "producto", "DetalleDescripcion"),
                Format2(GetFirstDecimal(item, 0m, "costo", "DetalleCosto")),
                Format2(GetFirstDecimal(item, 0m, "precio", "DetallePrecio")),
                Format2(GetFirstDecimal(item, 0m, "importe", "DetalleImporte")),
                string.IsNullOrWhiteSpace(GetFirstString(item, "DetalleEstado", "estado"))
                    ? "PENDIENTE"
                    : GetFirstString(item, "DetalleEstado", "estado"),
                "0"
            };
            detailParts.Add(string.Join("|", detailFields));
        }

        return string.Join("|", headerFields) + "[" + string.Join(";", detailParts);
    }

    private static string Format2(decimal value)
    {
        return value.ToString("0.##", CultureInfo.InvariantCulture);
    }

    private static string Format4(decimal value)
    {
        return value.ToString("0.####", CultureInfo.InvariantCulture);
    }

    private static string FormatDateForSql(DateTime? value)
    {
        return (value ?? DateTime.Now).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    private static string NormalizeDateValue(string rawDate)
    {
        if (DateTime.TryParse(rawDate, out var parsed))
        {
            return parsed.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        return DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    private static string GetFirstString(dynamic obj, params string[] names)
    {
        foreach (var name in names)
        {
            try
            {
                var token = obj[name];
                if (token != null) return token.ToString();
            }
            catch { }
        }
        return string.Empty;
    }

    private static decimal GetFirstDecimal(dynamic obj, decimal fallback, params string[] names)
    {
        foreach (var name in names)
        {
            try
            {
                var token = obj[name];
                if (token != null) return Convert.ToDecimal(token);
            }
            catch { }
        }
        return fallback;
    }
}

public class NotaPedidoConDetalleRequest
{
    public NotaPedido? Nota { get; set; }
    public List<DetalleNota>? Detalles { get; set; }
}
