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
    public ActionResult<IReadOnlyList<EListaNota>> ListarNota()
    {
        return Ok(_mediator.Listar());
    }

    [AllowAnonymous]
    [HttpGet("crud", Name = "GetNotaPedidoCrud")]
    [ProducesResponseType(typeof(IReadOnlyList<NotaPedido>), (int)HttpStatusCode.OK)]
    public ActionResult<IReadOnlyList<NotaPedido>> ListarNotaCrud([FromQuery] string? estado = null)
    {
        return Ok(_mediator.ListarCrud(estado));
    }

    [AllowAnonymous]
    [HttpGet("{id:long}", Name = "GetNotaPedidoById")]
    [ProducesResponseType(typeof(NotaPedido), (int)HttpStatusCode.OK)]
    public ActionResult<NotaPedido?> ObtenerNotaPedido(long id)
    {
        var nota = _mediator.ObtenerPorId(id);
        if (nota is null) return NotFound();
        return Ok(nota);
    }

    [AllowAnonymous]
    [HttpGet("{id:long}/detalles", Name = "GetNotaPedidoDetalles")]
    [ProducesResponseType(typeof(IReadOnlyList<DetalleNota>), (int)HttpStatusCode.OK)]
    public ActionResult<IReadOnlyList<DetalleNota>> ObtenerDetalles(long id)
    {
        return Ok(_mediator.ListarDetalle(id));
    }

    [AllowAnonymous]
    [HttpPost("register", Name = "RegisterNotaPedido")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult RegistrarNotaPedido([FromBody] NotaPedido notaPedido)
    {
        return Ok(_mediator.Insertar(notaPedido));
    }

    [AllowAnonymous]
    [HttpPost("register-with-detail", Name = "RegisterNotaPedidoConDetalle")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult RegistrarNotaPedidoConDetalle([FromBody] JsonElement body)
    {
        var hasNotaObject = body.ValueKind == JsonValueKind.Object &&
                            (body.TryGetProperty("Nota", out _) || body.TryGetProperty("nota", out _));
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
            Console.WriteLine("Preview register-with-detail (nota/detalles -> orden-string): " + vdataNota);
            return Ok(new
            {
                mode = "nota-detalle",
                data = vdataNota,
                nota = request.Nota,
                detalles,
                info = "Preview only, not persisted"
            });
        }

        var vdata = BuildOrdenPayload(body);
        Console.WriteLine("Preview register-with-detail (orden-string): " + vdata);
        return Ok(new
        {
            mode = "orden-string",
            data = vdata,
            info = "Preview only, not persisted"
        });
    }

    [AllowAnonymous]
    [HttpDelete("{id:long}", Name = "EliminarNotaPedido")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult EliminarNotaPedido(long id)
    {
        return Ok(_mediator.Eliminar(id));
    }

    [AllowAnonymous]
    [HttpPost("crearOrden", Name = "CrearOrden")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult RegistrarOrden(JsonElement body)
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
            return Ok(_mediator.RegistrarOrden(vdataNota));
        }

        var vdata = BuildOrdenPayload(body);
        return Ok(_mediator.RegistrarOrden(vdata));
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
            Format2(0m),
            string.Empty,
            "PENDIENTE",
            Format2(subtotal),
            Format2(igv),
            "0",
            Format2(icbper),
            Format2(subtotal),
            Format2(0m),
            "0"
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
                "E" // AplicaINV
            };
            vdata += string.Join("|", detailFields);
            if (i < detalleList.Count - 1) vdata += ";";
        }
        vdata += "[";
        return vdata;
    }

    private string BuildOrdenPayload(JsonElement body)
    {
        string json = JsonSerializer.Serialize(body);
        dynamic res = JObject.Parse(json);
        dynamic producto = (JObject)res["requestDetalle"];

        Console.WriteLine("aramirez Total:" + GetFirstDecimal(res, 0m, "Total", "NotaTotal"));
        Console.WriteLine("aramirez Items:" + GetFirstDecimal(res, 0m, "Items"));
        Console.WriteLine("aramirez Detalle:" + producto);

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
            notaIdbr
        };

        var vdata = string.Join("|", headerFields) + "[";

        var count = Convert.ToInt32(GetFirstDecimal(res, 0m, "Items"));
        for (int i = 0; i < count; i++)
        {
            var fila = i.ToString();
            var item = producto[fila];
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
        Console.WriteLine("aramirez DES: " + vdata);
        return vdata;
    }

    private static string Format2(decimal value)
    {
        return value.ToString("0.##", CultureInfo.InvariantCulture);
    }

    private static string Format4(decimal value)
    {
        return value.ToString("0.####", CultureInfo.InvariantCulture);
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
