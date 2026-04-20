using System.Net;
using Ecommerce.Api.Legacy;
using Ecommerce.Application.Contracts.Clientes;
using Ecommerce.Application.Contracts.Companias;
using Ecommerce.Application.Contracts.Lineas;
using Ecommerce.Application.Contracts.NotaPedido;
using Ecommerce.Application.Contracts.Productos;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using BusinessEntities;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class NotaController : ControllerBase
{
    private enum ModoEnvioBaja
    {
        ResumenBoletas,
        ComunicacionBaja,
        MezclaNoSoportada
    }

    private enum EstadoResultadoSunat
    {
        MantenerPendiente,
        Rechazado
    }

    private const long MaxCertificateSizeBytes = 2 * 1024 * 1024; // 2 MB
    private const decimal PorcentajeIgvDefault = 18m;
    private const int DecimalesSunatPrecioUnitario = 10;
    private const int DecimalesSunatMonto = 2;
    private const string CodigoSunatFacturaFallback = "50161509";
    private const string CodigoSunatBoletaFallback = "50161509";
    private const string CodigoSunatNotaCreditoFallback = "01010101";
    private const string DocuConceptoNotaCreditoDefault = "ANULACION DE LA OPERACION";
    private const string MensajeTicketNoGenerado = "NO SE GENERO EL TICKET DE SUNAT,SE RETORNARAN LAS BOLETAS...FAVOR DE ENVIARLO DENUEVO EN UNOS MINUTOS";
    private static readonly HashSet<string> AllowedCertificateExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".p12",
        ".pfx"
    };
    private static readonly HashSet<string> CodigosSunatRetornoBoletas = new(StringComparer.OrdinalIgnoreCase)
    {
        "0133",
        "0111",
        "133",
        "111",
        "109",
        "135",
        "0109",
        "2220",
        "2018",
        "100",
        "2223",
        "0135",
        "200",
        "2663"
    };

    private readonly INotaPedido _mediator;
    private readonly ICliente _clientes;
    private readonly ICompania _companias;
    private readonly IProducto _productos;
    private readonly ILinea _lineas;
    private readonly IConfiguration _configuration;
    private readonly ICpeGateway _cpeGateway;
    private readonly ILogger<NotaController> _logger;

    public NotaController(
        INotaPedido mediador,
        ICliente clientes,
        ICompania companias,
        IProducto productos,
        ILinea lineas,
        IConfiguration configuration,
        ICpeGateway cpeGateway,
        ILogger<NotaController> logger)
    {
        _mediator = mediador;
        _clientes = clientes;
        _companias = companias;
        _productos = productos;
        _lineas = lineas;
        _configuration = configuration;
        _cpeGateway = cpeGateway;
        _logger = logger;
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
    [HttpPost("lista-documentos", Name = "GetListaDocumentos")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ListarDocumentos([FromBody] ListaDocumentosRequest request, CancellationToken cancellationToken)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Data))
        {
            return BadRequest("Data es requerido.");
        }

        var resultado = await _mediator.ListarDocumentosAsync(request.Data, cancellationToken);
        return Content(resultado, "text/plain");
    }

    [AllowAnonymous]
    [HttpGet("ld-documentos", Name = "GetLdDocumentos")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ListarLdDocumentos(
        [FromQuery] DateTime? fechaInicio,
        [FromQuery] DateTime? fechaFin,
        CancellationToken cancellationToken)
    {
        if (!fechaInicio.HasValue || !fechaFin.HasValue)
        {
            return BadRequest("Debe enviar ambos parámetros: fechaInicio y fechaFin.");
        }

        if (fechaInicio.Value.Date > fechaFin.Value.Date)
        {
            return BadRequest("fechaInicio no puede ser mayor a fechaFin.");
        }

        var resultado = await _mediator.ListarLdDocumentosRangoAsync(fechaInicio.Value.Date, fechaFin.Value.Date, cancellationToken);
        return Content(resultado, "text/plain");
    }

    [AllowAnonymous]
    [HttpPost("ld-documentos", Name = "GetLdDocumentosLegacy")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ListarLdDocumentosLegacy([FromBody] LdDocumentosLegacyRequest? request, CancellationToken cancellationToken)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Data))
        {
            return BadRequest("Data es requerido en formato MM/dd/yyyy|MM/dd/yyyy.");
        }

        if (!TryObtenerRangoDesdeData(request.Data, out var fechaInicio, out var fechaFin, out var error))
        {
            return BadRequest(error);
        }

        var resultado = await _mediator.ListarLdDocumentosRangoAsync(fechaInicio, fechaFin, cancellationToken);
        return Content(resultado, "text/plain");
    }

    [AllowAnonymous]
    [HttpPost("lista-bajas", Name = "GetListaBajas")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ListarBajas([FromBody] ListaBajasRequest request, CancellationToken cancellationToken)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Data))
        {
            return BadRequest("Data es requerido.");
        }

        var resultado = await _mediator.ListarBajasAsync(request.Data, cancellationToken);
        return Content(resultado, "text/plain");
    }

    [AllowAnonymous]
    [HttpPost("anular-documento", Name = "AnularDocumento")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> AnularDocumento([FromBody] AnularDocumentoRequest? request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "Payload requerido."
            });
        }

        var listaOrden = string.IsNullOrWhiteSpace(request.ListaOrden)
            ? request.Data
            : request.ListaOrden;

        if (string.IsNullOrWhiteSpace(listaOrden))
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "ListaOrden es requerido."
            });
        }

        var resultado = await _mediator.AnularDocumentoAsync(listaOrden.Trim(), cancellationToken);
        return Ok(new
        {
            ok = string.Equals(resultado, "true", StringComparison.OrdinalIgnoreCase),
            resultado
        });
    }

    [AllowAnonymous]
    [HttpPost("boleta/anular-individual", Name = "AnularBoletaIndividualConNotaCredito")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    public async Task<IActionResult> AnularBoletaIndividualConNotaCredito(
        [FromBody] AnularBoletaIndividualRequest? request,
        CancellationToken cancellationToken)
    {
        var boletaSolicitada = string.Empty;
        if (request is null)
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "Payload requerido."
            });
        }

        if ((!request.DOCU_ID.HasValue || request.DOCU_ID.Value <= 0) &&
            string.IsNullOrWhiteSpace(request.NRO_DOCUMENTO_MODIFICA))
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "Debe enviar DOCU_ID o NRO_DOCUMENTO_MODIFICA para identificar la boleta."
            });
        }
        boletaSolicitada = (request.NRO_DOCUMENTO_MODIFICA ?? string.Empty).Trim();

        var (requestNc, statusCode, mensajePreparacion) = await ConstruirRequestAnulacionBoletaIndividualAsync(request, cancellationToken);
        if (requestNc is null)
        {
            return StatusCode(statusCode, new
            {
                ok = false,
                mensaje = mensajePreparacion
            });
        }

        requestNc = NormalizarRequestNotaCredito(requestNc);
        var errores = ValidarRequestNotaCredito(requestNc);
        if (errores.Count > 0)
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "No se pudo construir una nota de crédito válida para anular la boleta.",
                errores
            });
        }

        var tipoProceso = ParseTipoProceso(requestNc.TIPO_PROCESO);
        if (!tipoProceso.HasValue || tipoProceso.Value < 1 || tipoProceso.Value > 3)
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "TIPO_PROCESO inválido para enviar la nota de crédito.",
                errores = new[] { "TIPO_PROCESO debe ser 1 (producción), 2 (homologación) o 3 (beta)." }
            });
        }

        try
        {
            var respuestaSunat = await EjecutarEnvioNotaCreditoAsync(requestNc, tipoProceso.Value, cancellationToken);
            var aceptado = string.Equals(
                ObtenerValorNormalizadoRespuesta(respuestaSunat, "aceptado"),
                "true",
                StringComparison.OrdinalIgnoreCase);
            var codSunat = ObtenerValorNormalizadoRespuesta(respuestaSunat, "cod_sunat");

            if (!aceptado)
            {
                var mensaje = codSunat == "2116"
                    ? "SUNAT/OSE rechazó la NC porque para esta boleta exige referencia por TICKET (tipo 12), no por documento 03."
                    : "SUNAT/OSE rechazó la nota de crédito.";

                return StatusCode((int)HttpStatusCode.Conflict, new
                {
                    ok = false,
                    mensaje,
                    docu_id_boleta = requestNc.DOCU_ID,
                    boleta = boletaSolicitada,
                    nota_credito = requestNc.NRO_COMPROBANTE,
                    tipo_comprobante_modifica = requestNc.TIPO_COMPROBANTE_MODIFICA,
                    referencia_modifica = requestNc.NRO_DOCUMENTO_MODIFICA,
                    cod_sunat = codSunat,
                    msj_sunat = ObtenerValorNormalizadoRespuesta(respuestaSunat, "msj_sunat"),
                    sunat = respuestaSunat
                });
            }

            return Ok(new
            {
                ok = true,
                mensaje = "Se generó y envió la nota de crédito para anular la boleta individual.",
                docu_id_boleta = requestNc.DOCU_ID,
                boleta = boletaSolicitada,
                nota_credito = requestNc.NRO_COMPROBANTE,
                tipo_comprobante_modifica = requestNc.TIPO_COMPROBANTE_MODIFICA,
                referencia_modifica = requestNc.NRO_DOCUMENTO_MODIFICA,
                sunat = respuestaSunat
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al anular boleta individual via NC. DOCU_ID={DocuId}, NRO_DOCUMENTO_MODIFICA={NroDocumentoModifica}",
                requestNc.DOCU_ID,
                requestNc.NRO_DOCUMENTO_MODIFICA);

            return StatusCode((int)HttpStatusCode.InternalServerError, new
            {
                ok = false,
                mensaje = $"Error al anular la boleta vía nota de crédito: {ex.Message}"
            });
        }
    }

    [AllowAnonymous]
    [HttpPost("resumen/registrar", Name = "RegistrarResumenBoletas")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> RegistrarResumenBoletas([FromBody] RegistrarResumenBoletasRequest? request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest("Payload requerido.");
        }

        var listaOrden = string.IsNullOrWhiteSpace(request.ListaOrden)
            ? request.Data
            : request.ListaOrden;

        if (string.IsNullOrWhiteSpace(listaOrden))
        {
            return BadRequest("ListaOrden es requerido.");
        }

        var resultado = await _mediator.RegistrarResumenBoletasAsync(listaOrden.Trim(), cancellationToken);
        if (string.IsNullOrWhiteSpace(resultado))
        {
            return Ok(new
            {
                ok = true,
                mensaje = "Procedimiento ejecutado, pero no devolvió payload de salida.",
                resultado = "~"
            });
        }

        if (resultado == "~")
        {
            return Ok(new
            {
                ok = true,
                mensaje = "Procedimiento ejecutado. El SP devolvió '~' en el SELECT final.",
                resultado
            });
        }

        return Content(resultado, "text/plain");
    }

    [AllowAnonymous]
    [HttpGet("resumen/fecha", Name = "GetResumenPorFecha")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> ObtenerResumenPorFecha(
        [FromQuery] DateTime? fechaInicio,
        [FromQuery] DateTime? fechaFin,
        CancellationToken cancellationToken)
    {
        if (!fechaInicio.HasValue || !fechaFin.HasValue)
        {
            return BadRequest("Debe enviar fechaInicio y fechaFin en formato YYYY-MM-DD.");
        }

        if (fechaInicio.Value.Date > fechaFin.Value.Date)
        {
            return BadRequest("fechaInicio no puede ser mayor que fechaFin.");
        }

        var resultado = await _mediator.ResumenPorFechaAsync(fechaInicio.Value.Date, fechaFin.Value.Date, cancellationToken);
        if (string.IsNullOrWhiteSpace(resultado) || resultado == "~")
        {
            return NotFound(new { resultado = "~" });
        }

        return Content(resultado, "text/plain");
    }

    [AllowAnonymous]
    [HttpGet("resumen/secuencia/{companiaId}", Name = "GetSecuenciaResumen")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> TraerSecuenciaResumen(string companiaId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(companiaId))
        {
            return BadRequest("CompaniaId es requerido.");
        }

        var resultado = await _mediator.TraerSecuenciaResumenAsync(companiaId.Trim(), cancellationToken);
        return Ok(new { secuencia = resultado });
    }

    [AllowAnonymous]
    [HttpPost("resumen/inyectar-secuencia", Name = "InjectSecuenciaResumen")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> InyectarSecuenciaResumen([FromBody] JsonElement body, CancellationToken cancellationToken)
    {
        if (body.ValueKind != JsonValueKind.Object)
        {
            return BadRequest("Se requiere un JSON objeto.");
        }

        dynamic payload = JObject.Parse(body.GetRawText());
        var companiaId = GetFirstString(payload, "COMPANIA_ID", "CompaniaId", "companiaId");
        if (string.IsNullOrWhiteSpace(companiaId))
        {
            return BadRequest("COMPANIA_ID es requerido.");
        }

        var secuencia = await _mediator.TraerSecuenciaResumenAsync(companiaId.Trim(), cancellationToken);
        if (string.IsNullOrWhiteSpace(secuencia))
        {
            secuencia = "1";
        }

        payload["SECUENCIA"] = secuencia;

        var jsonFinal = payload.ToString(Newtonsoft.Json.Formatting.None);
        Console.WriteLine(jsonFinal);

        return Ok(payload);
    }

    [AllowAnonymous]
    [HttpPost("resumen/enviar-baja", Name = "EnviarResumenBaja")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> EnviarResumenBaja([FromBody] EnviarResumenBoletasRequest? request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "Payload requerido.",
                errores = new[] { "El body del request es obligatorio." }
            });
        }

        var requestBaja = NormalizarRequestParaBaja(request);
        var modoEnvio = ResolverModoEnvioBaja(requestBaja);

        if (modoEnvio == ModoEnvioBaja.MezclaNoSoportada)
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "No se puede enviar en un mismo lote boletas y documentos de baja RA.",
                errores = new[]
                {
                    "Si vas a anular boletas (tipo 03) se envian como resumen RC con statu=3.",
                    "Si vas a enviar comunicacion de baja RA, todos los detalles deben ser de documentos compatibles con RA."
                }
            });
        }

        if (modoEnvio == ModoEnvioBaja.ResumenBoletas)
        {
            return await EnviarResumenBoletas(requestBaja, cancellationToken);
        }

        var errores = ValidarRequestBaja(requestBaja);
        if (errores.Count > 0)
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "Existen campos obligatorios faltantes o inválidos para enviar la baja.",
                errores
            });
        }

        var tipoProceso = ParseTipoProceso(requestBaja.TIPO_PROCESO);
        if (!tipoProceso.HasValue || tipoProceso.Value < 1 || tipoProceso.Value > 3)
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "TIPO_PROCESO inválido.",
                errores = new[] { "TIPO_PROCESO debe ser 1 (producción), 2 (homologación) o 3 (beta)." }
            });
        }

        try
        {
            var rutaPfxNormalizada = ResolverRutaPfx(requestBaja.RUTA_PFX ?? string.Empty);
            var baja = MapearBajaLegacy(requestBaja, tipoProceso.Value, rutaPfxNormalizada);
            var respuestaLegacy = _cpeGateway.EnvioBaja(baja);
            var flgRta = ObtenerValorLegacy(respuestaLegacy, "flg_rta", "0");
            var codSunat = ObtenerValorLegacy(respuestaLegacy, "cod_sunat");
            var envioOk = string.Equals(flgRta, "1", StringComparison.Ordinal) && string.IsNullOrWhiteSpace(codSunat);

            object? registroBd;
            if (envioOk)
            {
                registroBd = await RegistrarResumenEnBaseDatosAsync(requestBaja, respuestaLegacy, cancellationToken);
            }
            else
            {
                registroBd = await RegistrarResultadoNoAceptadoResumenAsync(requestBaja, respuestaLegacy, cancellationToken);
            }

            return Ok(NormalizarRespuestaResumen(respuestaLegacy, registroBd: registroBd));
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, NormalizarRespuestaResumen(
                null,
                $"Error al enviar baja: {ex.Message}"));
        }
    }

    [AllowAnonymous]
    [HttpPost("resumen/enviar", Name = "EnviarResumenBoletas")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> EnviarResumenBoletas([FromBody] EnviarResumenBoletasRequest? request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "Payload requerido.",
                errores = new[] { "El body del request es obligatorio." }
            });
        }

        var errores = ValidarRequestResumen(request);
        if (errores.Count > 0)
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "Existen campos obligatorios faltantes o inválidos.",
                errores
            });
        }

        var tipoProceso = ParseTipoProceso(request.TIPO_PROCESO);
        if (!tipoProceso.HasValue || tipoProceso.Value < 1 || tipoProceso.Value > 3)
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "TIPO_PROCESO inválido.",
                errores = new[] { "TIPO_PROCESO debe ser 1 (producción), 2 (homologación) o 3 (beta)." }
            });
        }

        try
        {
            var rutaPfxNormalizada = ResolverRutaPfx(request.RUTA_PFX ?? string.Empty);
            var resumen = MapearResumenLegacy(request, tipoProceso.Value, rutaPfxNormalizada);
            var respuestaLegacy = _cpeGateway.EnvioResumen(resumen);
            var flgRta = ObtenerValorLegacy(respuestaLegacy, "flg_rta", "0");
            var codSunat = ObtenerValorLegacy(respuestaLegacy, "cod_sunat");
            var envioOk = string.Equals(flgRta, "1", StringComparison.Ordinal) && string.IsNullOrWhiteSpace(codSunat);

            object? registroBd;
            if (envioOk)
            {
                registroBd = await RegistrarResumenEnBaseDatosAsync(request, respuestaLegacy, cancellationToken);
            }
            else
            {
                registroBd = await RegistrarResultadoNoAceptadoResumenAsync(request, respuestaLegacy, cancellationToken);
            }

            return Ok(NormalizarRespuestaResumen(respuestaLegacy, registroBd: registroBd));
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, NormalizarRespuestaResumen(
                null,
                $"Error al enviar resumen: {ex.Message}"));
        }
    }

    [AllowAnonymous]
    [HttpPost("resumen/consultar", Name = "ConsultarResumenTicket")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> ConsultarResumenTicket([FromBody] ConsultarResumenTicketRequest? request, CancellationToken cancellationToken)
    {
        return await ConsultarResumenTicketCore(request, cancellationToken);
    }

    [AllowAnonymous]
    [HttpPost("resumen/consultar-baja", Name = "ConsultarResumenBajaTicket")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> ConsultarResumenBajaTicket([FromBody] ConsultarResumenTicketRequest? request, CancellationToken cancellationToken)
    {
        return await ConsultarResumenTicketCore(request, cancellationToken, "RC");
    }

    private async Task<IActionResult> ConsultarResumenTicketCore(
        ConsultarResumenTicketRequest? request,
        CancellationToken cancellationToken,
        string? tipoDocumentoForzado = null)
    {
        if (request is null)
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "Payload requerido."
            });
        }

        if (!request.RESUMEN_ID.HasValue || request.RESUMEN_ID.Value <= 0)
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "RESUMEN_ID es requerido y debe ser mayor a 0."
            });
        }

        var resumenIdLong = request.RESUMEN_ID.Value;
        var resumenId = resumenIdLong.ToString(CultureInfo.InvariantCulture);
        var ticket = (request.TICKET ?? string.Empty).Trim();
        var codigoSunatActual = (request.CODIGO_SUNAT ?? string.Empty).Trim();
        var mensajeSunatActual = (request.MENSAJE_SUNAT ?? string.Empty).Trim();
        var estado = (request.ESTADO ?? string.Empty).Trim();
        var intentos = request.INTENTOS ?? 0;

        if (!EsTicketNumerico(ticket) && string.IsNullOrWhiteSpace(mensajeSunatActual))
        {
            if (string.Equals(estado, "B", StringComparison.OrdinalIgnoreCase))
            {
                return Ok(new
                {
                    ok = true,
                    accion = "retornar_pendientes",
                    mensaje = MensajeTicketNoGenerado,
                    requiere_reenvio = true,
                    cdr_base64 = string.Empty
                });
            }

            var retornoTicket = await _mediator.RetornaBoletaPorTicketAsync(resumenId, cancellationToken);
            if (string.IsNullOrWhiteSpace(retornoTicket))
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new
                {
                    ok = false,
                    accion = "retornar_por_ticket_error",
                    mensaje = "No se pudo actualizar el resumen con uspRetornaBoletaPorTicket."
                });
            }

            return Ok(new
            {
                ok = true,
                accion = "retornar_por_ticket",
                mensaje = MensajeTicketNoGenerado,
                requiere_reenvio = true,
                mensaje_sunat = "NO SE GENERO EL TICKET DE RESPUESTA DE SUNAT",
                cdr_base64 = string.Empty
            });
        }

        if (EsCodigoSunatConErrorSoap(codigoSunatActual) || string.Equals(codigoSunatActual, "0109", StringComparison.OrdinalIgnoreCase))
        {
            codigoSunatActual = string.Empty;
            mensajeSunatActual = string.Empty;
        }

        if (!string.IsNullOrWhiteSpace(codigoSunatActual))
        {
            var cdrBase64Guardado = await _mediator.ObtenerCdrBase64ResumenAsync(resumenIdLong, cancellationToken);
            return Ok(new
            {
                ok = true,
                accion = "ya_consultado",
                mensaje = "El numero de ticket que selecciono ya fue consultado correctamente",
                cod_sunat = codigoSunatActual,
                msj_sunat = mensajeSunatActual,
                cdr_recibido = !string.IsNullOrWhiteSpace(cdrBase64Guardado),
                cdr_base64 = cdrBase64Guardado ?? string.Empty
            });
        }

        if (!EsTicketNumerico(ticket))
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "TICKET inválido. Debe ser numérico."
            });
        }

        if (string.IsNullOrWhiteSpace(request.RUC) ||
            string.IsNullOrWhiteSpace(request.USUARIO_SOL_EMPRESA) ||
            string.IsNullOrWhiteSpace(request.PASS_SOL_EMPRESA) ||
            string.IsNullOrWhiteSpace(request.SECUENCIA))
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "RUC, USUARIO_SOL_EMPRESA, PASS_SOL_EMPRESA y SECUENCIA son requeridos para consultar."
            });
        }

        var tipoProceso = ParseTipoProceso(request.TIPO_PROCESO) ?? 3;
        if (tipoProceso < 1 || tipoProceso > 3)
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "TIPO_PROCESO inválido. Debe ser 1, 2 o 3."
            });
        }

        Dictionary<string, string> respuestaSunat;
        try
        {
            var tipoDocumentoConsulta = string.IsNullOrWhiteSpace(tipoDocumentoForzado)
                ? (string.IsNullOrWhiteSpace(request.TIPO_DOCUMENTO) ? "RC" : request.TIPO_DOCUMENTO.Trim())
                : tipoDocumentoForzado.Trim();

            var consultaTicket = new CONSULTA_TICKET
            {
                TIPO_PROCESO = tipoProceso,
                NRO_DOCUMENTO_EMPRESA = request.RUC.Trim(),
                USUARIO_SOL_EMPRESA = request.USUARIO_SOL_EMPRESA.Trim(),
                PASS_SOL_EMPRESA = request.PASS_SOL_EMPRESA.Trim(),
                TICKET = ticket,
                TIPO_DOCUMENTO = tipoDocumentoConsulta,
                NRO_DOCUMENTO = request.SECUENCIA.Trim()
            };

            respuestaSunat = _cpeGateway.ConsultaTicket(consultaTicket);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new
            {
                ok = false,
                accion = "consulta_sunat_error",
                mensaje = $"Error al consultar SUNAT: {ex.Message}",
                cdr_base64 = string.Empty
            });
        }

        var codSunat = ObtenerValorLegacy(respuestaSunat, "cod_sunat");
        var msjSunat = ObtenerValorLegacy(respuestaSunat, "msj_sunat");
        var hashCpe = ObtenerValorLegacy(respuestaSunat, "hash_cpe");
        var hashCdr = ObtenerValorLegacy(respuestaSunat, "hash_cdr");
        var cdrBase64 = ObtenerValorLegacy(respuestaSunat, "cdr_base64");

        var codSunatDb = SanitizarCampoListaOrden(codSunat);
        var msjSunatDb = SanitizarCampoListaOrden(msjSunat);
        var hashCdrDb = SanitizarCampoListaOrden(hashCdr);
        var cdrBase64Db = LimpiarBase64(cdrBase64);

        var dataEdicion = $"{resumenId}|{codSunatDb}|{msjSunatDb}|{hashCdrDb}|{cdrBase64Db}";
        var actualizacionResumen = await _mediator.EditarResumenBoletasAsync(dataEdicion, cancellationToken);
        if (string.IsNullOrWhiteSpace(actualizacionResumen))
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new
            {
                ok = false,
                accion = "actualizar_resumen_error",
                mensaje = "No se pudo actualizar el resumen con uspEditarRB.",
                cod_sunat = codSunat,
                msj_sunat = msjSunat,
                hash_cdr = hashCdr,
                cdr_recibido = !string.IsNullOrWhiteSpace(cdrBase64Db),
                cdr_base64 = cdrBase64Db
            });
        }

        var documentosActualizados = await _mediator.ActualizarRespuestaSunatDocumentoVentaPorResumenAsync(
            resumenIdLong,
            codSunatDb,
            msjSunatDb,
            hashCdrDb,
            cancellationToken);

        if (documentosActualizados <= 0)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new
            {
                ok = false,
                accion = "actualizar_documentoventa_error",
                mensaje = "No se pudo actualizar CodigoSunat/MensajeSunat en DocumentoVenta para el lote consultado.",
                cod_sunat = codSunat,
                msj_sunat = msjSunat,
                hash_cdr = hashCdr,
                cdr_recibido = !string.IsNullOrWhiteSpace(cdrBase64Db),
                cdr_base64 = cdrBase64Db
            });
        }

        var cdrBase64GuardadoBd = await _mediator.ObtenerCdrBase64ResumenAsync(resumenIdLong, cancellationToken);
        var cdrBase64Respuesta = string.IsNullOrWhiteSpace(cdrBase64GuardadoBd) ? cdrBase64Db : cdrBase64GuardadoBd;
        var cdrRecibido = !string.IsNullOrWhiteSpace(cdrBase64Respuesta);

        if (CodigosSunatRetornoBoletas.Contains(codSunat))
        {
            var retornoBoletas = await _mediator.RetornarBoletasAsync(resumenId, cancellationToken);
            if (string.IsNullOrWhiteSpace(retornoBoletas))
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new
                {
                    ok = false,
                    accion = "retornar_boletas_error",
                    mensaje = "No se pudo ejecutar uspRetornarBoletas.",
                    cod_sunat = codSunat,
                    msj_sunat = msjSunat
                });
            }

            return Ok(new
            {
                ok = true,
                accion = "retornar_boletas",
                mensaje = string.IsNullOrWhiteSpace(msjSunat) ? "Se retornaron boletas a pendiente." : msjSunat,
                cod_sunat = codSunat,
                msj_sunat = msjSunat,
                hash_cdr = hashCdr,
                hash_cpe = hashCpe,
                documentos_actualizados = documentosActualizados,
                cdr_recibido = cdrRecibido,
                cdr_base64 = cdrBase64Respuesta,
                requiere_reenvio = true
            });
        }

        if (EsCodigoSunatConErrorSoap(codSunat) || string.IsNullOrWhiteSpace(codSunat))
        {
            intentos++;
            return Ok(new
            {
                ok = true,
                accion = intentos <= 2 ? "reintentar" : "consulta_manual",
                mensaje = intentos <= 2
                    ? $"Intente Nuevamente {intentos} de 3"
                    : "Se excedieron los intentos automáticos de consulta.",
                intentos,
                cod_sunat = codSunat,
                msj_sunat = msjSunat,
                hash_cdr = hashCdr,
                hash_cpe = hashCpe,
                documentos_actualizados = documentosActualizados,
                cdr_recibido = cdrRecibido,
                cdr_base64 = cdrBase64Respuesta
            });
        }

        return Ok(new
        {
            ok = true,
            accion = "consultado_correctamente",
            mensaje = "Se consulto Correctamente",
            cod_sunat = codSunat,
            msj_sunat = msjSunat,
            hash_cdr = hashCdr,
            hash_cpe = hashCpe,
            documentos_actualizados = documentosActualizados,
            cdr_recibido = cdrRecibido,
            cdr_base64 = cdrBase64Respuesta
        });
    }

    [AllowAnonymous]
    [HttpPost("factura/enviar", Name = "EnviarFacturaElectronica")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> EnviarFacturaElectronica([FromBody] EnviarFacturaRequest? request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "Payload requerido.",
                errores = new[] { "El body del request es obligatorio." }
            });
        }

        var requestFactura = NormalizarRequestFactura(request);
        var errores = ValidarRequestFactura(requestFactura);
        if (errores.Count > 0)
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "Existen campos obligatorios faltantes o inválidos para enviar la factura.",
                errores
            });
        }

        var tipoProceso = ParseTipoProceso(requestFactura.TIPO_PROCESO);
        if (!tipoProceso.HasValue || tipoProceso.Value < 1 || tipoProceso.Value > 3)
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "TIPO_PROCESO inválido.",
                errores = new[] { "TIPO_PROCESO debe ser 1 (producción), 2 (homologación) o 3 (beta)." }
            });
        }

        try
        {
            return Ok(await EjecutarEnvioFacturaAsync(requestFactura, tipoProceso.Value, cancellationToken));
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, NormalizarRespuestaFactura(
                null,
                $"Error al enviar factura: {ex.Message}"));
        }
    }

    [AllowAnonymous]
    [HttpPost("boleta/enviar", Name = "EnviarBoletaElectronica")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> EnviarBoletaElectronica([FromBody] EnviarFacturaRequest? request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "Payload requerido.",
                errores = new[] { "El body del request es obligatorio." }
            });
        }

        var requestBoleta = NormalizarRequestBoleta(request);
        var errores = ValidarRequestBoleta(requestBoleta);
        if (errores.Count > 0)
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "Existen campos obligatorios faltantes o inválidos para enviar la boleta.",
                errores
            });
        }

        var tipoProceso = ParseTipoProceso(requestBoleta.TIPO_PROCESO);
        if (!tipoProceso.HasValue || tipoProceso.Value < 1 || tipoProceso.Value > 3)
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "TIPO_PROCESO inválido.",
                errores = new[] { "TIPO_PROCESO debe ser 1 (producción), 2 (homologación) o 3 (beta)." }
            });
        }

        try
        {
            return Ok(await EjecutarEnvioBoletaAsync(requestBoleta, tipoProceso.Value, cancellationToken));
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, NormalizarRespuestaFactura(
                null,
                $"Error al enviar boleta: {ex.Message}"));
        }
    }

    [AllowAnonymous]
    [HttpPost("credito/enviar", Name = "EnviarNotaCreditoElectronica")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> EnviarNotaCreditoElectronica([FromBody] EnviarFacturaRequest? request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "Payload requerido.",
                errores = new[] { "El body del request es obligatorio." }
            });
        }

        var requestNotaCredito = NormalizarRequestNotaCredito(request);
        var itemsSinCodigoSunat = AplicarCodigoSunatFallback(requestNotaCredito, CodigoSunatNotaCreditoFallback);
        if (itemsSinCodigoSunat.Count > 0)
        {
            _logger.LogWarning(
                "Falta Codigo SUNAT en {Cantidad} item(s) de nota de crédito: {Items}. Se usará {CodigoFallback} temporalmente.",
                itemsSinCodigoSunat.Count,
                string.Join(", ", itemsSinCodigoSunat),
                CodigoSunatNotaCreditoFallback);
        }

        var errores = ValidarRequestNotaCredito(requestNotaCredito);
        if (errores.Count > 0)
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "Existen campos obligatorios faltantes o inválidos para enviar la nota de crédito.",
                errores
            });
        }

        var tipoProceso = ParseTipoProceso(requestNotaCredito.TIPO_PROCESO);
        if (!tipoProceso.HasValue || tipoProceso.Value < 1 || tipoProceso.Value > 3)
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "TIPO_PROCESO inválido.",
                errores = new[] { "TIPO_PROCESO debe ser 1 (producción), 2 (homologación) o 3 (beta)." }
            });
        }

        try
        {
            return Ok(await EjecutarEnvioNotaCreditoAsync(requestNotaCredito, tipoProceso.Value, cancellationToken));
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, NormalizarRespuestaFactura(
                null,
                $"Error al enviar nota de crédito: {ex.Message}"));
        }
    }

    [AllowAnonymous]
    [HttpGet("credenciales-sunat/{companiaId:int}", Name = "GetCredencialesSunat")]
    [ProducesResponseType(typeof(CredencialesSunat), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ObtenerCredencialesSunat(int companiaId, CancellationToken cancellationToken)
    {
        if (companiaId <= 0)
        {
            return BadRequest("CompaniaId debe ser mayor a 0.");
        }

        var resultado = await _mediator.ObtenerCredencialesSunatAsync(companiaId, cancellationToken);
        if (resultado is null)
        {
            return NotFound();
        }

        return Ok(resultado);
    }

    [Authorize]
    [RequestSizeLimit(MaxCertificateSizeBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxCertificateSizeBytes)]
    [HttpPost("credenciales-sunat", Name = "GuardarCredencialesSunat")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> GuardarCredencialesSunat([FromForm] GuardarCredencialesSunatRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest("Request requerido.");
        }

        if (request.CompaniaId <= 0)
        {
            return BadRequest("CompaniaId debe ser mayor a 0.");
        }

        if (string.IsNullOrWhiteSpace(request.UsuarioSOL))
        {
            return BadRequest("UsuarioSOL es requerido.");
        }

        if (string.IsNullOrWhiteSpace(request.ClaveSOL))
        {
            return BadRequest("ClaveSOL es requerido.");
        }

        if (string.IsNullOrWhiteSpace(request.ClaveCertificado))
        {
            return BadRequest("ClaveCertificado es requerido.");
        }

        if (request.Entorno <= 0)
        {
            return BadRequest("Entorno debe ser mayor a 0.");
        }

        if (request.Certificado is null || request.Certificado.Length == 0)
        {
            return BadRequest("Debe enviar el archivo del certificado.");
        }

        if (request.Certificado.Length > MaxCertificateSizeBytes)
        {
            return BadRequest($"El certificado excede el límite de {MaxCertificateSizeBytes / (1024 * 1024)} MB.");
        }

        var extension = Path.GetExtension(request.Certificado.FileName);
        if (!AllowedCertificateExtensions.Contains(extension))
        {
            return BadRequest("Solo se permiten archivos .p12 o .pfx.");
        }

        await using var stream = request.Certificado.OpenReadStream();
        await using var memory = new MemoryStream();
        await stream.CopyToAsync(memory, cancellationToken);
        var certificadoBytes = memory.ToArray();
        if (certificadoBytes.Length == 0)
        {
            return BadRequest("El certificado no contiene datos.");
        }

        var certificadoBase64 = Convert.ToBase64String(certificadoBytes);

        var ok = await _mediator.GuardarCredencialesSunatAsync(
            request.CompaniaId,
            request.UsuarioSOL.Trim(),
            request.ClaveSOL.Trim(),
            certificadoBase64,
            request.ClaveCertificado.Trim(),
            request.Entorno,
            cancellationToken);

        return Ok(new { ok });
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
            AplicarReglaTributariaDocumento(request.Nota, detalles);
            var clienteDocumento = await ObtenerDatosClienteDocumentoConFallbackAsync(request.Nota.ClienteId, cancellationToken);
            var vdataNota = BuildOrdenPayload(request.Nota, detalles, clienteDocumento);
            var resultado = await _mediator.RegistrarOrdenAsync(vdataNota, cancellationToken);
            if (EsFactura(request.Nota.NotaDocu))
            {
                var respuestaSunat = await IntentarEmitirFacturaDesdeOrdenAsync(request.Nota, detalles, resultado, cancellationToken);
                return Ok(new
                {
                    resultado,
                    cod_sunat = ObtenerValorNormalizadoRespuesta(respuestaSunat, "cod_sunat"),
                    msj_sunat = ObtenerValorNormalizadoRespuesta(respuestaSunat, "msj_sunat"),
                    aceptado = ObtenerValorNormalizadoRespuesta(respuestaSunat, "aceptado"),
                    hash_cdr = ObtenerValorNormalizadoRespuesta(respuestaSunat, "hash_cdr"),
                    cdr_recibido = ObtenerValorNormalizadoRespuesta(respuestaSunat, "cdr_recibido"),
                    cdr_base64 = ObtenerValorNormalizadoRespuesta(respuestaSunat, "cdr_base64"),
                    sunat = respuestaSunat
                });
            }
            if (EsBoleta(request.Nota.NotaDocu))
            {
                var respuestaSunat = await IntentarEmitirBoletaDesdeOrdenAsync(request.Nota, detalles, resultado, cancellationToken);
                return Ok(new
                {
                    resultado,
                    cod_sunat = ObtenerValorNormalizadoRespuesta(respuestaSunat, "cod_sunat"),
                    msj_sunat = ObtenerValorNormalizadoRespuesta(respuestaSunat, "msj_sunat"),
                    aceptado = ObtenerValorNormalizadoRespuesta(respuestaSunat, "aceptado"),
                    hash_cdr = ObtenerValorNormalizadoRespuesta(respuestaSunat, "hash_cdr"),
                    cdr_recibido = ObtenerValorNormalizadoRespuesta(respuestaSunat, "cdr_recibido"),
                    cdr_base64 = ObtenerValorNormalizadoRespuesta(respuestaSunat, "cdr_base64"),
                    sunat = respuestaSunat
                });
            }

            return Ok(resultado);
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
            AplicarReglaTributariaDocumento(request.Nota, detalles);
            var clienteDocumento = await ObtenerDatosClienteDocumentoConFallbackAsync(request.Nota.ClienteId, cancellationToken);
            var vdataNota = BuildOrdenPayload(request.Nota, detalles, clienteDocumento);
            var resultado = await _mediator.RegistrarOrdenAsync(vdataNota, cancellationToken);
            if (EsFactura(request.Nota.NotaDocu))
            {
                var respuestaSunat = await IntentarEmitirFacturaDesdeOrdenAsync(request.Nota, detalles, resultado, cancellationToken);
                return Ok(new
                {
                    resultado,
                    cod_sunat = ObtenerValorNormalizadoRespuesta(respuestaSunat, "cod_sunat"),
                    msj_sunat = ObtenerValorNormalizadoRespuesta(respuestaSunat, "msj_sunat"),
                    aceptado = ObtenerValorNormalizadoRespuesta(respuestaSunat, "aceptado"),
                    hash_cdr = ObtenerValorNormalizadoRespuesta(respuestaSunat, "hash_cdr"),
                    cdr_recibido = ObtenerValorNormalizadoRespuesta(respuestaSunat, "cdr_recibido"),
                    cdr_base64 = ObtenerValorNormalizadoRespuesta(respuestaSunat, "cdr_base64"),
                    sunat = respuestaSunat
                });
            }
            if (EsBoleta(request.Nota.NotaDocu))
            {
                var respuestaSunat = await IntentarEmitirBoletaDesdeOrdenAsync(request.Nota, detalles, resultado, cancellationToken);
                return Ok(new
                {
                    resultado,
                    cod_sunat = ObtenerValorNormalizadoRespuesta(respuestaSunat, "cod_sunat"),
                    msj_sunat = ObtenerValorNormalizadoRespuesta(respuestaSunat, "msj_sunat"),
                    aceptado = ObtenerValorNormalizadoRespuesta(respuestaSunat, "aceptado"),
                    hash_cdr = ObtenerValorNormalizadoRespuesta(respuestaSunat, "hash_cdr"),
                    cdr_recibido = ObtenerValorNormalizadoRespuesta(respuestaSunat, "cdr_recibido"),
                    cdr_base64 = ObtenerValorNormalizadoRespuesta(respuestaSunat, "cdr_base64"),
                    sunat = respuestaSunat
                });
            }

            return Ok(resultado);
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
            AplicarReglaTributariaDocumento(request.Nota, detalles);
            var vdataNota = BuildEditarPayload(request.Nota, detalles);
            var resultado = await _mediator.EditarOrdenAsync(vdataNota, cancellationToken);
            if (EsResultadoEdicionExitosa(resultado))
            {
                await ActualizarTributacionPostEdicionAsync(request.Nota, detalles, cancellationToken);
            }
            return Ok(resultado);
        }

        var vdata = BuildEditarPayload(body);
        return Ok(await _mediator.EditarOrdenAsync(vdata, cancellationToken));
    }

    private string BuildOrdenPayload(NotaPedido nota, IEnumerable<DetalleNota> detalles, ClienteDocumentoInfo? clienteDocumento = null)
    {
        var detalleList = detalles == null ? new List<DetalleNota>() : new List<DetalleNota>(detalles);
        var xDocumento = string.IsNullOrWhiteSpace(nota.NotaDocu) ? "BOLETA" : nota.NotaDocu!;
        var icbper = nota.ICBPER ?? 0m;
        var totalDetalle = detalleList.Sum(x => x.DetalleImporte ?? 0m);
        var total = nota.NotaTotal ?? (totalDetalle + icbper);
        var calculoTributario = CalcularTributacionDocumento(xDocumento, total, icbper);
        var subtotal = calculoTributario.SubTotal;
        var igv = calculoTributario.Igv;

        var movilidad = nota.NotaMovilidad ?? 0m;
        var descuento = nota.NotaDescuento ?? 0m;
        var acuenta = nota.NotaAcuenta ?? 0m;
        var saldo = nota.NotaSaldo ?? 0m;
        var adicional = nota.NotaAdicional ?? 0m;
        var tarjeta = nota.NotaTarjeta ?? 0m;
        var pagar = nota.NotaPagar ?? total;
        var ganancia = nota.NotaGanancia ?? 0m;
        var xserie = nota.NotaSerie ?? string.Empty;
        var numero = nota.NotaNumero ?? string.Empty;
        // Defaults for uspinsertarNotaB fields not present in NotaPedido payloads
        var docuAdicional = 0m;
        var docuHash = string.Empty;
        var estadoSunat = "PENDIENTE";
        var docuSubtotal = calculoTributario.SubTotal;
        var docuIgv = calculoTributario.Igv;
        var usuarioId = "7";
        var docuGravada = calculoTributario.Gravada;
        var docuDescuento = 0m;
        var notaIdbr = nota.NotaId > 0 ? nota.NotaId.ToString() : "0";
        var entidadBancaria = nota.EntidadBancaria ?? "-";
        var nroOperacion = nota.NroOperacion ?? string.Empty;
        var efectivo = nota.Efectivo ?? pagar;
        var deposito = nota.Deposito ?? 0m;
        var esBoleta = string.Equals(xDocumento, "BOLETA", StringComparison.OrdinalIgnoreCase);
        var clienteRazon = clienteDocumento?.ClienteRazon ?? string.Empty;
        var clienteRuc = clienteDocumento?.ClienteRuc ?? string.Empty;
        var clienteDni = clienteDocumento?.ClienteDni ?? string.Empty;
        var direccionFiscal = !string.IsNullOrWhiteSpace(clienteDocumento?.DireccionFiscal)
            ? clienteDocumento!.DireccionFiscal
            : (nota.NotaDireccion ?? string.Empty);

        if (string.IsNullOrWhiteSpace(clienteRazon))
        {
            clienteRazon = "VARIOS";
        }

        if (esBoleta && string.IsNullOrWhiteSpace(clienteRuc) && string.IsNullOrWhiteSpace(clienteDni))
        {
            clienteDni = "00000000";
        }

        if (string.IsNullOrWhiteSpace(direccionFiscal))
        {
            direccionFiscal = "-";
        }

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

    private async Task<ClienteDocumentoInfo?> ObtenerDatosClienteDocumentoAsync(long? clienteId, CancellationToken cancellationToken)
    {
        if (!clienteId.HasValue || clienteId.Value <= 0)
        {
            return null;
        }

        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return null;
        }

        const string sql = """
            SELECT TOP (1)
                ClienteRazon,
                ClienteRuc,
                ClienteDni,
                ClienteDireccion
            FROM Cliente
            WHERE ClienteId = @ClienteId;
            """;

        await using var con = new SqlConnection(connectionString);
        await using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@ClienteId", clienteId.Value);

        await con.OpenAsync(cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return new ClienteDocumentoInfo
        {
            ClienteRazon = reader["ClienteRazon"]?.ToString()?.Trim() ?? string.Empty,
            ClienteRuc = reader["ClienteRuc"]?.ToString()?.Trim() ?? string.Empty,
            ClienteDni = reader["ClienteDni"]?.ToString()?.Trim() ?? string.Empty,
            DireccionFiscal = reader["ClienteDireccion"]?.ToString()?.Trim() ?? string.Empty
        };
    }

    private async Task<ClienteDocumentoInfo?> ObtenerDatosClienteDocumentoConFallbackAsync(long? clienteId, CancellationToken cancellationToken)
    {
        var cliente = await ObtenerDatosClienteDocumentoAsync(clienteId, cancellationToken);
        var necesitaFallback = cliente is null ||
                               string.IsNullOrWhiteSpace(cliente.ClienteRazon) ||
                               string.IsNullOrWhiteSpace(cliente.DireccionFiscal) ||
                               (string.IsNullOrWhiteSpace(cliente.ClienteRuc) && string.IsNullOrWhiteSpace(cliente.ClienteDni));

        if (!necesitaFallback)
        {
            return cliente;
        }

        var clienteGenerico = await ObtenerDatosClienteDocumentoAsync(1, cancellationToken);
        if (clienteGenerico is null)
        {
            return cliente;
        }

        return new ClienteDocumentoInfo
        {
            ClienteRazon = !string.IsNullOrWhiteSpace(cliente?.ClienteRazon) ? cliente.ClienteRazon : clienteGenerico.ClienteRazon,
            ClienteRuc = !string.IsNullOrWhiteSpace(cliente?.ClienteRuc) ? cliente.ClienteRuc : clienteGenerico.ClienteRuc,
            ClienteDni = !string.IsNullOrWhiteSpace(cliente?.ClienteDni) ? cliente.ClienteDni : clienteGenerico.ClienteDni,
            DireccionFiscal = !string.IsNullOrWhiteSpace(cliente?.DireccionFiscal) ? cliente.DireccionFiscal : clienteGenerico.DireccionFiscal
        };
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
        var icbper = GetFirstDecimal(res, 0m, "ICBPER");
        var subtotalInformativo = GetFirstDecimal(res, 0m, "SubTotal", "NotaSubtotal", "DocuSubtotal");
        var movilidad = GetFirstDecimal(res, 0m, "Movilidad", "NotaMovilidad");
        var descuento = GetFirstDecimal(res, 0m, "Descuento", "NotaDescuento", "DocuDescuento");
        var total = GetFirstDecimal(res, 0m, "Total", "NotaTotal");
        if (total <= 0m)
        {
            total = SumarImporteDetalleJson(productoArray, producto) + icbper;
            if (total <= 0m && subtotalInformativo > 0m)
            {
                total = subtotalInformativo + icbper;
            }
        }
        var calculoTributario = CalcularTributacionDocumento(docu, total, icbper);
        var subtotal = calculoTributario.SubTotal;
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
        var docuSubtotal = calculoTributario.SubTotal;
        var igv = calculoTributario.Igv;
        var usuarioId = GetFirstString(res, "UsuarioId");
        var docuGravada = calculoTributario.Gravada;
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
        return FormatDecimalSinRedondeo(value);
    }

    private static string Format4(decimal value)
    {
        return FormatDecimalSinRedondeo(value);
    }

    private static string FormatDecimalSinRedondeo(decimal value)
    {
        return value.ToString("0.#############################", CultureInfo.InvariantCulture);
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

    private static bool EsTicketNumerico(string ticket)
    {
        if (string.IsNullOrWhiteSpace(ticket))
        {
            return false;
        }

        foreach (var ch in ticket.Trim())
        {
            if (!char.IsDigit(ch))
            {
                return false;
            }
        }

        return true;
    }

    private static bool EsCodigoSunatConErrorSoap(string? codigoSunat)
    {
        if (string.IsNullOrWhiteSpace(codigoSunat))
        {
            return false;
        }

        return codigoSunat.Contains("env:Server", StringComparison.OrdinalIgnoreCase) ||
               codigoSunat.Contains("env:Client", StringComparison.OrdinalIgnoreCase);
    }

    private static string LimpiarBase64(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return string.Empty;
        }

        return valor
            .Trim()
            .Replace("\r", string.Empty, StringComparison.Ordinal)
            .Replace("\n", string.Empty, StringComparison.Ordinal);
    }

    private static List<string> ValidarRequestResumen(EnviarResumenBoletasRequest request)
    {
        var errores = new List<string>();
        AgregarErrorSiVacio(errores, request.NRO_DOCUMENTO_EMPRESA, "NRO_DOCUMENTO_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.RAZON_SOCIAL, "RAZON_SOCIAL es requerido.");
        AgregarErrorSiVacio(errores, request.TIPO_DOCUMENTO, "TIPO_DOCUMENTO es requerido.");
        AgregarErrorSiVacio(errores, request.CODIGO, "CODIGO es requerido.");
        AgregarErrorSiVacio(errores, request.SERIE, "SERIE es requerido.");
        AgregarErrorSiVacio(errores, request.SECUENCIA, "SECUENCIA es requerido.");
        AgregarErrorSiVacio(errores, request.FECHA_REFERENCIA, "FECHA_REFERENCIA es requerido.");
        AgregarErrorSiVacio(errores, request.FECHA_DOCUMENTO, "FECHA_DOCUMENTO es requerido.");
        AgregarErrorSiVacio(errores, request.CONTRA_FIRMA, "CONTRA_FIRMA es requerido.");
        AgregarErrorSiVacio(errores, request.USUARIO_SOL_EMPRESA, "USUARIO_SOL_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.PASS_SOL_EMPRESA, "PASS_SOL_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.RUTA_PFX, "RUTA_PFX es requerido.");

        if (!EsFechaIsoValida(request.FECHA_REFERENCIA))
        {
            errores.Add("FECHA_REFERENCIA debe tener formato yyyy-MM-dd.");
        }

        if (!EsFechaIsoValida(request.FECHA_DOCUMENTO))
        {
            errores.Add("FECHA_DOCUMENTO debe tener formato yyyy-MM-dd.");
        }

        if (request.detalle is null || request.detalle.Count == 0)
        {
            errores.Add("detalle es requerido y debe contener al menos un elemento.");
            return errores;
        }

        for (int i = 0; i < request.detalle.Count; i++)
        {
            var item = request.detalle[i];
            if (item is null)
            {
                errores.Add($"detalle[{i}] no puede ser null.");
                continue;
            }

            if (!item.item.HasValue || item.item <= 0)
            {
                errores.Add($"detalle[{i}].item es requerido y debe ser mayor a 0.");
            }

            AgregarErrorSiVacio(errores, item.tipoComprobante, $"detalle[{i}].tipoComprobante es requerido.");
            AgregarErrorSiVacio(errores, item.nroComprobante, $"detalle[{i}].nroComprobante es requerido.");
            AgregarErrorSiVacio(errores, item.tipoDocumento, $"detalle[{i}].tipoDocumento es requerido.");
            AgregarErrorSiVacio(errores, item.nroDocumento, $"detalle[{i}].nroDocumento es requerido.");
            AgregarErrorSiVacio(errores, item.statu, $"detalle[{i}].statu es requerido.");
            AgregarErrorSiVacio(errores, item.codMoneda, $"detalle[{i}].codMoneda es requerido.");
        }

        return errores;
    }

    private static List<string> ValidarRequestBaja(EnviarResumenBoletasRequest request)
    {
        var errores = new List<string>();
        AgregarErrorSiVacio(errores, request.NRO_DOCUMENTO_EMPRESA, "NRO_DOCUMENTO_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.RAZON_SOCIAL, "RAZON_SOCIAL es requerido.");
        AgregarErrorSiVacio(errores, request.TIPO_DOCUMENTO, "TIPO_DOCUMENTO es requerido.");
        AgregarErrorSiVacio(errores, request.SECUENCIA, "SECUENCIA es requerido.");
        AgregarErrorSiVacio(errores, request.FECHA_REFERENCIA, "FECHA_REFERENCIA es requerido.");
        AgregarErrorSiVacio(errores, request.FECHA_DOCUMENTO, "FECHA_DOCUMENTO es requerido.");
        AgregarErrorSiVacio(errores, request.CONTRA_FIRMA, "CONTRA_FIRMA es requerido.");
        AgregarErrorSiVacio(errores, request.USUARIO_SOL_EMPRESA, "USUARIO_SOL_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.PASS_SOL_EMPRESA, "PASS_SOL_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.RUTA_PFX, "RUTA_PFX es requerido.");

        if (!EsFechaIsoValida(request.FECHA_REFERENCIA))
        {
            errores.Add("FECHA_REFERENCIA debe tener formato yyyy-MM-dd.");
        }

        if (!EsFechaIsoValida(request.FECHA_DOCUMENTO))
        {
            errores.Add("FECHA_DOCUMENTO debe tener formato yyyy-MM-dd.");
        }

        if (request.detalle is null || request.detalle.Count == 0)
        {
            errores.Add("detalle es requerido y debe contener al menos un elemento.");
            return errores;
        }

        for (int i = 0; i < request.detalle.Count; i++)
        {
            var item = request.detalle[i];
            if (item is null)
            {
                errores.Add($"detalle[{i}] no puede ser null.");
                continue;
            }

            if (!item.item.HasValue || item.item <= 0)
            {
                errores.Add($"detalle[{i}].item es requerido y debe ser mayor a 0.");
            }

            AgregarErrorSiVacio(errores, item.tipoComprobante, $"detalle[{i}].tipoComprobante es requerido.");

            var (serie, numero) = ResolverSerieNumeroBaja(item);
            if (string.IsNullOrWhiteSpace(serie) || string.IsNullOrWhiteSpace(numero))
            {
                errores.Add($"detalle[{i}] debe incluir nroComprobante en formato SERIE-NUMERO o bien serie y numero.");
            }
        }

        return errores;
    }

    private static CPE_RESUMEN_BOLETA MapearResumenLegacy(EnviarResumenBoletasRequest request, int tipoProceso, string rutaPfxNormalizada)
    {
        var resumen = new CPE_RESUMEN_BOLETA
        {
            NRO_DOCUMENTO_EMPRESA = request.NRO_DOCUMENTO_EMPRESA?.Trim(),
            RAZON_SOCIAL = request.RAZON_SOCIAL?.Trim(),
            TIPO_DOCUMENTO = request.TIPO_DOCUMENTO?.Trim(),
            CODIGO = request.CODIGO?.Trim(),
            SERIE = request.SERIE?.Trim(),
            SECUENCIA = request.SECUENCIA?.Trim(),
            FECHA_REFERENCIA = request.FECHA_REFERENCIA?.Trim(),
            FECHA_DOCUMENTO = request.FECHA_DOCUMENTO?.Trim(),
            TIPO_PROCESO = tipoProceso,
            CONTRA_FIRMA = request.CONTRA_FIRMA?.Trim(),
            USUARIO_SOL_EMPRESA = request.USUARIO_SOL_EMPRESA?.Trim(),
            PASS_SOL_EMPRESA = request.PASS_SOL_EMPRESA?.Trim(),
            RUTA_PFX = rutaPfxNormalizada
        };

        foreach (var detalle in request.detalle ?? Enumerable.Empty<EnviarResumenBoletasDetalleRequest>())
        {
            resumen.detalle.Add(new CPE_RESUMEN_BOLETA_DETALLE
            {
                ITEM = detalle.item,
                TIPO_COMPROBANTE = detalle.tipoComprobante?.Trim(),
                NRO_COMPROBANTE = detalle.nroComprobante?.Trim(),
                TIPO_DOCUMENTO = detalle.tipoDocumento?.Trim(),
                NRO_DOCUMENTO = detalle.nroDocumento?.Trim(),
                TIPO_COMPROBANTE_REF = detalle.tipoComprobanteRef?.Trim(),
                NRO_COMPROBANTE_REF = detalle.nroComprobanteRef?.Trim(),
                STATU = detalle.statu?.Trim(),
                COD_MONEDA = detalle.codMoneda?.Trim(),
                TOTAL = detalle.total ?? 0m,
                ICBPER = detalle.icbper ?? 0m,
                GRAVADA = detalle.gravada ?? 0m,
                ISC = detalle.isc ?? 0m,
                IGV = detalle.igv ?? 0m,
                OTROS = detalle.otros ?? 0m,
                CARGO_X_ASIGNACION = detalle.cargoXAsignacion ?? 0,
                MONTO_CARGO_X_ASIG = detalle.montoCargoXAsig ?? 0m,
                EXONERADO = detalle.exonerado ?? 0m,
                INAFECTO = detalle.inafecto ?? 0m,
                EXPORTACION = detalle.exportacion ?? 0m,
                GRATUITAS = detalle.gratuitas ?? 0m
            });
        }

        return resumen;
    }

    private static CPE_BAJA MapearBajaLegacy(EnviarResumenBoletasRequest request, int tipoProceso, string rutaPfxNormalizada)
    {
        var baja = new CPE_BAJA
        {
            NRO_DOCUMENTO_EMPRESA = request.NRO_DOCUMENTO_EMPRESA?.Trim(),
            RAZON_SOCIAL = request.RAZON_SOCIAL?.Trim(),
            TIPO_DOCUMENTO = request.TIPO_DOCUMENTO?.Trim(),
            CODIGO = string.IsNullOrWhiteSpace(request.CODIGO) ? "RA" : request.CODIGO.Trim(),
            SERIE = ResolverSerieBaja(request),
            SECUENCIA = request.SECUENCIA?.Trim(),
            FECHA_REFERENCIA = request.FECHA_REFERENCIA?.Trim(),
            FECHA_BAJA = request.FECHA_DOCUMENTO?.Trim(),
            TIPO_PROCESO = tipoProceso,
            CONTRA_FIRMA = request.CONTRA_FIRMA?.Trim(),
            USUARIO_SOL_EMPRESA = request.USUARIO_SOL_EMPRESA?.Trim(),
            PASS_SOL_EMPRESA = request.PASS_SOL_EMPRESA?.Trim(),
            RUTA_PFX = rutaPfxNormalizada
        };

        foreach (var detalle in request.detalle ?? Enumerable.Empty<EnviarResumenBoletasDetalleRequest>())
        {
            var (serie, numero) = ResolverSerieNumeroBaja(detalle);

            baja.detalle.Add(new CPE_BAJA_DETALLE
            {
                ITEM = detalle.item,
                TIPO_COMPROBANTE = detalle.tipoComprobante?.Trim(),
                SERIE = serie,
                NUMERO = numero,
                DESCRIPCION = ResolverDescripcionBaja(detalle)
            });
        }

        return baja;
    }

    private static List<string> ValidarRequestFactura(EnviarFacturaRequest request)
    {
        var errores = new List<string>();

        if (!string.Equals((request.COD_TIPO_DOCUMENTO ?? string.Empty).Trim(), "01", StringComparison.Ordinal))
        {
            errores.Add("COD_TIPO_DOCUMENTO debe ser '01' para este endpoint.");
        }

        AgregarErrorSiVacio(errores, request.NRO_DOCUMENTO_EMPRESA, "NRO_DOCUMENTO_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.TIPO_DOCUMENTO_EMPRESA, "TIPO_DOCUMENTO_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.RAZON_SOCIAL_EMPRESA, "RAZON_SOCIAL_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.CODIGO_UBIGEO_EMPRESA, "CODIGO_UBIGEO_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.DIRECCION_EMPRESA, "DIRECCION_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.DEPARTAMENTO_EMPRESA, "DEPARTAMENTO_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.PROVINCIA_EMPRESA, "PROVINCIA_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.DISTRITO_EMPRESA, "DISTRITO_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.CODIGO_PAIS_EMPRESA, "CODIGO_PAIS_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.NRO_COMPROBANTE, "NRO_COMPROBANTE es requerido.");
        AgregarErrorSiVacio(errores, request.FECHA_DOCUMENTO, "FECHA_DOCUMENTO es requerido.");
        AgregarErrorSiVacio(errores, request.COD_MONEDA, "COD_MONEDA es requerido.");
        AgregarErrorSiVacio(errores, request.USUARIO_SOL_EMPRESA, "USUARIO_SOL_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.PASS_SOL_EMPRESA, "PASS_SOL_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.CONTRA_FIRMA, "CONTRA_FIRMA es requerido.");
        AgregarErrorSiVacio(errores, request.RUTA_PFX, "RUTA_PFX es requerido.");
        AgregarErrorSiVacio(errores, request.NRO_DOCUMENTO_CLIENTE, "NRO_DOCUMENTO_CLIENTE es requerido.");
        AgregarErrorSiVacio(errores, request.TIPO_DOCUMENTO_CLIENTE, "TIPO_DOCUMENTO_CLIENTE es requerido.");
        AgregarErrorSiVacio(errores, request.RAZON_SOCIAL_CLIENTE, "RAZON_SOCIAL_CLIENTE es requerido.");
        AgregarErrorSiVacio(errores, request.COD_UBIGEO_CLIENTE, "COD_UBIGEO_CLIENTE es requerido.");
        AgregarErrorSiVacio(errores, request.DIRECCION_CLIENTE, "DIRECCION_CLIENTE es requerido.");
        AgregarErrorSiVacio(errores, request.DEPARTAMENTO_CLIENTE, "DEPARTAMENTO_CLIENTE es requerido.");
        AgregarErrorSiVacio(errores, request.PROVINCIA_CLIENTE, "PROVINCIA_CLIENTE es requerido.");
        AgregarErrorSiVacio(errores, request.DISTRITO_CLIENTE, "DISTRITO_CLIENTE es requerido.");
        AgregarErrorSiVacio(errores, request.COD_PAIS_CLIENTE, "COD_PAIS_CLIENTE es requerido.");

        if (!EsFechaIsoValida(request.FECHA_DOCUMENTO))
        {
            errores.Add("FECHA_DOCUMENTO debe tener formato yyyy-MM-dd.");
        }

        if (!string.IsNullOrWhiteSpace(request.FECHA_VTO) && !EsFechaIsoValida(request.FECHA_VTO))
        {
            errores.Add("FECHA_VTO debe tener formato yyyy-MM-dd.");
        }

        if (request.detalle is null || request.detalle.Count == 0)
        {
            errores.Add("detalle es requerido y debe contener al menos un elemento.");
            return errores;
        }

        for (var i = 0; i < request.detalle.Count; i++)
        {
            var item = request.detalle[i];
            if (item is null)
            {
                errores.Add($"detalle[{i}] no puede ser null.");
                continue;
            }

            if (!item.item.HasValue || item.item <= 0)
            {
                errores.Add($"detalle[{i}].item es requerido y debe ser mayor a 0.");
            }

            if (!item.cantidad.HasValue || item.cantidad <= 0)
            {
                errores.Add($"detalle[{i}].cantidad es requerido y debe ser mayor a 0.");
            }

            if (!item.importe.HasValue || item.importe < 0)
            {
                errores.Add($"detalle[{i}].importe es requerido y debe ser mayor o igual a 0.");
            }

            if (!item.precio.HasValue || item.precio < 0)
            {
                errores.Add($"detalle[{i}].precio es requerido y debe ser mayor o igual a 0.");
            }

            AgregarErrorSiVacio(errores, item.descripcion, $"detalle[{i}].descripcion es requerido.");
            AgregarErrorSiVacio(errores, item.codTipoOperacion, $"detalle[{i}].codTipoOperacion es requerido.");
        }

        return errores;
    }

    private static List<string> ValidarRequestBoleta(EnviarFacturaRequest request)
    {
        var errores = new List<string>();

        if (!string.Equals((request.COD_TIPO_DOCUMENTO ?? string.Empty).Trim(), "03", StringComparison.Ordinal))
        {
            errores.Add("COD_TIPO_DOCUMENTO debe ser '03' para este endpoint.");
        }

        AgregarErrorSiVacio(errores, request.NRO_DOCUMENTO_EMPRESA, "NRO_DOCUMENTO_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.TIPO_DOCUMENTO_EMPRESA, "TIPO_DOCUMENTO_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.RAZON_SOCIAL_EMPRESA, "RAZON_SOCIAL_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.CODIGO_UBIGEO_EMPRESA, "CODIGO_UBIGEO_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.DIRECCION_EMPRESA, "DIRECCION_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.DEPARTAMENTO_EMPRESA, "DEPARTAMENTO_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.PROVINCIA_EMPRESA, "PROVINCIA_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.DISTRITO_EMPRESA, "DISTRITO_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.CODIGO_PAIS_EMPRESA, "CODIGO_PAIS_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.NRO_COMPROBANTE, "NRO_COMPROBANTE es requerido.");
        AgregarErrorSiVacio(errores, request.FECHA_DOCUMENTO, "FECHA_DOCUMENTO es requerido.");
        AgregarErrorSiVacio(errores, request.COD_MONEDA, "COD_MONEDA es requerido.");
        AgregarErrorSiVacio(errores, request.USUARIO_SOL_EMPRESA, "USUARIO_SOL_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.PASS_SOL_EMPRESA, "PASS_SOL_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.CONTRA_FIRMA, "CONTRA_FIRMA es requerido.");
        AgregarErrorSiVacio(errores, request.RUTA_PFX, "RUTA_PFX es requerido.");
        AgregarErrorSiVacio(errores, request.NRO_DOCUMENTO_CLIENTE, "NRO_DOCUMENTO_CLIENTE es requerido.");
        AgregarErrorSiVacio(errores, request.TIPO_DOCUMENTO_CLIENTE, "TIPO_DOCUMENTO_CLIENTE es requerido.");
        AgregarErrorSiVacio(errores, request.RAZON_SOCIAL_CLIENTE, "RAZON_SOCIAL_CLIENTE es requerido.");
        AgregarErrorSiVacio(errores, request.COD_UBIGEO_CLIENTE, "COD_UBIGEO_CLIENTE es requerido.");
        AgregarErrorSiVacio(errores, request.DIRECCION_CLIENTE, "DIRECCION_CLIENTE es requerido.");
        AgregarErrorSiVacio(errores, request.DEPARTAMENTO_CLIENTE, "DEPARTAMENTO_CLIENTE es requerido.");
        AgregarErrorSiVacio(errores, request.PROVINCIA_CLIENTE, "PROVINCIA_CLIENTE es requerido.");
        AgregarErrorSiVacio(errores, request.DISTRITO_CLIENTE, "DISTRITO_CLIENTE es requerido.");
        AgregarErrorSiVacio(errores, request.COD_PAIS_CLIENTE, "COD_PAIS_CLIENTE es requerido.");

        if (!EsFechaIsoValida(request.FECHA_DOCUMENTO))
        {
            errores.Add("FECHA_DOCUMENTO debe tener formato yyyy-MM-dd.");
        }

        if (!string.IsNullOrWhiteSpace(request.FECHA_VTO) && !EsFechaIsoValida(request.FECHA_VTO))
        {
            errores.Add("FECHA_VTO debe tener formato yyyy-MM-dd.");
        }

        if (request.detalle is null || request.detalle.Count == 0)
        {
            errores.Add("detalle es requerido y debe contener al menos un elemento.");
            return errores;
        }

        for (var i = 0; i < request.detalle.Count; i++)
        {
            var item = request.detalle[i];
            if (item is null)
            {
                errores.Add($"detalle[{i}] no puede ser null.");
                continue;
            }

            if (!item.item.HasValue || item.item <= 0)
            {
                errores.Add($"detalle[{i}].item es requerido y debe ser mayor a 0.");
            }

            if (!item.cantidad.HasValue || item.cantidad <= 0)
            {
                errores.Add($"detalle[{i}].cantidad es requerido y debe ser mayor a 0.");
            }

            if (!item.importe.HasValue || item.importe < 0)
            {
                errores.Add($"detalle[{i}].importe es requerido y debe ser mayor o igual a 0.");
            }

            if (!item.precio.HasValue || item.precio < 0)
            {
                errores.Add($"detalle[{i}].precio es requerido y debe ser mayor o igual a 0.");
            }

            AgregarErrorSiVacio(errores, item.descripcion, $"detalle[{i}].descripcion es requerido.");
            AgregarErrorSiVacio(errores, item.codTipoOperacion, $"detalle[{i}].codTipoOperacion es requerido.");
        }

        return errores;
    }

    private static List<string> ValidarRequestNotaCredito(EnviarFacturaRequest request)
    {
        var errores = new List<string>();

        if (!string.Equals((request.COD_TIPO_DOCUMENTO ?? string.Empty).Trim(), "07", StringComparison.Ordinal))
        {
            errores.Add("COD_TIPO_DOCUMENTO debe ser '07' para este endpoint.");
        }

        AgregarErrorSiVacio(errores, request.NRO_DOCUMENTO_EMPRESA, "NRO_DOCUMENTO_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.TIPO_DOCUMENTO_EMPRESA, "TIPO_DOCUMENTO_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.RAZON_SOCIAL_EMPRESA, "RAZON_SOCIAL_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.CODIGO_UBIGEO_EMPRESA, "CODIGO_UBIGEO_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.DIRECCION_EMPRESA, "DIRECCION_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.DEPARTAMENTO_EMPRESA, "DEPARTAMENTO_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.PROVINCIA_EMPRESA, "PROVINCIA_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.DISTRITO_EMPRESA, "DISTRITO_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.CODIGO_PAIS_EMPRESA, "CODIGO_PAIS_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.NRO_COMPROBANTE, "NRO_COMPROBANTE es requerido.");
        AgregarErrorSiVacio(errores, request.FECHA_DOCUMENTO, "FECHA_DOCUMENTO es requerido.");
        AgregarErrorSiVacio(errores, request.COD_MONEDA, "COD_MONEDA es requerido.");
        AgregarErrorSiVacio(errores, request.USUARIO_SOL_EMPRESA, "USUARIO_SOL_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.PASS_SOL_EMPRESA, "PASS_SOL_EMPRESA es requerido.");
        AgregarErrorSiVacio(errores, request.CONTRA_FIRMA, "CONTRA_FIRMA es requerido.");
        AgregarErrorSiVacio(errores, request.RUTA_PFX, "RUTA_PFX es requerido.");
        AgregarErrorSiVacio(errores, request.NRO_DOCUMENTO_CLIENTE, "NRO_DOCUMENTO_CLIENTE es requerido.");
        AgregarErrorSiVacio(errores, request.TIPO_DOCUMENTO_CLIENTE, "TIPO_DOCUMENTO_CLIENTE es requerido.");
        AgregarErrorSiVacio(errores, request.RAZON_SOCIAL_CLIENTE, "RAZON_SOCIAL_CLIENTE es requerido.");
        AgregarErrorSiVacio(errores, request.COD_UBIGEO_CLIENTE, "COD_UBIGEO_CLIENTE es requerido.");
        AgregarErrorSiVacio(errores, request.DIRECCION_CLIENTE, "DIRECCION_CLIENTE es requerido.");
        AgregarErrorSiVacio(errores, request.DEPARTAMENTO_CLIENTE, "DEPARTAMENTO_CLIENTE es requerido.");
        AgregarErrorSiVacio(errores, request.PROVINCIA_CLIENTE, "PROVINCIA_CLIENTE es requerido.");
        AgregarErrorSiVacio(errores, request.DISTRITO_CLIENTE, "DISTRITO_CLIENTE es requerido.");
        AgregarErrorSiVacio(errores, request.COD_PAIS_CLIENTE, "COD_PAIS_CLIENTE es requerido.");
        AgregarErrorSiVacio(errores, request.TIPO_COMPROBANTE_MODIFICA, "TIPO_COMPROBANTE_MODIFICA es requerido.");
        AgregarErrorSiVacio(errores, request.NRO_DOCUMENTO_MODIFICA, "NRO_DOCUMENTO_MODIFICA es requerido.");
        AgregarErrorSiVacio(errores, request.COD_TIPO_MOTIVO, "COD_TIPO_MOTIVO es requerido.");
        AgregarErrorSiVacio(errores, request.DESCRIPCION_MOTIVO, "DESCRIPCION_MOTIVO es requerido.");

        if (!EsFechaIsoValida(request.FECHA_DOCUMENTO))
        {
            errores.Add("FECHA_DOCUMENTO debe tener formato yyyy-MM-dd.");
        }

        if (!string.IsNullOrWhiteSpace(request.FECHA_VTO) && !EsFechaIsoValida(request.FECHA_VTO))
        {
            errores.Add("FECHA_VTO debe tener formato yyyy-MM-dd.");
        }

        if (request.detalle is null || request.detalle.Count == 0)
        {
            errores.Add("detalle es requerido y debe contener al menos un elemento.");
            return errores;
        }

        for (var i = 0; i < request.detalle.Count; i++)
        {
            var item = request.detalle[i];
            if (item is null)
            {
                errores.Add($"detalle[{i}] no puede ser null.");
                continue;
            }

            if (!item.item.HasValue || item.item <= 0)
            {
                errores.Add($"detalle[{i}].item es requerido y debe ser mayor a 0.");
            }

            if (!item.cantidad.HasValue || item.cantidad <= 0)
            {
                errores.Add($"detalle[{i}].cantidad es requerido y debe ser mayor a 0.");
            }

            if (!item.importe.HasValue || item.importe < 0)
            {
                errores.Add($"detalle[{i}].importe es requerido y debe ser mayor o igual a 0.");
            }

            if (!item.precio.HasValue || item.precio < 0)
            {
                errores.Add($"detalle[{i}].precio es requerido y debe ser mayor o igual a 0.");
            }

            AgregarErrorSiVacio(errores, item.descripcion, $"detalle[{i}].descripcion es requerido.");
            AgregarErrorSiVacio(errores, item.codigoSunat, $"detalle[{i}].codigoSunat es requerido.");
            AgregarErrorSiVacio(errores, item.codTipoOperacion, $"detalle[{i}].codTipoOperacion es requerido.");
        }

        return errores;
    }

    private static string NormalizarHoraRegistro(string? horaRegistro)
    {
        var ahora = ObtenerAhoraCpe().ToString("HH:mm:ss", CultureInfo.InvariantCulture);
        if (string.IsNullOrWhiteSpace(horaRegistro))
        {
            return ahora;
        }

        var raw = horaRegistro.Trim();
        if (raw == "0" || raw == "00:00" || raw == "00:00:00" || raw == "0:0:0")
        {
            return ahora;
        }

        if (TryParseHoraSimple(raw, out var horaSimple))
        {
            return horaSimple;
        }

        if (DateTimeOffset.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out var parsedOffset) ||
            DateTimeOffset.TryParse(raw, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out parsedOffset))
        {
            var horaLocal = ConvertirACpe(parsedOffset);
            if (horaLocal.TimeOfDay == TimeSpan.Zero)
            {
                return ahora;
            }

            return horaLocal.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
        }

        if (DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed) ||
            DateTime.TryParse(raw, CultureInfo.CurrentCulture, DateTimeStyles.None, out parsed))
        {
            if (parsed.TimeOfDay == TimeSpan.Zero)
            {
                return ahora;
            }

            return parsed.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
        }

        return raw;
    }

    private static string ResolverHoraRegistroDesdeNota(DateTime? notaFechaPago, DateTime? notaFecha)
    {
        var fuente = notaFechaPago ?? notaFecha;
        if (!fuente.HasValue)
        {
            return ObtenerAhoraCpe().ToString("HH:mm:ss", CultureInfo.InvariantCulture);
        }

        var valor = fuente.Value;
        if (valor.Kind == DateTimeKind.Unspecified)
        {
            return valor.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
        }

        // Preserva offset (por ejemplo Z) para convertir correctamente a hora CPE (Lima).
        return new DateTimeOffset(valor).ToString("o", CultureInfo.InvariantCulture);
    }

    private static bool TryParseHoraSimple(string valor, out string horaNormalizada)
    {
        horaNormalizada = string.Empty;
        var formatos = new[] { "H:m", "H:m:s", "HH:mm", "HH:mm:ss" };

        if (DateTime.TryParseExact(valor, formatos, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed) ||
            DateTime.TryParseExact(valor, formatos, CultureInfo.CurrentCulture, DateTimeStyles.None, out parsed))
        {
            horaNormalizada = parsed.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            return true;
        }

        return false;
    }

    private static DateTime ObtenerAhoraCpe()
    {
        return ConvertirACpe(DateTimeOffset.UtcNow);
    }

    private static DateTime ConvertirACpe(DateTimeOffset valor)
    {
        return TimeZoneInfo.ConvertTime(valor, ResolverZonaHorariaCpe()).DateTime;
    }

    private static TimeZoneInfo ResolverZonaHorariaCpe()
    {
        var zonas = new[] { "SA Pacific Standard Time", "America/Lima" };
        foreach (var zonaId in zonas)
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(zonaId);
            }
            catch
            {
                // Continuar con siguiente zona.
            }
        }

        return TimeZoneInfo.Local;
    }

    private static EnviarFacturaRequest NormalizarRequestFactura(EnviarFacturaRequest request)
    {
        request.COD_TIPO_DOCUMENTO = "01";
        request.TIPO_OPERACION = string.IsNullOrWhiteSpace(request.TIPO_OPERACION) ? "0101" : request.TIPO_OPERACION.Trim();
        request.COD_MONEDA = string.IsNullOrWhiteSpace(request.COD_MONEDA) ? "PEN" : request.COD_MONEDA.Trim().ToUpperInvariant();
        request.TIPO_DOCUMENTO_EMPRESA = string.IsNullOrWhiteSpace(request.TIPO_DOCUMENTO_EMPRESA) ? "6" : request.TIPO_DOCUMENTO_EMPRESA.Trim();
        request.CODIGO_PAIS_EMPRESA = string.IsNullOrWhiteSpace(request.CODIGO_PAIS_EMPRESA) ? "PE" : request.CODIGO_PAIS_EMPRESA.Trim().ToUpperInvariant();
        request.COD_PAIS_CLIENTE = string.IsNullOrWhiteSpace(request.COD_PAIS_CLIENTE) ? "PE" : request.COD_PAIS_CLIENTE.Trim().ToUpperInvariant();
        request.CODIGO_ANEXO = string.IsNullOrWhiteSpace(request.CODIGO_ANEXO) ? "0000" : request.CODIGO_ANEXO.Trim();
        request.NOMBRE_COMERCIAL_EMPRESA = string.IsNullOrWhiteSpace(request.NOMBRE_COMERCIAL_EMPRESA)
            ? request.RAZON_SOCIAL_EMPRESA?.Trim()
            : request.NOMBRE_COMERCIAL_EMPRESA.Trim();
        request.CONTACTO_EMPRESA = request.CONTACTO_EMPRESA?.Trim() ?? string.Empty;
        request.HORA_REGISTRO = NormalizarHoraRegistro(request.HORA_REGISTRO);
        request.FORMA_PAGO = NormalizarFormaPago(request.FORMA_PAGO);
        request.FECHA_VTO = string.IsNullOrWhiteSpace(request.FECHA_VTO)
            ? request.FECHA_DOCUMENTO?.Trim()
            : request.FECHA_VTO.Trim();
        request.TIPO_DOCUMENTO_CLIENTE = InferirTipoDocumentoCliente(request.TIPO_DOCUMENTO_CLIENTE, request.NRO_DOCUMENTO_CLIENTE);
        request.CIUDAD_CLIENTE = string.IsNullOrWhiteSpace(request.CIUDAD_CLIENTE)
            ? request.PROVINCIA_CLIENTE?.Trim()
            : request.CIUDAD_CLIENTE.Trim();
        request.TOTAL_LETRAS ??= string.Empty;
        request.GLOSA ??= string.Empty;
        request.NRO_GUIA_REMISION ??= string.Empty;
        request.FECHA_GUIA_REMISION ??= string.Empty;
        request.COD_GUIA_REMISION = string.IsNullOrWhiteSpace(request.COD_GUIA_REMISION) ? "09" : request.COD_GUIA_REMISION.Trim();
        request.NRO_OTR_COMPROBANTE ??= string.Empty;
        request.COD_OTR_COMPROBANTE ??= string.Empty;
        request.CUENTA_DETRACCION ??= string.Empty;
        request.MONTO_DETRACCION ??= 0m;
        request.PORCENTAJE_DES ??= 0m;

        request.detalle ??= new List<EnviarFacturaDetalleRequest>();
        for (var i = 0; i < request.detalle.Count; i++)
        {
            request.detalle[i] = NormalizarDetalleFactura(request.detalle[i], i + 1);
        }

        request.POR_IGV ??= PorcentajeIgvDefault;
        request.TOTAL_ISC ??= request.detalle.Sum(x => x.isc ?? 0m);
        request.TOTAL_ICBPER ??= request.detalle.Sum(x => Convert.ToDecimal(x.impuestoIcbper ?? 0d));
        request.TOTAL_OTR_IMP ??= 0m;
        request.TOTAL_GRATUITAS ??= 0m;
        request.TOTAL_DESCUENTO ??= request.detalle.Sum(x => x.descuento ?? 0m);

        var totalBrutoDetalle = request.detalle.Sum(x => x.importe ?? 0m);
        request.TOTAL ??= totalBrutoDetalle + (request.TOTAL_ICBPER ?? 0m);

        AplicarCalculoTributarioParaEnvio(request);

        return request;
    }

    private static EnviarFacturaRequest NormalizarRequestBoleta(EnviarFacturaRequest request)
    {
        request.COD_TIPO_DOCUMENTO = "03";
        request.TIPO_OPERACION = string.IsNullOrWhiteSpace(request.TIPO_OPERACION) ? "0101" : request.TIPO_OPERACION.Trim();
        request.COD_MONEDA = string.IsNullOrWhiteSpace(request.COD_MONEDA) ? "PEN" : request.COD_MONEDA.Trim().ToUpperInvariant();
        request.TIPO_DOCUMENTO_EMPRESA = string.IsNullOrWhiteSpace(request.TIPO_DOCUMENTO_EMPRESA) ? "6" : request.TIPO_DOCUMENTO_EMPRESA.Trim();
        request.CODIGO_PAIS_EMPRESA = string.IsNullOrWhiteSpace(request.CODIGO_PAIS_EMPRESA) ? "PE" : request.CODIGO_PAIS_EMPRESA.Trim().ToUpperInvariant();
        request.COD_PAIS_CLIENTE = string.IsNullOrWhiteSpace(request.COD_PAIS_CLIENTE) ? "PE" : request.COD_PAIS_CLIENTE.Trim().ToUpperInvariant();
        request.CODIGO_ANEXO = string.IsNullOrWhiteSpace(request.CODIGO_ANEXO) ? "0000" : request.CODIGO_ANEXO.Trim();
        request.NOMBRE_COMERCIAL_EMPRESA = string.IsNullOrWhiteSpace(request.NOMBRE_COMERCIAL_EMPRESA)
            ? request.RAZON_SOCIAL_EMPRESA?.Trim()
            : request.NOMBRE_COMERCIAL_EMPRESA.Trim();
        request.CONTACTO_EMPRESA = request.CONTACTO_EMPRESA?.Trim() ?? string.Empty;
        request.HORA_REGISTRO = NormalizarHoraRegistro(request.HORA_REGISTRO);
        request.FORMA_PAGO = NormalizarFormaPago(request.FORMA_PAGO);
        request.FECHA_VTO = string.IsNullOrWhiteSpace(request.FECHA_VTO)
            ? request.FECHA_DOCUMENTO?.Trim()
            : request.FECHA_VTO.Trim();
        request.CIUDAD_CLIENTE = string.IsNullOrWhiteSpace(request.CIUDAD_CLIENTE)
            ? request.PROVINCIA_CLIENTE?.Trim()
            : request.CIUDAD_CLIENTE.Trim();
        request.TOTAL_LETRAS ??= string.Empty;
        request.GLOSA ??= string.Empty;
        request.NRO_GUIA_REMISION ??= string.Empty;
        request.FECHA_GUIA_REMISION ??= string.Empty;
        request.COD_GUIA_REMISION = string.IsNullOrWhiteSpace(request.COD_GUIA_REMISION) ? "09" : request.COD_GUIA_REMISION.Trim();
        request.NRO_OTR_COMPROBANTE ??= string.Empty;
        request.COD_OTR_COMPROBANTE ??= string.Empty;
        request.CUENTA_DETRACCION ??= string.Empty;
        request.MONTO_DETRACCION ??= 0m;
        request.PORCENTAJE_DES ??= 0m;

        request.detalle ??= new List<EnviarFacturaDetalleRequest>();
        for (var i = 0; i < request.detalle.Count; i++)
        {
            request.detalle[i] = NormalizarDetalleFactura(request.detalle[i], i + 1);
        }

        request.POR_IGV ??= PorcentajeIgvDefault;
        request.TOTAL_ISC ??= request.detalle.Sum(x => x.isc ?? 0m);
        request.TOTAL_ICBPER ??= request.detalle.Sum(x => Convert.ToDecimal(x.impuestoIcbper ?? 0d));
        request.TOTAL_OTR_IMP ??= 0m;
        request.TOTAL_GRATUITAS ??= 0m;
        request.TOTAL_DESCUENTO ??= request.detalle.Sum(x => x.descuento ?? 0m);

        var totalBrutoDetalle = request.detalle.Sum(x => x.importe ?? 0m);
        request.TOTAL ??= totalBrutoDetalle + (request.TOTAL_ICBPER ?? 0m);

        AplicarCalculoTributarioParaEnvio(request);

        var nroDocCliente = (request.NRO_DOCUMENTO_CLIENTE ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(nroDocCliente))
        {
            nroDocCliente = "00000000";
        }

        request.NRO_DOCUMENTO_CLIENTE = nroDocCliente;
        request.TIPO_DOCUMENTO_CLIENTE = InferirTipoDocumentoCliente(request.TIPO_DOCUMENTO_CLIENTE, nroDocCliente);
        if (string.IsNullOrWhiteSpace(request.TIPO_DOCUMENTO_CLIENTE))
        {
            request.TIPO_DOCUMENTO_CLIENTE = "1";
        }

        request.RAZON_SOCIAL_CLIENTE = string.IsNullOrWhiteSpace(request.RAZON_SOCIAL_CLIENTE)
            ? "VARIOS"
            : request.RAZON_SOCIAL_CLIENTE.Trim();

        return request;
    }

    private static EnviarFacturaRequest NormalizarRequestNotaCredito(EnviarFacturaRequest request)
    {
        request.COD_TIPO_DOCUMENTO = "07";
        request.TIPO_OPERACION = string.IsNullOrWhiteSpace(request.TIPO_OPERACION) ? "0101" : request.TIPO_OPERACION.Trim();
        request.COD_MONEDA = string.IsNullOrWhiteSpace(request.COD_MONEDA) ? "PEN" : request.COD_MONEDA.Trim().ToUpperInvariant();
        request.TIPO_DOCUMENTO_EMPRESA = string.IsNullOrWhiteSpace(request.TIPO_DOCUMENTO_EMPRESA) ? "6" : request.TIPO_DOCUMENTO_EMPRESA.Trim();
        request.CODIGO_PAIS_EMPRESA = string.IsNullOrWhiteSpace(request.CODIGO_PAIS_EMPRESA) ? "PE" : request.CODIGO_PAIS_EMPRESA.Trim().ToUpperInvariant();
        request.COD_PAIS_CLIENTE = string.IsNullOrWhiteSpace(request.COD_PAIS_CLIENTE) ? "PE" : request.COD_PAIS_CLIENTE.Trim().ToUpperInvariant();
        request.CODIGO_ANEXO = string.IsNullOrWhiteSpace(request.CODIGO_ANEXO) ? "0000" : request.CODIGO_ANEXO.Trim();
        request.NOMBRE_COMERCIAL_EMPRESA = string.IsNullOrWhiteSpace(request.NOMBRE_COMERCIAL_EMPRESA)
            ? request.RAZON_SOCIAL_EMPRESA?.Trim()
            : request.NOMBRE_COMERCIAL_EMPRESA.Trim();
        request.CONTACTO_EMPRESA = request.CONTACTO_EMPRESA?.Trim() ?? string.Empty;
        request.HORA_REGISTRO = NormalizarHoraRegistro(request.HORA_REGISTRO);
        request.FORMA_PAGO = NormalizarFormaPago(request.FORMA_PAGO);
        request.FECHA_VTO = string.IsNullOrWhiteSpace(request.FECHA_VTO)
            ? request.FECHA_DOCUMENTO?.Trim()
            : request.FECHA_VTO.Trim();
        request.TIPO_DOCUMENTO_CLIENTE = InferirTipoDocumentoCliente(request.TIPO_DOCUMENTO_CLIENTE, request.NRO_DOCUMENTO_CLIENTE);
        request.CIUDAD_CLIENTE = string.IsNullOrWhiteSpace(request.CIUDAD_CLIENTE)
            ? request.PROVINCIA_CLIENTE?.Trim()
            : request.CIUDAD_CLIENTE.Trim();
        request.TIPO_COMPROBANTE_MODIFICA = (request.TIPO_COMPROBANTE_MODIFICA ?? string.Empty).Trim();
        request.NRO_DOCUMENTO_MODIFICA = (request.NRO_DOCUMENTO_MODIFICA ?? string.Empty).Trim();
        request.COD_TIPO_MOTIVO = (request.COD_TIPO_MOTIVO ?? string.Empty).Trim();
        request.DESCRIPCION_MOTIVO = (request.DESCRIPCION_MOTIVO ?? string.Empty).Trim();
        request.TOTAL_LETRAS ??= string.Empty;
        request.GLOSA ??= string.Empty;
        request.NRO_GUIA_REMISION ??= string.Empty;
        request.FECHA_GUIA_REMISION ??= string.Empty;
        request.COD_GUIA_REMISION = string.IsNullOrWhiteSpace(request.COD_GUIA_REMISION) ? "09" : request.COD_GUIA_REMISION.Trim();
        request.NRO_OTR_COMPROBANTE ??= string.Empty;
        request.COD_OTR_COMPROBANTE ??= string.Empty;
        request.CUENTA_DETRACCION ??= string.Empty;
        request.MONTO_DETRACCION ??= 0m;
        request.PORCENTAJE_DES ??= 0m;

        request.detalle ??= new List<EnviarFacturaDetalleRequest>();
        for (var i = 0; i < request.detalle.Count; i++)
        {
            request.detalle[i] = NormalizarDetalleFactura(request.detalle[i], i + 1);
        }

        request.POR_IGV ??= PorcentajeIgvDefault;
        request.TOTAL_ISC ??= request.detalle.Sum(x => x.isc ?? 0m);
        request.TOTAL_ICBPER ??= request.detalle.Sum(x => Convert.ToDecimal(x.impuestoIcbper ?? 0d));
        request.TOTAL_OTR_IMP ??= 0m;
        request.TOTAL_GRATUITAS ??= 0m;
        request.TOTAL_DESCUENTO ??= request.detalle.Sum(x => x.descuento ?? 0m);

        var totalBrutoDetalle = request.detalle.Sum(x => x.importe ?? 0m);
        request.TOTAL ??= totalBrutoDetalle + (request.TOTAL_ICBPER ?? 0m);

        AplicarCalculoTributarioParaEnvio(request);

        return request;
    }

    private static List<string> AplicarCodigoSunatFallback(EnviarFacturaRequest request, string codigoFallback)
    {
        var itemsSinCodigo = new List<string>();

        if (request.detalle is null || request.detalle.Count == 0)
        {
            return itemsSinCodigo;
        }

        for (var i = 0; i < request.detalle.Count; i++)
        {
            var item = request.detalle[i];
            if (item is null)
            {
                continue;
            }

            if (!string.IsNullOrWhiteSpace(item.codigoSunat))
            {
                continue;
            }

            item.codigoSunat = codigoFallback;
            var etiqueta = string.IsNullOrWhiteSpace(item.descripcion)
                ? $"item {i + 1}"
                : $"{i + 1} - {item.descripcion.Trim()}";
            itemsSinCodigo.Add(etiqueta);
        }

        return itemsSinCodigo;
    }

    private static EnviarFacturaDetalleRequest NormalizarDetalleFactura(EnviarFacturaDetalleRequest? item, int itemIndex)
    {
        item ??= new EnviarFacturaDetalleRequest();
        item.item ??= itemIndex;
        item.unidadMedida = NormalizarUnidadMedidaSunat(item.unidadMedida, null);
        item.precioTipoCodigo = string.IsNullOrWhiteSpace(item.precioTipoCodigo) ? "01" : item.precioTipoCodigo.Trim();
        item.codTipoOperacion = string.IsNullOrWhiteSpace(item.codTipoOperacion) ? "10" : item.codTipoOperacion.Trim();
        item.codigo = string.IsNullOrWhiteSpace(item.codigo) ? $"ITEM{item.item}" : item.codigo.Trim();
        item.codigoSunat = item.codigoSunat?.Trim();
        item.descripcion = item.descripcion?.Trim();
        item.cantidad ??= 0m;
        item.igv ??= 0m;
        item.isc ??= 0m;
        item.descuento ??= 0m;
        item.impuestoIcbper ??= 0d;
        item.cantidadBolsas ??= 0;
        item.sunatIcbper ??= 0d;
        item.tipoIsc = string.IsNullOrWhiteSpace(item.tipoIsc) ? string.Empty : item.tipoIsc.Trim();
        item.biIsc ??= 0m;
        item.porIsc ??= 0m;

        if (!item.importe.HasValue && item.cantidad > 0 && item.precio.HasValue)
        {
            item.importe = item.cantidad.Value * item.precio.Value;
        }

        if (!item.importe.HasValue && item.cantidad > 0 && item.precioSinImpuesto.HasValue)
        {
            item.importe = item.cantidad.Value * item.precioSinImpuesto.Value;
        }

        if (!item.precio.HasValue && item.cantidad > 0)
        {
            item.precio = item.importe.Value / item.cantidad.Value;
        }

        item.subTotal ??= item.importe ?? 0m;
        item.biIsc ??= item.importe ?? 0m;

        return item;
    }

    private static CPE MapearFacturaLegacy(EnviarFacturaRequest request, int tipoProceso, string rutaPfxNormalizada)
    {
        return MapearComprobanteVentaLegacy(request, tipoProceso, rutaPfxNormalizada, "01");
    }

    private static CPE MapearBoletaLegacy(EnviarFacturaRequest request, int tipoProceso, string rutaPfxNormalizada)
    {
        return MapearComprobanteVentaLegacy(request, tipoProceso, rutaPfxNormalizada, "03");
    }

    private static CPE MapearComprobanteVentaLegacy(
        EnviarFacturaRequest request,
        int tipoProceso,
        string rutaPfxNormalizada,
        string codigoTipoDocumento)
    {
        var comprobante = new CPE
        {
            TIPO_OPERACION = request.TIPO_OPERACION?.Trim(),
            HORA_REGISTRO = request.HORA_REGISTRO?.Trim(),
            TOTAL_GRAVADAS = request.TOTAL_GRAVADAS ?? 0m,
            TOTAL_INAFECTA = request.TOTAL_INAFECTA ?? 0m,
            TOTAL_EXONERADAS = request.TOTAL_EXONERADAS ?? 0m,
            TOTAL_GRATUITAS = request.TOTAL_GRATUITAS ?? 0m,
            TOTAL_DESCUENTO = request.TOTAL_DESCUENTO ?? 0m,
            SUB_TOTAL = request.SUB_TOTAL ?? 0m,
            POR_IGV = request.POR_IGV ?? 18m,
            TOTAL_IGV = request.TOTAL_IGV ?? 0m,
            TOTAL_ISC = request.TOTAL_ISC ?? 0m,
            TOTAL_EXPORTACION = request.TOTAL_EXPORTACION ?? 0m,
            TOTAL_OTR_IMP = request.TOTAL_OTR_IMP ?? 0m,
            TOTAL_ICBPER = request.TOTAL_ICBPER ?? 0m,
            TOTAL = request.TOTAL ?? 0m,
            TOTAL_LETRAS = request.TOTAL_LETRAS?.Trim() ?? string.Empty,
            NRO_GUIA_REMISION = request.NRO_GUIA_REMISION?.Trim() ?? string.Empty,
            FECHA_GUIA_REMISION = request.FECHA_GUIA_REMISION?.Trim() ?? string.Empty,
            COD_GUIA_REMISION = request.COD_GUIA_REMISION?.Trim() ?? string.Empty,
            NRO_OTR_COMPROBANTE = request.NRO_OTR_COMPROBANTE?.Trim() ?? string.Empty,
            COD_OTR_COMPROBANTE = request.COD_OTR_COMPROBANTE?.Trim() ?? string.Empty,
            NRO_COMPROBANTE = request.NRO_COMPROBANTE?.Trim(),
            FECHA_DOCUMENTO = request.FECHA_DOCUMENTO?.Trim(),
            COD_TIPO_DOCUMENTO = codigoTipoDocumento,
            COD_MONEDA = request.COD_MONEDA?.Trim(),
            NRO_DOCUMENTO_CLIENTE = request.NRO_DOCUMENTO_CLIENTE?.Trim(),
            RAZON_SOCIAL_CLIENTE = request.RAZON_SOCIAL_CLIENTE?.Trim(),
            TIPO_DOCUMENTO_CLIENTE = request.TIPO_DOCUMENTO_CLIENTE?.Trim(),
            DIRECCION_CLIENTE = request.DIRECCION_CLIENTE?.Trim(),
            CIUDAD_CLIENTE = request.CIUDAD_CLIENTE?.Trim(),
            COD_PAIS_CLIENTE = request.COD_PAIS_CLIENTE?.Trim(),
            COD_UBIGEO_CLIENTE = request.COD_UBIGEO_CLIENTE?.Trim(),
            DEPARTAMENTO_CLIENTE = request.DEPARTAMENTO_CLIENTE?.Trim(),
            PROVINCIA_CLIENTE = request.PROVINCIA_CLIENTE?.Trim(),
            DISTRITO_CLIENTE = request.DISTRITO_CLIENTE?.Trim(),
            NRO_DOCUMENTO_EMPRESA = request.NRO_DOCUMENTO_EMPRESA?.Trim(),
            TIPO_DOCUMENTO_EMPRESA = request.TIPO_DOCUMENTO_EMPRESA?.Trim(),
            NOMBRE_COMERCIAL_EMPRESA = request.NOMBRE_COMERCIAL_EMPRESA?.Trim(),
            CODIGO_UBIGEO_EMPRESA = request.CODIGO_UBIGEO_EMPRESA?.Trim(),
            DIRECCION_EMPRESA = request.DIRECCION_EMPRESA?.Trim(),
            CONTACTO_EMPRESA = request.CONTACTO_EMPRESA?.Trim(),
            DEPARTAMENTO_EMPRESA = request.DEPARTAMENTO_EMPRESA?.Trim(),
            PROVINCIA_EMPRESA = request.PROVINCIA_EMPRESA?.Trim(),
            DISTRITO_EMPRESA = request.DISTRITO_EMPRESA?.Trim(),
            CODIGO_PAIS_EMPRESA = request.CODIGO_PAIS_EMPRESA?.Trim(),
            RAZON_SOCIAL_EMPRESA = request.RAZON_SOCIAL_EMPRESA?.Trim(),
            USUARIO_SOL_EMPRESA = request.USUARIO_SOL_EMPRESA?.Trim(),
            PASS_SOL_EMPRESA = request.PASS_SOL_EMPRESA?.Trim(),
            CONTRA_FIRMA = request.CONTRA_FIRMA?.Trim(),
            TIPO_PROCESO = tipoProceso,
            FECHA_VTO = request.FECHA_VTO?.Trim(),
            FORMA_PAGO = request.FORMA_PAGO?.Trim(),
            GLOSA = request.GLOSA?.Trim(),
            RUTA_PFX = rutaPfxNormalizada,
            CODIGO_ANEXO = request.CODIGO_ANEXO?.Trim(),
            CUENTA_DETRACCION = request.CUENTA_DETRACCION?.Trim(),
            MONTO_DETRACCION = request.MONTO_DETRACCION ?? 0m,
            PORCENTAJE_DES = request.PORCENTAJE_DES ?? 0m
        };

        foreach (var detalle in request.detalle ?? Enumerable.Empty<EnviarFacturaDetalleRequest>())
        {
            comprobante.detalle.Add(new CPE_DETALLE
            {
                ITEM = detalle.item,
                UNIDAD_MEDIDA = detalle.unidadMedida?.Trim(),
                CANTIDAD = detalle.cantidad ?? 0m,
                PRECIO = detalle.precio ?? 0m,
                IMPORTE = detalle.importe ?? 0m,
                IMPUESTO_ICBPER = detalle.impuestoIcbper ?? 0d,
                CANTIDAD_BOLSAS = detalle.cantidadBolsas ?? 0,
                SUNAT_ICBPER = detalle.sunatIcbper ?? 0d,
                PRECIO_TIPO_CODIGO = detalle.precioTipoCodigo?.Trim(),
                IGV = detalle.igv ?? 0m,
                BI_ISC = detalle.biIsc ?? 0m,
                POR_ISC = detalle.porIsc ?? 0m,
                TIPO_ISC = detalle.tipoIsc?.Trim() ?? string.Empty,
                ISC = detalle.isc ?? 0m,
                COD_TIPO_OPERACION = detalle.codTipoOperacion?.Trim(),
                CODIGO = detalle.codigo?.Trim(),
                CODIGO_SUNAT = detalle.codigoSunat?.Trim(),
                DESCRIPCION = detalle.descripcion?.Trim(),
                DESCUENTO = detalle.descuento ?? 0m,
                SUB_TOTAL = detalle.subTotal ?? 0m,
                PRECIO_SIN_IMPUESTO = detalle.precioSinImpuesto ?? 0m
            });
        }

        return comprobante;
    }

    private static CPE MapearNotaCreditoLegacy(EnviarFacturaRequest request, int tipoProceso, string rutaPfxNormalizada)
    {
        var notaCredito = new CPE
        {
            TIPO_OPERACION = request.TIPO_OPERACION?.Trim(),
            HORA_REGISTRO = request.HORA_REGISTRO?.Trim(),
            TOTAL_GRAVADAS = request.TOTAL_GRAVADAS ?? 0m,
            TOTAL_INAFECTA = request.TOTAL_INAFECTA ?? 0m,
            TOTAL_EXONERADAS = request.TOTAL_EXONERADAS ?? 0m,
            TOTAL_GRATUITAS = request.TOTAL_GRATUITAS ?? 0m,
            TOTAL_DESCUENTO = request.TOTAL_DESCUENTO ?? 0m,
            SUB_TOTAL = request.SUB_TOTAL ?? 0m,
            POR_IGV = request.POR_IGV ?? 18m,
            TOTAL_IGV = request.TOTAL_IGV ?? 0m,
            TOTAL_ISC = request.TOTAL_ISC ?? 0m,
            TOTAL_EXPORTACION = request.TOTAL_EXPORTACION ?? 0m,
            TOTAL_OTR_IMP = request.TOTAL_OTR_IMP ?? 0m,
            TOTAL_ICBPER = request.TOTAL_ICBPER ?? 0m,
            TOTAL = request.TOTAL ?? 0m,
            TOTAL_LETRAS = request.TOTAL_LETRAS?.Trim() ?? string.Empty,
            NRO_GUIA_REMISION = request.NRO_GUIA_REMISION?.Trim() ?? string.Empty,
            FECHA_GUIA_REMISION = request.FECHA_GUIA_REMISION?.Trim() ?? string.Empty,
            COD_GUIA_REMISION = request.COD_GUIA_REMISION?.Trim() ?? string.Empty,
            NRO_OTR_COMPROBANTE = request.NRO_OTR_COMPROBANTE?.Trim() ?? string.Empty,
            COD_OTR_COMPROBANTE = request.COD_OTR_COMPROBANTE?.Trim() ?? string.Empty,
            TIPO_COMPROBANTE_MODIFICA = request.TIPO_COMPROBANTE_MODIFICA?.Trim() ?? string.Empty,
            NRO_DOCUMENTO_MODIFICA = request.NRO_DOCUMENTO_MODIFICA?.Trim() ?? string.Empty,
            COD_TIPO_MOTIVO = request.COD_TIPO_MOTIVO?.Trim() ?? string.Empty,
            DESCRIPCION_MOTIVO = request.DESCRIPCION_MOTIVO?.Trim() ?? string.Empty,
            NRO_COMPROBANTE = request.NRO_COMPROBANTE?.Trim(),
            FECHA_DOCUMENTO = request.FECHA_DOCUMENTO?.Trim(),
            COD_TIPO_DOCUMENTO = "07",
            COD_MONEDA = request.COD_MONEDA?.Trim(),
            NRO_DOCUMENTO_CLIENTE = request.NRO_DOCUMENTO_CLIENTE?.Trim(),
            RAZON_SOCIAL_CLIENTE = request.RAZON_SOCIAL_CLIENTE?.Trim(),
            TIPO_DOCUMENTO_CLIENTE = request.TIPO_DOCUMENTO_CLIENTE?.Trim(),
            DIRECCION_CLIENTE = request.DIRECCION_CLIENTE?.Trim(),
            CIUDAD_CLIENTE = request.CIUDAD_CLIENTE?.Trim(),
            COD_PAIS_CLIENTE = request.COD_PAIS_CLIENTE?.Trim(),
            COD_UBIGEO_CLIENTE = request.COD_UBIGEO_CLIENTE?.Trim(),
            DEPARTAMENTO_CLIENTE = request.DEPARTAMENTO_CLIENTE?.Trim(),
            PROVINCIA_CLIENTE = request.PROVINCIA_CLIENTE?.Trim(),
            DISTRITO_CLIENTE = request.DISTRITO_CLIENTE?.Trim(),
            NRO_DOCUMENTO_EMPRESA = request.NRO_DOCUMENTO_EMPRESA?.Trim(),
            TIPO_DOCUMENTO_EMPRESA = request.TIPO_DOCUMENTO_EMPRESA?.Trim(),
            NOMBRE_COMERCIAL_EMPRESA = request.NOMBRE_COMERCIAL_EMPRESA?.Trim(),
            CODIGO_UBIGEO_EMPRESA = request.CODIGO_UBIGEO_EMPRESA?.Trim(),
            DIRECCION_EMPRESA = request.DIRECCION_EMPRESA?.Trim(),
            CONTACTO_EMPRESA = request.CONTACTO_EMPRESA?.Trim(),
            DEPARTAMENTO_EMPRESA = request.DEPARTAMENTO_EMPRESA?.Trim(),
            PROVINCIA_EMPRESA = request.PROVINCIA_EMPRESA?.Trim(),
            DISTRITO_EMPRESA = request.DISTRITO_EMPRESA?.Trim(),
            CODIGO_PAIS_EMPRESA = request.CODIGO_PAIS_EMPRESA?.Trim(),
            RAZON_SOCIAL_EMPRESA = request.RAZON_SOCIAL_EMPRESA?.Trim(),
            USUARIO_SOL_EMPRESA = request.USUARIO_SOL_EMPRESA?.Trim(),
            PASS_SOL_EMPRESA = request.PASS_SOL_EMPRESA?.Trim(),
            CONTRA_FIRMA = request.CONTRA_FIRMA?.Trim(),
            TIPO_PROCESO = tipoProceso,
            FECHA_VTO = request.FECHA_VTO?.Trim(),
            FORMA_PAGO = request.FORMA_PAGO?.Trim(),
            GLOSA = request.GLOSA?.Trim(),
            RUTA_PFX = rutaPfxNormalizada,
            CODIGO_ANEXO = request.CODIGO_ANEXO?.Trim(),
            CUENTA_DETRACCION = request.CUENTA_DETRACCION?.Trim(),
            MONTO_DETRACCION = request.MONTO_DETRACCION ?? 0m,
            PORCENTAJE_DES = request.PORCENTAJE_DES ?? 0m
        };

        foreach (var detalle in request.detalle ?? Enumerable.Empty<EnviarFacturaDetalleRequest>())
        {
            notaCredito.detalle.Add(new CPE_DETALLE
            {
                ITEM = detalle.item,
                UNIDAD_MEDIDA = detalle.unidadMedida?.Trim(),
                CANTIDAD = detalle.cantidad ?? 0m,
                PRECIO = detalle.precio ?? 0m,
                IMPORTE = detalle.importe ?? 0m,
                IMPUESTO_ICBPER = detalle.impuestoIcbper ?? 0d,
                CANTIDAD_BOLSAS = detalle.cantidadBolsas ?? 0,
                SUNAT_ICBPER = detalle.sunatIcbper ?? 0d,
                PRECIO_TIPO_CODIGO = detalle.precioTipoCodigo?.Trim(),
                IGV = detalle.igv ?? 0m,
                BI_ISC = detalle.biIsc ?? 0m,
                POR_ISC = detalle.porIsc ?? 0m,
                TIPO_ISC = detalle.tipoIsc?.Trim() ?? string.Empty,
                ISC = detalle.isc ?? 0m,
                COD_TIPO_OPERACION = detalle.codTipoOperacion?.Trim(),
                CODIGO = detalle.codigo?.Trim(),
                CODIGO_SUNAT = detalle.codigoSunat?.Trim(),
                DESCRIPCION = detalle.descripcion?.Trim(),
                DESCUENTO = detalle.descuento ?? 0m,
                SUB_TOTAL = detalle.subTotal ?? 0m,
                PRECIO_SIN_IMPUESTO = detalle.precioSinImpuesto ?? 0m
            });
        }

        return notaCredito;
    }

    private async Task<object> RegistrarFacturaEnBaseDatosAsync(
        EnviarFacturaRequest request,
        Dictionary<string, string>? respuestaLegacy,
        CancellationToken cancellationToken)
    {
        if (!request.NOTA_ID.HasValue || request.NOTA_ID.Value <= 0)
        {
            return new
            {
                ok = false,
                mensaje = "NOTA_ID es requerido para actualizar DocumentoVenta en BD."
            };
        }

        var codSunat = ObtenerValorLegacy(respuestaLegacy, "cod_sunat");
        if (!EsCodigoFacturaAceptado(codSunat))
        {
            return new
            {
                ok = false,
                mensaje = "No se actualizó BD porque SUNAT/OCE no devolvió aceptación de la factura."
            };
        }

        var mensajeSunat = ObtenerValorLegacy(respuestaLegacy, "msj_sunat");
        var hashCpe = ObtenerValorLegacy(respuestaLegacy, "hash_cpe");
        var data = string.Join("|", new[]
        {
            request.NOTA_ID.Value.ToString(CultureInfo.InvariantCulture),
            SanitizarCampoListaOrden(codSunat),
            SanitizarCampoListaOrden(mensajeSunat),
            SanitizarCampoListaOrden(hashCpe)
        });

        try
        {
            var resultado = await _mediator.ReenviarFacturaAsync(data, cancellationToken);
            if (string.IsNullOrWhiteSpace(resultado))
            {
                return new
                {
                    ok = true,
                    mensaje = "SUNAT/OCE aceptó la factura y se ejecutó uspReEnviarFactura, pero el SP no devolvió payload."
                };
            }

            return new
            {
                ok = true,
                resultado
            };
        }
        catch (Exception ex)
        {
            return new
            {
                ok = false,
                mensaje = $"SUNAT/OCE aceptó la factura, pero falló la actualización en BD: {ex.Message}"
            };
        }
    }

    private async Task<object> RegistrarBoletaEnBaseDatosAsync(
        EnviarFacturaRequest request,
        Dictionary<string, string>? respuestaLegacy,
        CancellationToken cancellationToken)
    {
        if (!request.NOTA_ID.HasValue || request.NOTA_ID.Value <= 0)
        {
            return new
            {
                ok = false,
                mensaje = "NOTA_ID es requerido para actualizar DocumentoVenta en BD."
            };
        }

        var codSunat = ObtenerValorLegacy(respuestaLegacy, "cod_sunat");
        if (!EsCodigoFacturaAceptado(codSunat))
        {
            return new
            {
                ok = false,
                mensaje = "No se actualizó BD porque SUNAT/OCE no devolvió aceptación de la boleta."
            };
        }

        var mensajeSunat = ObtenerValorLegacy(respuestaLegacy, "msj_sunat");
        var hashCpe = ObtenerValorLegacy(respuestaLegacy, "hash_cpe");
        var data = string.Join("|", new[]
        {
            request.NOTA_ID.Value.ToString(CultureInfo.InvariantCulture),
            SanitizarCampoListaOrden(codSunat),
            SanitizarCampoListaOrden(mensajeSunat),
            SanitizarCampoListaOrden(hashCpe)
        });

        try
        {
            var resultado = await _mediator.ReenviarFacturaAsync(data, cancellationToken);
            if (string.IsNullOrWhiteSpace(resultado))
            {
                return new
                {
                    ok = true,
                    mensaje = "SUNAT/OCE aceptó la boleta y se ejecutó uspReEnviarFactura, pero el SP no devolvió payload."
                };
            }

            return new
            {
                ok = true,
                resultado
            };
        }
        catch (Exception ex)
        {
            return new
            {
                ok = false,
                mensaje = $"SUNAT/OCE aceptó la boleta, pero falló la actualización en BD: {ex.Message}"
            };
        }
    }

    private static EstadoResultadoSunat ResolverEstadoResultadoSunat(string? flgRta, string? codSunat)
    {
        if (!string.Equals((flgRta ?? string.Empty).Trim(), "1", StringComparison.Ordinal))
        {
            return EstadoResultadoSunat.MantenerPendiente;
        }

        if (!int.TryParse((codSunat ?? string.Empty).Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var codigo))
        {
            return EstadoResultadoSunat.MantenerPendiente;
        }

        if (codigo == 1033 || (codigo >= 2000 && codigo <= 3999))
        {
            return EstadoResultadoSunat.Rechazado;
        }

        return EstadoResultadoSunat.MantenerPendiente;
    }

    private async Task<object> RegistrarResultadoNoAceptadoDocumentoVentaAsync(
        EnviarFacturaRequest request,
        string tipoCodigo,
        Dictionary<string, string>? respuestaLegacy,
        CancellationToken cancellationToken,
        bool actualizarNotaEnRechazo = true,
        string descripcionDocumento = "documento")
    {
        var notaId = request.NOTA_ID.GetValueOrDefault();
        var docuId = request.DOCU_ID.GetValueOrDefault();
        var descripcion = string.IsNullOrWhiteSpace(descripcionDocumento) ? "documento" : descripcionDocumento.Trim();

        if (notaId <= 0 && docuId <= 0)
        {
            return new
            {
                ok = false,
                mensaje = $"NOTA_ID o DOCU_ID es requerido para registrar estado no aceptado de {descripcion} en BD."
            };
        }

        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return new
            {
                ok = false,
                mensaje = "No se encontró la cadena de conexión para registrar estado no aceptado."
            };
        }

        var flgRta = ObtenerValorLegacy(respuestaLegacy, "flg_rta", "0");
        var codSunat = ObtenerValorLegacy(respuestaLegacy, "cod_sunat");
        var mensajeSunat = ObtenerValorLegacy(respuestaLegacy, "msj_sunat");
        var hashCpe = ObtenerValorLegacy(respuestaLegacy, "hash_cpe");
        var estadoResultado = ResolverEstadoResultadoSunat(flgRta, codSunat);
        var estadoSunatObjetivo = estadoResultado == EstadoResultadoSunat.Rechazado ? "RECHAZADO" : "PENDIENTE";
        var docuEstadoObjetivo = estadoResultado == EstadoResultadoSunat.Rechazado ? "RECHAZADO" : null;

        const string sqlActualizarDocumento = """
            ;WITH UltimoPendiente AS (
                SELECT TOP (1) d.DocuId
                FROM DocumentoVenta d
                WHERE d.TipoCodigo = @TipoCodigo
                  AND d.EstadoSunat = 'PENDIENTE'
                  AND (
                        (@DocuId > 0 AND d.DocuId = @DocuId)
                        OR (@DocuId <= 0 AND @NotaId > 0 AND d.NotaId = @NotaId)
                      )
                ORDER BY CASE WHEN @DocuId > 0 AND d.DocuId = @DocuId THEN 0 ELSE 1 END,
                         d.DocuId DESC
            )
            UPDATE d
            SET d.CodigoSunat = @CodigoSunat,
                d.MensajeSunat = @MensajeSunat,
                d.DocuHash = CASE WHEN NULLIF(@DocuHash, '') IS NULL THEN d.DocuHash ELSE @DocuHash END,
                d.EstadoSunat = @EstadoSunat,
                d.DocuEstado = CASE WHEN NULLIF(@DocuEstado, '') IS NULL THEN d.DocuEstado ELSE @DocuEstado END
            FROM DocumentoVenta d
            INNER JOIN UltimoPendiente u ON u.DocuId = d.DocuId;

            SELECT @@ROWCOUNT;
            """;

        const string sqlActualizarNotaRechazo = """
            UPDATE NotaPedido
            SET NotaEstado = 'PENDIENTE'
            WHERE NotaId = @NotaId;
            """;

        const string sqlActualizarDetalleRechazo = """
            UPDATE DetallePedido
            SET DetalleEstado = 'PENDIENTE'
            WHERE NotaId = @NotaId
              AND DetalleEstado = 'EMITIDO';
            """;

        try
        {
            await using var con = new SqlConnection(connectionString);
            await con.OpenAsync(cancellationToken);
            await using var tx = await con.BeginTransactionAsync(cancellationToken);

            await using var cmd = new SqlCommand(sqlActualizarDocumento, con, (SqlTransaction)tx);
            cmd.Parameters.AddWithValue("@NotaId", notaId);
            cmd.Parameters.AddWithValue("@DocuId", docuId);
            cmd.Parameters.AddWithValue("@TipoCodigo", (tipoCodigo ?? string.Empty).Trim());
            cmd.Parameters.AddWithValue("@CodigoSunat", (object?)codSunat ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MensajeSunat", (object?)mensajeSunat ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DocuHash", (object?)hashCpe ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EstadoSunat", estadoSunatObjetivo);
            cmd.Parameters.AddWithValue("@DocuEstado", (object?)docuEstadoObjetivo ?? DBNull.Value);

            var filasAfectadas = Convert.ToInt32(await cmd.ExecuteScalarAsync(cancellationToken) ?? 0, CultureInfo.InvariantCulture);
            if (filasAfectadas <= 0)
            {
                await tx.RollbackAsync(cancellationToken);
                return new
                {
                    ok = false,
                    mensaje = docuId > 0
                        ? $"No se encontró DocumentoVenta pendiente para DocuId={docuId}, TipoCodigo={tipoCodigo} ({descripcion})."
                        : $"No se encontró DocumentoVenta pendiente para NotaId={notaId}, TipoCodigo={tipoCodigo} ({descripcion}).",
                    accion_bd = "sin_documento_pendiente",
                    cod_sunat = codSunat,
                    msj_sunat = mensajeSunat
                };
            }

            if (estadoResultado == EstadoResultadoSunat.Rechazado && actualizarNotaEnRechazo && notaId > 0)
            {
                await using var cmdNota = new SqlCommand(sqlActualizarNotaRechazo, con, (SqlTransaction)tx);
                cmdNota.Parameters.AddWithValue("@NotaId", notaId);
                await cmdNota.ExecuteNonQueryAsync(cancellationToken);

                await using var cmdDetalle = new SqlCommand(sqlActualizarDetalleRechazo, con, (SqlTransaction)tx);
                cmdDetalle.Parameters.AddWithValue("@NotaId", notaId);
                await cmdDetalle.ExecuteNonQueryAsync(cancellationToken);
            }

            await tx.CommitAsync(cancellationToken);

            return new
            {
                ok = true,
                accion_bd = estadoResultado == EstadoResultadoSunat.Rechazado
                    ? (actualizarNotaEnRechazo && notaId > 0 ? "registrar_rechazo" : "registrar_rechazo_documento")
                    : "mantener_pendiente",
                estado_sunat = estadoSunatObjetivo,
                cod_sunat = codSunat,
                msj_sunat = mensajeSunat,
                mensaje = estadoResultado == EstadoResultadoSunat.Rechazado
                    ? (actualizarNotaEnRechazo && notaId > 0
                        ? "Se registró el documento como RECHAZADO y la nota volvió a estado PENDIENTE."
                        : "Se registró el documento como RECHAZADO.")
                    : "Se registró la respuesta SUNAT/OSE manteniendo el documento en estado PENDIENTE para reintento."
            };
        }
        catch (Exception ex)
        {
            return new
            {
                ok = false,
                mensaje = $"No se pudo registrar el estado no aceptado en BD: {ex.Message}",
                cod_sunat = codSunat,
                msj_sunat = mensajeSunat
            };
        }
    }

    private async Task<object> RegistrarResultadoNoAceptadoResumenAsync(
        EnviarResumenBoletasRequest request,
        Dictionary<string, string>? respuestaLegacy,
        CancellationToken cancellationToken)
    {
        var docuIds = (request.detalle ?? new List<EnviarResumenBoletasDetalleRequest>())
            .Where(x => x is not null && x.docuId.HasValue && x.docuId.Value > 0)
            .Select(x => x.docuId!.Value)
            .Distinct()
            .ToList();

        if (docuIds.Count == 0)
        {
            return new
            {
                ok = false,
                mensaje = "No se pudo registrar estado no aceptado del lote: se requiere detalle.docuId."
            };
        }

        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return new
            {
                ok = false,
                mensaje = "No se encontró la cadena de conexión para registrar estado no aceptado del lote."
            };
        }

        var flgRta = ObtenerValorLegacy(respuestaLegacy, "flg_rta", "0");
        var codSunat = ObtenerValorLegacy(respuestaLegacy, "cod_sunat");
        var mensajeSunat = ObtenerValorLegacy(respuestaLegacy, "msj_sunat");
        var hashCpe = ObtenerValorLegacy(respuestaLegacy, "hash_cpe");
        var estadoResultado = ResolverEstadoResultadoSunat(flgRta, codSunat);
        var estadoSunatObjetivo = estadoResultado == EstadoResultadoSunat.Rechazado ? "RECHAZADO" : "PENDIENTE";
        var docuEstadoObjetivo = estadoResultado == EstadoResultadoSunat.Rechazado ? "RECHAZADO" : null;

        var parametrosDocuIds = docuIds
            .Select((_, index) => $"@DocuId{index}")
            .ToList();

        var sqlActualizarDocumento = $"""
            UPDATE d
            SET d.CodigoSunat = @CodigoSunat,
                d.MensajeSunat = @MensajeSunat,
                d.DocuHash = CASE WHEN NULLIF(@DocuHash, '') IS NULL THEN d.DocuHash ELSE @DocuHash END,
                d.EstadoSunat = @EstadoSunat,
                d.DocuEstado = CASE WHEN NULLIF(@DocuEstado, '') IS NULL THEN d.DocuEstado ELSE @DocuEstado END
            FROM DocumentoVenta d
            WHERE d.DocuId IN ({string.Join(",", parametrosDocuIds)})
              AND d.EstadoSunat IN ('PENDIENTE', 'ENVIADO');

            SELECT @@ROWCOUNT;
            """;

        try
        {
            await using var con = new SqlConnection(connectionString);
            await con.OpenAsync(cancellationToken);

            await using var cmd = new SqlCommand(sqlActualizarDocumento, con);
            cmd.Parameters.AddWithValue("@CodigoSunat", (object?)codSunat ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MensajeSunat", (object?)mensajeSunat ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DocuHash", (object?)hashCpe ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EstadoSunat", estadoSunatObjetivo);
            cmd.Parameters.AddWithValue("@DocuEstado", (object?)docuEstadoObjetivo ?? DBNull.Value);

            for (var i = 0; i < docuIds.Count; i++)
            {
                cmd.Parameters.AddWithValue(parametrosDocuIds[i], docuIds[i]);
            }

            var filasAfectadas = Convert.ToInt32(await cmd.ExecuteScalarAsync(cancellationToken) ?? 0, CultureInfo.InvariantCulture);
            if (filasAfectadas <= 0)
            {
                return new
                {
                    ok = false,
                    accion_bd = "sin_documentos_pendientes",
                    mensaje = "No se encontraron documentos del lote en estado PENDIENTE/ENVIADO para registrar la respuesta SUNAT/OSE.",
                    cod_sunat = codSunat,
                    msj_sunat = mensajeSunat
                };
            }

            return new
            {
                ok = true,
                accion_bd = estadoResultado == EstadoResultadoSunat.Rechazado
                    ? "registrar_rechazo_lote"
                    : "mantener_pendiente_lote",
                documentos_actualizados = filasAfectadas,
                estado_sunat = estadoSunatObjetivo,
                cod_sunat = codSunat,
                msj_sunat = mensajeSunat,
                mensaje = estadoResultado == EstadoResultadoSunat.Rechazado
                    ? "Se registró el lote como RECHAZADO en DocumentoVenta."
                    : "Se registró la respuesta del lote manteniendo DocumentoVenta en PENDIENTE para reintento."
            };
        }
        catch (Exception ex)
        {
            return new
            {
                ok = false,
                mensaje = $"No se pudo registrar estado no aceptado del lote en BD: {ex.Message}",
                cod_sunat = codSunat,
                msj_sunat = mensajeSunat
            };
        }
    }

    private async Task<object> RegistrarNotaCreditoEnBaseDatosAsync(
        EnviarFacturaRequest request,
        Dictionary<string, string>? respuestaLegacy,
        CancellationToken cancellationToken)
    {
        var codSunat = ObtenerValorLegacy(respuestaLegacy, "cod_sunat");
        if (!EsCodigoFacturaAceptado(codSunat))
        {
            return new
            {
                ok = false,
                mensaje = "No se actualizó BD porque SUNAT/OCE no devolvió aceptación de la nota de crédito."
            };
        }

        var mensajeSunat = ObtenerValorLegacy(respuestaLegacy, "msj_sunat");
        var hashCpe = ObtenerValorLegacy(respuestaLegacy, "hash_cpe");

        if (!string.IsNullOrWhiteSpace(request.LISTA_ORDEN_NC))
        {
            try
            {
                var listaOrdenNormalizada = ForzarDocuConceptoListaOrdenNotaCredito(request.LISTA_ORDEN_NC.Trim());
                var resultado = await _mediator.RegistrarNotaCreditoAsync(listaOrdenNormalizada, cancellationToken);
                var ok = string.Equals(resultado?.Trim(), "true", StringComparison.OrdinalIgnoreCase);
                var estadoActualizado = ok && await MarcarDocumentoModificadoComoAnuladoAsync(request, cancellationToken);
                return new
                {
                    ok,
                    accion_bd = "uspinsertarNC",
                    estado_documento_modificado = estadoActualizado,
                    resultado = string.IsNullOrWhiteSpace(resultado) ? string.Empty : resultado,
                    mensaje = ok
                        ? estadoActualizado
                            ? "SUNAT/OCE aceptó la nota de crédito, se ejecutó uspinsertarNC y el documento modificado quedó ANULADO."
                            : "SUNAT/OCE aceptó la nota de crédito y se ejecutó uspinsertarNC."
                        : "SUNAT/OCE aceptó la nota de crédito, pero uspinsertarNC no devolvió 'true'."
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    ok = false,
                    accion_bd = "uspinsertarNC",
                    mensaje = $"SUNAT/OCE aceptó la nota de crédito, pero falló el registro en BD: {ex.Message}"
                };
            }
        }

        if (request.DOCU_ID.HasValue && request.DOCU_ID.Value > 0)
        {
            try
            {
                var listaOrdenAuto = await ConstruirListaOrdenNotaCreditoDesdeBdAsync(
                    request,
                    codSunat,
                    mensajeSunat,
                    hashCpe,
                    cancellationToken);

                if (!string.IsNullOrWhiteSpace(listaOrdenAuto))
                {
                    var resultado = await _mediator.RegistrarNotaCreditoAsync(listaOrdenAuto, cancellationToken);
                    var ok = string.Equals(resultado?.Trim(), "true", StringComparison.OrdinalIgnoreCase);
                    var estadoActualizado = ok && await MarcarDocumentoModificadoComoAnuladoAsync(request, cancellationToken);
                    return new
                    {
                        ok,
                        accion_bd = "uspinsertarNC(auto)",
                        estado_documento_modificado = estadoActualizado,
                        resultado = string.IsNullOrWhiteSpace(resultado) ? string.Empty : resultado,
                        mensaje = ok
                            ? estadoActualizado
                                ? "SUNAT/OCE aceptó la nota de crédito, se ejecutó uspinsertarNC (LISTA_ORDEN_NC armado en backend) y el documento modificado quedó ANULADO."
                                : "SUNAT/OCE aceptó la nota de crédito y se ejecutó uspinsertarNC (LISTA_ORDEN_NC armado en backend)."
                            : "SUNAT/OCE aceptó la nota de crédito, se intentó uspinsertarNC auto, pero no devolvió 'true'."
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "No se pudo armar/registrar LISTA_ORDEN_NC automáticamente para DOCU_ID={DocuId}. Se intentará flujo de reenvío.",
                    request.DOCU_ID.Value);
            }
        }

        if (request.DOCU_ID.HasValue && request.DOCU_ID.Value > 0)
        {
            var data = string.Join("|", new[]
            {
                request.DOCU_ID.Value.ToString(CultureInfo.InvariantCulture),
                SanitizarCampoListaOrden(codSunat),
                SanitizarCampoListaOrden(mensajeSunat)
            });

            try
            {
                var resultado = await _mediator.ReenviarNotaCreditoAsync(data, cancellationToken);
                var ok = string.Equals(resultado?.Trim(), "true", StringComparison.OrdinalIgnoreCase);
                var estadoActualizado = ok && await MarcarDocumentoModificadoComoAnuladoAsync(request, cancellationToken);
                return new
                {
                    ok,
                    accion_bd = "uspReEnviarNotaCredito",
                    estado_documento_modificado = estadoActualizado,
                    resultado = string.IsNullOrWhiteSpace(resultado) ? string.Empty : resultado,
                    mensaje = ok
                        ? estadoActualizado
                            ? "SUNAT/OCE aceptó la nota de crédito, se ejecutó uspReEnviarNotaCredito y el documento modificado quedó ANULADO."
                            : "SUNAT/OCE aceptó la nota de crédito y se ejecutó uspReEnviarNotaCredito."
                        : "SUNAT/OCE aceptó la nota de crédito, pero uspReEnviarNotaCredito no devolvió 'true'."
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    ok = false,
                    accion_bd = "uspReEnviarNotaCredito",
                    mensaje = $"SUNAT/OCE aceptó la nota de crédito, pero falló la actualización en BD: {ex.Message}"
                };
            }
        }

        return new
        {
            ok = false,
            mensaje = "SUNAT/OCE aceptó la nota de crédito, pero no se registró en BD porque falta LISTA_ORDEN_NC o DOCU_ID."
        };
    }

    private async Task<bool> MarcarDocumentoModificadoComoAnuladoAsync(
        EnviarFacturaRequest request,
        CancellationToken cancellationToken)
    {
        if (!string.Equals((request.COD_TIPO_MOTIVO ?? string.Empty).Trim(), "01", StringComparison.Ordinal))
        {
            return false;
        }

        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return false;
        }

        var (serieRef, numeroRef) = SepararSerieNumeroComprobante(request.NRO_DOCUMENTO_MODIFICA);
        if ((!request.DOCU_ID.HasValue || request.DOCU_ID.Value <= 0) &&
            (string.IsNullOrWhiteSpace(serieRef) || string.IsNullOrWhiteSpace(numeroRef)))
        {
            return false;
        }

        const string sql = """
            DECLARE @DocuIdAfectado BIGINT;

            SELECT TOP (1)
                @DocuIdAfectado = d.DocuId
            FROM DocumentoVenta d
            WHERE d.TipoCodigo IN ('01', '03')
              AND (
                    (@SerieRef <> '' AND @NumeroRef <> '' AND LTRIM(RTRIM(d.DocuSerie)) = @SerieRef AND LTRIM(RTRIM(d.DocuNumero)) = @NumeroRef)
                    OR ((@SerieRef = '' OR @NumeroRef = '') AND @DocuId > 0 AND d.DocuId = @DocuId)
                  )
            ORDER BY CASE
                        WHEN @SerieRef <> '' AND @NumeroRef <> '' AND LTRIM(RTRIM(d.DocuSerie)) = @SerieRef AND LTRIM(RTRIM(d.DocuNumero)) = @NumeroRef THEN 0
                        WHEN @DocuId > 0 AND d.DocuId = @DocuId THEN 1
                        ELSE 2
                     END,
                     d.DocuId DESC;

            IF @DocuIdAfectado IS NOT NULL
            BEGIN
                UPDATE DocumentoVenta
                SET DocuEstado = 'ANULADO'
                WHERE DocuId = @DocuIdAfectado
                  AND ISNULL(LTRIM(RTRIM(DocuEstado)), '') <> 'ANULADO';

                UPDATE n
                SET NotaEstado = 'ANULADO'
                FROM NotaPedido n
                INNER JOIN DocumentoVenta d ON d.NotaId = n.NotaId
                WHERE d.DocuId = @DocuIdAfectado
                  AND ISNULL(LTRIM(RTRIM(n.NotaEstado)), '') <> 'ANULADO';
            END

            SELECT ISNULL(@DocuIdAfectado, 0);
            """;

        try
        {
            await using var con = new SqlConnection(connectionString);
            await using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@DocuId", request.DOCU_ID ?? 0L);
            cmd.Parameters.AddWithValue("@SerieRef", serieRef);
            cmd.Parameters.AddWithValue("@NumeroRef", numeroRef);

            await con.OpenAsync(cancellationToken);
            var value = await cmd.ExecuteScalarAsync(cancellationToken);
            var docuIdAfectado = value == null || value == DBNull.Value ? 0L : Convert.ToInt64(value, CultureInfo.InvariantCulture);
            return docuIdAfectado > 0;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "No se pudo marcar como ANULADO el documento modificado de la NC. DOCU_ID={DocuId}, NRO_DOCUMENTO_MODIFICA={NroDocumentoModifica}",
                request.DOCU_ID,
                request.NRO_DOCUMENTO_MODIFICA);
            return false;
        }
    }

    private async Task<object> EjecutarEnvioNotaCreditoAsync(
        EnviarFacturaRequest requestNotaCredito,
        int tipoProceso,
        CancellationToken cancellationToken)
    {
        var rutaPfxNormalizada = ResolverRutaPfx(requestNotaCredito.RUTA_PFX ?? string.Empty);
        var notaCredito = MapearNotaCreditoLegacy(requestNotaCredito, tipoProceso, rutaPfxNormalizada);
        var respuestaLegacy = _cpeGateway.Envio(notaCredito);

        var flgRta = ObtenerValorLegacy(respuestaLegacy, "flg_rta", "0");
        var codSunat = ObtenerValorLegacy(respuestaLegacy, "cod_sunat");
        var aceptadoSunat = string.Equals(flgRta, "1", StringComparison.Ordinal) && EsCodigoFacturaAceptado(codSunat);

        object? registroBd;
        if (aceptadoSunat)
        {
            registroBd = await RegistrarNotaCreditoEnBaseDatosAsync(requestNotaCredito, respuestaLegacy, cancellationToken);
        }
        else if (string.Equals(flgRta, "1", StringComparison.Ordinal))
        {
            registroBd = await RegistrarResultadoNoAceptadoDocumentoVentaAsync(
                requestNotaCredito,
                tipoCodigo: "07",
                respuestaLegacy,
                cancellationToken,
                actualizarNotaEnRechazo: false,
                descripcionDocumento: "nota de crédito");
        }
        else
        {
            registroBd = await RegistrarResultadoNoAceptadoDocumentoVentaAsync(
                requestNotaCredito,
                tipoCodigo: "07",
                respuestaLegacy,
                cancellationToken,
                actualizarNotaEnRechazo: false,
                descripcionDocumento: "nota de crédito");
        }

        return NormalizarRespuestaFactura(respuestaLegacy, registroBd: registroBd);
    }

    private async Task<object> EjecutarEnvioFacturaAsync(
        EnviarFacturaRequest requestFactura,
        int tipoProceso,
        CancellationToken cancellationToken)
    {
        var rutaPfxNormalizada = ResolverRutaPfx(requestFactura.RUTA_PFX ?? string.Empty);
        var factura = MapearFacturaLegacy(requestFactura, tipoProceso, rutaPfxNormalizada);
        var respuestaLegacy = _cpeGateway.Envio(factura);

        var flgRta = ObtenerValorLegacy(respuestaLegacy, "flg_rta", "0");
        var codSunat = ObtenerValorLegacy(respuestaLegacy, "cod_sunat");
        var aceptadoSunat = string.Equals(flgRta, "1", StringComparison.Ordinal) && EsCodigoFacturaAceptado(codSunat);

        object? registroBd;
        if (aceptadoSunat)
        {
            registroBd = await RegistrarFacturaEnBaseDatosAsync(requestFactura, respuestaLegacy, cancellationToken);
        }
        else if (string.Equals(flgRta, "1", StringComparison.Ordinal))
        {
            registroBd = await RegistrarResultadoNoAceptadoDocumentoVentaAsync(
                requestFactura,
                tipoCodigo: "01",
                respuestaLegacy,
                cancellationToken);
        }
        else
        {
            registroBd = await RegistrarResultadoNoAceptadoDocumentoVentaAsync(
                requestFactura,
                tipoCodigo: "01",
                respuestaLegacy,
                cancellationToken);
        }

        return NormalizarRespuestaFactura(respuestaLegacy, registroBd: registroBd);
    }

    private async Task<object> EjecutarEnvioBoletaAsync(
        EnviarFacturaRequest requestBoleta,
        int tipoProceso,
        CancellationToken cancellationToken)
    {
        var rutaPfxNormalizada = ResolverRutaPfx(requestBoleta.RUTA_PFX ?? string.Empty);
        var boleta = MapearBoletaLegacy(requestBoleta, tipoProceso, rutaPfxNormalizada);
        var respuestaLegacy = _cpeGateway.Envio(boleta);

        var flgRta = ObtenerValorLegacy(respuestaLegacy, "flg_rta", "0");
        var codSunat = ObtenerValorLegacy(respuestaLegacy, "cod_sunat");
        var aceptadoSunat = string.Equals(flgRta, "1", StringComparison.Ordinal) && EsCodigoFacturaAceptado(codSunat);

        object? registroBd;
        if (aceptadoSunat)
        {
            registroBd = await RegistrarBoletaEnBaseDatosAsync(requestBoleta, respuestaLegacy, cancellationToken);
        }
        else if (string.Equals(flgRta, "1", StringComparison.Ordinal))
        {
            registroBd = await RegistrarResultadoNoAceptadoDocumentoVentaAsync(
                requestBoleta,
                tipoCodigo: "03",
                respuestaLegacy,
                cancellationToken);
        }
        else
        {
            registroBd = await RegistrarResultadoNoAceptadoDocumentoVentaAsync(
                requestBoleta,
                tipoCodigo: "03",
                respuestaLegacy,
                cancellationToken);
        }

        return NormalizarRespuestaFactura(respuestaLegacy, registroBd: registroBd);
    }

    private async Task<object> IntentarEmitirFacturaDesdeOrdenAsync(
        NotaPedido nota,
        IReadOnlyList<DetalleNota> detalles,
        string? resultadoRegistro,
        CancellationToken cancellationToken)
    {
        if (!EsFactura(nota.NotaDocu))
        {
            return CrearRespuestaFacturaPendiente("La orden registrada no corresponde a una FACTURA.");
        }

        try
        {
            var notaId = ExtraerNotaIdDeRegistro(resultadoRegistro) ?? (nota.NotaId > 0 ? nota.NotaId : null);
            var numeroComprobante = ResolverNumeroComprobanteDesdeRegistro(resultadoRegistro, nota.NotaNumero);
            if (!notaId.HasValue || notaId.Value <= 0)
            {
                _logger.LogWarning("No se pudo determinar NotaId para emitir la factura automaticamente. Resultado registro: {Resultado}", resultadoRegistro);
                return CrearRespuestaFacturaPendiente("La orden se registró, pero no se pudo determinar el NotaId para emitir la factura.");
            }

            var requestFactura = await ConstruirRequestFacturaDesdeOrdenAsync(nota, detalles, notaId.Value, numeroComprobante, cancellationToken);
            if (requestFactura is null)
            {
                return CrearRespuestaFacturaPendiente("La orden se registró, pero no se pudo completar la informacion necesaria para emitir la factura.");
            }

            requestFactura = NormalizarRequestFactura(requestFactura);
            var errores = ValidarRequestFactura(requestFactura);
            if (errores.Count > 0)
            {
                _logger.LogWarning(
                    "La orden FACTURA {NotaId} se registro pero no se emitio automaticamente por datos faltantes: {Errores}",
                    notaId.Value,
                    string.Join(" | ", errores));
                return CrearRespuestaFacturaPendiente("La orden se registró, pero faltan datos para emitir la factura: " + string.Join(" | ", errores));
            }

            var tipoProceso = ParseTipoProceso(requestFactura.TIPO_PROCESO);
            if (!tipoProceso.HasValue || tipoProceso.Value < 1 || tipoProceso.Value > 3)
            {
                _logger.LogWarning("La orden FACTURA {NotaId} se registro pero no se emitio automaticamente por TIPO_PROCESO invalido.", notaId.Value);
                return CrearRespuestaFacturaPendiente("La orden se registró, pero el TIPO_PROCESO configurado es inválido para emitir la factura.");
            }

            var respuesta = await EjecutarEnvioFacturaAsync(requestFactura, tipoProceso.Value, cancellationToken);
            var respuestaJson = JsonSerializer.Serialize(respuesta);
            _logger.LogInformation("Resultado de emision automatica de FACTURA para NotaId {NotaId}: {Respuesta}", notaId.Value, respuestaJson);
            return respuesta;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "La orden FACTURA se registro, pero la emision automatica fallo y quedo pendiente para reintento.");
            return CrearRespuestaFacturaPendiente("La orden se registró, pero la emisión automática al OCE/SUNAT falló: " + ex.Message);
        }
    }

    private async Task<EnviarFacturaRequest?> ConstruirRequestFacturaDesdeOrdenAsync(
        NotaPedido nota,
        IReadOnlyList<DetalleNota> detalles,
        long notaId,
        string numeroComprobante,
        CancellationToken cancellationToken)
    {
        if (!nota.CompaniaId.HasValue || nota.CompaniaId.Value <= 0)
        {
            _logger.LogWarning("La orden FACTURA {NotaId} no tiene CompaniaId valido para emitir.", notaId);
            return null;
        }

        if (!nota.ClienteId.HasValue || nota.ClienteId.Value <= 0)
        {
            _logger.LogWarning("La orden FACTURA {NotaId} no tiene ClienteId valido para emitir.", notaId);
            return null;
        }

        var companias = await _companias.ListarAsync(page: 1, pageSize: 1000, cancellationToken: cancellationToken);
        var compania = companias.FirstOrDefault(x => x.CompaniaId == nota.CompaniaId.Value);
        if (compania is null)
        {
            _logger.LogWarning("No se encontro la compania {CompaniaId} para emitir la FACTURA {NotaId}.", nota.CompaniaId.Value, notaId);
            return null;
        }

        var clientes = await _clientes.ListarAsync(estado: null, page: 1, pageSize: 10000, cancellationToken: cancellationToken);
        var cliente = clientes.FirstOrDefault(x => x.ClienteId == nota.ClienteId.Value);
        if (cliente is null)
        {
            _logger.LogWarning("No se encontro el cliente {ClienteId} para emitir la FACTURA {NotaId}.", nota.ClienteId.Value, notaId);
            return null;
        }

        var credenciales = await _mediator.ObtenerCredencialesSunatAsync(nota.CompaniaId.Value, cancellationToken);
        if (credenciales is null)
        {
            _logger.LogWarning("No hay credenciales SUNAT configuradas para la compania {CompaniaId}. La FACTURA {NotaId} quedo pendiente.", nota.CompaniaId.Value, notaId);
            return null;
        }

        var tipoProceso = ResolverTipoProcesoDesdeCredenciales(credenciales);
        var ubigeoEmpresa = await ObtenerUbigeoAsync(compania.CompaniaCodigoUBG, cancellationToken);
        var ubigeoCliente = ubigeoEmpresa;

        var lineas = await _lineas.ListarAsync(page: 1, pageSize: 5000, cancellationToken: cancellationToken);
        var lineasPorId = lineas
            .Where(x => !string.IsNullOrWhiteSpace(x.Id))
            .Select(x => new { Ok = int.TryParse(x.Id, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id), Id = x.Id, Item = x })
            .Where(x => x.Ok)
            .ToDictionary(x => int.Parse(x.Id!, CultureInfo.InvariantCulture), x => x.Item);

        var totalIcbper = nota.ICBPER ?? 0m;
        var totalDocumento = nota.NotaTotal ?? (detalles.Sum(x => x.DetalleImporte ?? 0m) + totalIcbper);

        var request = new EnviarFacturaRequest
        {
            NOTA_ID = notaId,
            TIPO_OPERACION = "0101",
            HORA_REGISTRO = NormalizarHoraRegistro(ResolverHoraRegistroDesdeNota(nota.NotaFechaPago, nota.NotaFecha)),
            SUB_TOTAL = 0m,
            TOTAL = totalDocumento,
            TOTAL_IGV = 0m,
            TOTAL_ISC = 0m,
            TOTAL_ICBPER = totalIcbper,
            TOTAL_OTR_IMP = 0m,
            TOTAL_EXPORTACION = 0m,
            TOTAL_DESCUENTO = nota.NotaDescuento ?? 0m,
            TOTAL_GRATUITAS = 0m,
            TOTAL_GRAVADAS = 0m,
            TOTAL_EXONERADAS = 0m,
            TOTAL_INAFECTA = 0m,
            POR_IGV = PorcentajeIgvDefault,
            TOTAL_LETRAS = Letras.enletras(totalDocumento.ToString("N2", CultureInfo.InvariantCulture)) + " SOLES",
            NRO_COMPROBANTE = $"{(nota.NotaSerie ?? string.Empty).Trim()}-{numeroComprobante}".Trim('-'),
            FECHA_DOCUMENTO = FormatearFechaIso(nota.NotaFecha),
            FECHA_VTO = FormatearFechaIso(nota.NotaFechaPago ?? nota.NotaFecha),
            COD_TIPO_DOCUMENTO = "01",
            COD_MONEDA = "PEN",
            NRO_DOCUMENTO_CLIENTE = ObtenerDocumentoCliente(cliente),
            RAZON_SOCIAL_CLIENTE = cliente.ClienteRazon?.Trim(),
            TIPO_DOCUMENTO_CLIENTE = InferirTipoDocumentoCliente(null, ObtenerDocumentoCliente(cliente)),
            DIRECCION_CLIENTE = string.IsNullOrWhiteSpace(cliente.ClienteDireccion) ? "-" : cliente.ClienteDireccion.Trim(),
            CIUDAD_CLIENTE = ubigeoCliente?.Provincia ?? compania.CompaniaNomUBG?.Trim() ?? compania.CompaniaDistrito?.Trim(),
            COD_PAIS_CLIENTE = "PE",
            COD_UBIGEO_CLIENTE = compania.CompaniaCodigoUBG?.Trim(),
            DEPARTAMENTO_CLIENTE = ubigeoCliente?.Departamento ?? compania.CompaniaNomUBG?.Trim() ?? compania.CompaniaDistrito?.Trim(),
            PROVINCIA_CLIENTE = ubigeoCliente?.Provincia ?? compania.CompaniaNomUBG?.Trim() ?? compania.CompaniaDistrito?.Trim(),
            DISTRITO_CLIENTE = ubigeoCliente?.Distrito ?? compania.CompaniaDistrito?.Trim() ?? compania.CompaniaNomUBG?.Trim(),
            NRO_DOCUMENTO_EMPRESA = compania.CompaniaRUC?.Trim(),
            TIPO_DOCUMENTO_EMPRESA = "6",
            NOMBRE_COMERCIAL_EMPRESA = string.IsNullOrWhiteSpace(compania.CompaniaComercial) ? compania.CompaniaRazonSocial?.Trim() : compania.CompaniaComercial.Trim(),
            CODIGO_UBIGEO_EMPRESA = compania.CompaniaCodigoUBG?.Trim(),
            DIRECCION_EMPRESA = string.IsNullOrWhiteSpace(compania.CompaniaDirecSunat) ? compania.CompaniaDireccion?.Trim() : compania.CompaniaDirecSunat.Trim(),
            CONTACTO_EMPRESA = compania.CompaniaTelefono?.Trim() ?? string.Empty,
            DEPARTAMENTO_EMPRESA = ubigeoEmpresa?.Departamento ?? compania.CompaniaNomUBG?.Trim() ?? compania.CompaniaDistrito?.Trim(),
            PROVINCIA_EMPRESA = ubigeoEmpresa?.Provincia ?? compania.CompaniaNomUBG?.Trim() ?? compania.CompaniaDistrito?.Trim(),
            DISTRITO_EMPRESA = ubigeoEmpresa?.Distrito ?? compania.CompaniaDistrito?.Trim() ?? compania.CompaniaNomUBG?.Trim(),
            CODIGO_PAIS_EMPRESA = "PE",
            RAZON_SOCIAL_EMPRESA = compania.CompaniaRazonSocial?.Trim(),
            USUARIO_SOL_EMPRESA = credenciales.UsuarioSOL?.Trim(),
            PASS_SOL_EMPRESA = credenciales.ClaveSOL?.Trim(),
            CONTRA_FIRMA = credenciales.ClaveCertificado?.Trim(),
            TIPO_PROCESO = JsonSerializer.SerializeToElement(tipoProceso),
            FORMA_PAGO = ResolverFormaPagoFactura(nota),
            GLOSA = nota.NotaConcepto?.Trim() ?? string.Empty,
            RUTA_PFX = credenciales.CertificadoPFX?.Trim(),
            CODIGO_ANEXO = "0000"
        };

        var detalleFactura = new List<EnviarFacturaDetalleRequest>();
        var indice = 1;

        foreach (var detalle in detalles)
        {
            if (!detalle.IdProducto.HasValue || detalle.IdProducto.Value <= 0)
            {
                _logger.LogWarning("La FACTURA {NotaId} tiene un detalle sin IdProducto. Se omitira en la emision automatica.", notaId);
                indice++;
                continue;
            }

            var producto = await _productos.ObtenerPorIdAsync(detalle.IdProducto.Value, cancellationToken);
            var codigoSunat = string.Empty;
            if (producto?.IdSubLinea.HasValue == true &&
                lineasPorId.TryGetValue(Convert.ToInt32(producto.IdSubLinea.Value, CultureInfo.InvariantCulture), out var linea))
            {
                codigoSunat = linea.CodigoSunat?.Trim() ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(codigoSunat))
            {
                codigoSunat = CodigoSunatFacturaFallback;
                _logger.LogWarning(
                    "El producto {ProductoId} no tiene CodigoSunat configurado. Se usará el fallback temporal {CodigoSunat} para la FACTURA {NotaId}.",
                    detalle.IdProducto.Value,
                    codigoSunat,
                    notaId);
            }

            var cantidad = detalle.DetalleCantidad ?? 0m;
            var importeBruto = detalle.DetalleImporte ?? 0m;
            var precioOriginal = detalle.DetallePrecio ?? (cantidad > 0 ? (importeBruto / cantidad) : 0m);

            detalleFactura.Add(new EnviarFacturaDetalleRequest
            {
                item = indice,
                unidadMedida = NormalizarUnidadMedidaSunat(detalle.DetalleUm, producto?.ProductoUM),
                cantidad = cantidad,
                precio = precioOriginal,
                importe = importeBruto,
                precioSinImpuesto = 0m,
                igv = 0m,
                codTipoOperacion = "10",
                codigo = string.IsNullOrWhiteSpace(producto?.ProductoCodigo)
                    ? $"PROD{detalle.IdProducto.Value}"
                    : producto.ProductoCodigo.Trim(),
                codigoSunat = codigoSunat,
                descripcion = string.IsNullOrWhiteSpace(detalle.DetalleDescripcion)
                    ? producto?.ProductoNombre?.Trim()
                    : detalle.DetalleDescripcion.Trim(),
                descuento = 0m,
                subTotal = 0m
            });

            indice++;
        }

        request.detalle = detalleFactura;
        AplicarCalculoTributarioParaEnvio(request);
        request.TOTAL_LETRAS = Letras.enletras((request.TOTAL ?? 0m).ToString("N2", CultureInfo.InvariantCulture)) + " SOLES";
        return request;
    }

    private async Task<object> IntentarEmitirBoletaDesdeOrdenAsync(
        NotaPedido nota,
        IReadOnlyList<DetalleNota> detalles,
        string? resultadoRegistro,
        CancellationToken cancellationToken)
    {
        if (!EsBoleta(nota.NotaDocu))
        {
            return CrearRespuestaFacturaPendiente("La orden registrada no corresponde a una BOLETA.");
        }

        try
        {
            var notaId = ExtraerNotaIdDeRegistro(resultadoRegistro) ?? (nota.NotaId > 0 ? nota.NotaId : null);
            var numeroComprobante = ResolverNumeroComprobanteDesdeRegistro(resultadoRegistro, nota.NotaNumero);
            if (!notaId.HasValue || notaId.Value <= 0)
            {
                _logger.LogWarning("No se pudo determinar NotaId para emitir la boleta automaticamente. Resultado registro: {Resultado}", resultadoRegistro);
                return CrearRespuestaFacturaPendiente("La orden se registró, pero no se pudo determinar el NotaId para emitir la boleta.");
            }

            var requestBoleta = await ConstruirRequestBoletaDesdeOrdenAsync(nota, detalles, notaId.Value, numeroComprobante, cancellationToken);
            if (requestBoleta is null)
            {
                return CrearRespuestaFacturaPendiente("La orden se registró, pero no se pudo completar la informacion necesaria para emitir la boleta.");
            }

            requestBoleta = NormalizarRequestBoleta(requestBoleta);
            var errores = ValidarRequestBoleta(requestBoleta);
            if (errores.Count > 0)
            {
                _logger.LogWarning(
                    "La orden BOLETA {NotaId} se registro pero no se emitio automaticamente por datos faltantes: {Errores}",
                    notaId.Value,
                    string.Join(" | ", errores));
                return CrearRespuestaFacturaPendiente("La orden se registró, pero faltan datos para emitir la boleta: " + string.Join(" | ", errores));
            }

            var tipoProceso = ParseTipoProceso(requestBoleta.TIPO_PROCESO);
            if (!tipoProceso.HasValue || tipoProceso.Value < 1 || tipoProceso.Value > 3)
            {
                _logger.LogWarning("La orden BOLETA {NotaId} se registro pero no se emitio automaticamente por TIPO_PROCESO invalido.", notaId.Value);
                return CrearRespuestaFacturaPendiente("La orden se registró, pero el TIPO_PROCESO configurado es inválido para emitir la boleta.");
            }

            var respuesta = await EjecutarEnvioBoletaAsync(requestBoleta, tipoProceso.Value, cancellationToken);
            var respuestaJson = JsonSerializer.Serialize(respuesta);
            _logger.LogInformation("Resultado de emision automatica de BOLETA para NotaId {NotaId}: {Respuesta}", notaId.Value, respuestaJson);
            return respuesta;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "La orden BOLETA se registro, pero la emision automatica fallo y quedo pendiente para reintento.");
            return CrearRespuestaFacturaPendiente("La orden se registró, pero la emisión automática al OCE/SUNAT falló: " + ex.Message);
        }
    }

    private async Task<EnviarFacturaRequest?> ConstruirRequestBoletaDesdeOrdenAsync(
        NotaPedido nota,
        IReadOnlyList<DetalleNota> detalles,
        long notaId,
        string numeroComprobante,
        CancellationToken cancellationToken)
    {
        var request = await ConstruirRequestFacturaDesdeOrdenAsync(nota, detalles, notaId, numeroComprobante, cancellationToken);
        if (request is null)
        {
            return null;
        }

        request.COD_TIPO_DOCUMENTO = "03";
        request.detalle ??= new List<EnviarFacturaDetalleRequest>();
        foreach (var item in request.detalle)
        {
            if (string.IsNullOrWhiteSpace(item.codigoSunat))
            {
                item.codigoSunat = CodigoSunatBoletaFallback;
            }
        }

        if (string.IsNullOrWhiteSpace(request.NRO_DOCUMENTO_CLIENTE))
        {
            request.NRO_DOCUMENTO_CLIENTE = "00000000";
        }

        request.TIPO_DOCUMENTO_CLIENTE = InferirTipoDocumentoCliente(request.TIPO_DOCUMENTO_CLIENTE, request.NRO_DOCUMENTO_CLIENTE);
        if (string.IsNullOrWhiteSpace(request.TIPO_DOCUMENTO_CLIENTE))
        {
            request.TIPO_DOCUMENTO_CLIENTE = "1";
        }

        if (string.IsNullOrWhiteSpace(request.RAZON_SOCIAL_CLIENTE))
        {
            request.RAZON_SOCIAL_CLIENTE = "VARIOS";
        }

        return request;
    }

    private async Task<(EnviarFacturaRequest? Request, int StatusCode, string Mensaje)> ConstruirRequestAnulacionBoletaIndividualAsync(
        AnularBoletaIndividualRequest payload,
        CancellationToken cancellationToken)
    {
        var requestBusqueda = new EnviarFacturaRequest
        {
            DOCU_ID = payload.DOCU_ID,
            NRO_DOCUMENTO_MODIFICA = payload.NRO_DOCUMENTO_MODIFICA
        };

        var origen = await ObtenerOrigenNotaCreditoDesdeBdAsync(requestBusqueda, cancellationToken);
        if (origen is null)
        {
            return (null, (int)HttpStatusCode.NotFound, "No se encontró la boleta a anular (DOCU_ID/NRO_DOCUMENTO_MODIFICA).");
        }

        if (!string.Equals(origen.TipoCodigo, "03", StringComparison.Ordinal))
        {
            return (null, (int)HttpStatusCode.BadRequest, "El documento encontrado no es una boleta electrónica (TipoCodigo distinto de '03').");
        }

        if (string.Equals(origen.Estado, "ANULADO", StringComparison.OrdinalIgnoreCase))
        {
            return (null, (int)HttpStatusCode.Conflict, "La boleta ya se encuentra ANULADA.");
        }

        if (origen.CompaniaId <= 0)
        {
            return (null, (int)HttpStatusCode.BadRequest, "La boleta no tiene CompaniaId válido.");
        }

        var companias = await _companias.ListarAsync(page: 1, pageSize: 1000, cancellationToken: cancellationToken);
        var compania = companias.FirstOrDefault(x => x.CompaniaId == origen.CompaniaId);
        if (compania is null)
        {
            return (null, (int)HttpStatusCode.NotFound, $"No se encontró la compañía {origen.CompaniaId} para construir la nota de crédito.");
        }

        var credenciales = await _mediator.ObtenerCredencialesSunatAsync(origen.CompaniaId, cancellationToken);
        if (credenciales is null)
        {
            return (null, (int)HttpStatusCode.BadRequest, "La compañía no tiene credenciales SUNAT configuradas.");
        }

        var seriePreferidaNc = ResolverSerieNcParaBoleta(origen.Serie);
        var (serieNc, numeroNc) = await ObtenerSerieNumeroNotaCreditoAsync(origen.CompaniaId, seriePreferidaNc, cancellationToken);
        if (string.IsNullOrWhiteSpace(serieNc) || string.IsNullOrWhiteSpace(numeroNc))
        {
            return (null, (int)HttpStatusCode.BadRequest, "No se pudo generar el correlativo de nota de crédito. Verifique SerieNC en MAQUINAS.");
        }

        DateTime fechaDocumento;
        var fechaDocumentoRaw = payload.FECHA_DOCUMENTO?.Trim();
        if (!string.IsNullOrWhiteSpace(fechaDocumentoRaw))
        {
            if (!DateTime.TryParseExact(
                    fechaDocumentoRaw,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out fechaDocumento))
            {
                return (null, (int)HttpStatusCode.BadRequest, "FECHA_DOCUMENTO debe tener formato yyyy-MM-dd.");
            }
        }
        else
        {
            fechaDocumento = ObtenerAhoraCpe().Date;
        }

        if (origen.Detalles.Count == 0)
        {
            return (null, (int)HttpStatusCode.BadRequest, "La boleta no tiene detalle para construir la nota de crédito.");
        }

        var ubigeoEmpresa = await ObtenerUbigeoAsync(compania.CompaniaCodigoUBG, cancellationToken);
        var ubigeoCliente = ubigeoEmpresa;
        var tipoProceso = ResolverTipoProcesoDesdeCredenciales(credenciales);

        var nroDocumentoCliente = !string.IsNullOrWhiteSpace(origen.ClienteRuc)
            ? origen.ClienteRuc.Trim()
            : !string.IsNullOrWhiteSpace(origen.ClienteDni)
                ? origen.ClienteDni.Trim()
                : "00000000";

        var tipoDocumentoCliente = InferirTipoDocumentoCliente(null, nroDocumentoCliente);
        if (string.IsNullOrWhiteSpace(tipoDocumentoCliente))
        {
            tipoDocumentoCliente = "1";
        }

        var codTipoMotivo = "01";
        var descripcionMotivo = string.IsNullOrWhiteSpace(payload.DESCRIPCION_MOTIVO)
            ? DocuConceptoNotaCreditoDefault
            : payload.DESCRIPCION_MOTIVO.Trim();
        var ticketManual = (payload.TICKET_REFERENCIA ?? string.Empty).Trim();
        var ticketResumenBoleta = !string.IsNullOrWhiteSpace(ticketManual)
            ? ticketManual
            : await ObtenerTicketResumenBoletaAsync(origen, cancellationToken);
        var tipoComprobanteModifica = string.IsNullOrWhiteSpace(ticketResumenBoleta) ? "03" : "12";
        var referenciaModifica = string.IsNullOrWhiteSpace(ticketResumenBoleta)
            ? $"{origen.Serie}-{origen.Numero}".Trim('-')
            : ticketResumenBoleta;

        var detalleNc = new List<EnviarFacturaDetalleRequest>();
        var item = 1;
        foreach (var detalle in origen.Detalles)
        {
            var cantidad = detalle.Cantidad > 0m ? detalle.Cantidad : 1m;
            var importe = detalle.Importe;
            if (importe <= 0m && detalle.Precio > 0m)
            {
                importe = detalle.Precio * cantidad;
            }

            var precio = detalle.Precio > 0m
                ? detalle.Precio
                : (cantidad > 0m ? importe / cantidad : 0m);

            detalleNc.Add(new EnviarFacturaDetalleRequest
            {
                item = item,
                unidadMedida = NormalizarUnidadMedidaSunat(detalle.Um, detalle.Um),
                cantidad = cantidad,
                precio = precio,
                importe = importe,
                precioSinImpuesto = 0m,
                igv = 0m,
                codTipoOperacion = "10",
                codigo = $"PROD{detalle.IdProducto}",
                codigoSunat = string.IsNullOrWhiteSpace(detalle.CodigoSunat)
                    ? CodigoSunatNotaCreditoFallback
                    : detalle.CodigoSunat.Trim(),
                descripcion = string.IsNullOrWhiteSpace(detalle.Descripcion)
                    ? $"ITEM {item}"
                    : detalle.Descripcion.Trim(),
                descuento = 0m,
                subTotal = 0m
            });
            item++;
        }

        var requestNc = new EnviarFacturaRequest
        {
            NOTA_ID = origen.NotaId,
            DOCU_ID = origen.DocuId,
            TIPO_OPERACION = "0101",
            HORA_REGISTRO = ObtenerAhoraCpe().ToString("HH:mm:ss", CultureInfo.InvariantCulture),
            SUB_TOTAL = origen.SubTotal,
            TOTAL = origen.Total,
            TOTAL_IGV = origen.Igv,
            TOTAL_ISC = 0m,
            TOTAL_ICBPER = origen.Icbper,
            TOTAL_OTR_IMP = 0m,
            TOTAL_EXPORTACION = 0m,
            TOTAL_DESCUENTO = origen.Descuento,
            TOTAL_GRATUITAS = 0m,
            TOTAL_GRAVADAS = origen.Gravada,
            TOTAL_EXONERADAS = 0m,
            TOTAL_INAFECTA = 0m,
            POR_IGV = PorcentajeIgvDefault,
            TOTAL_LETRAS = Letras.enletras((origen.Total).ToString("N2", CultureInfo.InvariantCulture)) + " SOLES",
            NRO_COMPROBANTE = $"{serieNc}-{numeroNc}".Trim('-'),
            FECHA_DOCUMENTO = fechaDocumento.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            FECHA_VTO = fechaDocumento.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            COD_TIPO_DOCUMENTO = "07",
            COD_MONEDA = "PEN",
            NRO_DOCUMENTO_CLIENTE = nroDocumentoCliente,
            RAZON_SOCIAL_CLIENTE = string.IsNullOrWhiteSpace(origen.ClienteRazon) ? "VARIOS" : origen.ClienteRazon.Trim(),
            TIPO_DOCUMENTO_CLIENTE = tipoDocumentoCliente,
            DIRECCION_CLIENTE = string.IsNullOrWhiteSpace(origen.DireccionFiscal) ? "-" : origen.DireccionFiscal.Trim(),
            CIUDAD_CLIENTE = ubigeoCliente?.Provincia ?? compania.CompaniaNomUBG?.Trim() ?? compania.CompaniaDistrito?.Trim(),
            COD_PAIS_CLIENTE = "PE",
            COD_UBIGEO_CLIENTE = compania.CompaniaCodigoUBG?.Trim(),
            DEPARTAMENTO_CLIENTE = ubigeoCliente?.Departamento ?? compania.CompaniaNomUBG?.Trim() ?? compania.CompaniaDistrito?.Trim(),
            PROVINCIA_CLIENTE = ubigeoCliente?.Provincia ?? compania.CompaniaNomUBG?.Trim() ?? compania.CompaniaDistrito?.Trim(),
            DISTRITO_CLIENTE = ubigeoCliente?.Distrito ?? compania.CompaniaDistrito?.Trim() ?? compania.CompaniaNomUBG?.Trim(),
            NRO_DOCUMENTO_EMPRESA = compania.CompaniaRUC?.Trim(),
            TIPO_DOCUMENTO_EMPRESA = "6",
            NOMBRE_COMERCIAL_EMPRESA = string.IsNullOrWhiteSpace(compania.CompaniaComercial)
                ? compania.CompaniaRazonSocial?.Trim()
                : compania.CompaniaComercial.Trim(),
            CODIGO_UBIGEO_EMPRESA = compania.CompaniaCodigoUBG?.Trim(),
            DIRECCION_EMPRESA = string.IsNullOrWhiteSpace(compania.CompaniaDirecSunat)
                ? compania.CompaniaDireccion?.Trim()
                : compania.CompaniaDirecSunat.Trim(),
            CONTACTO_EMPRESA = compania.CompaniaTelefono?.Trim() ?? string.Empty,
            DEPARTAMENTO_EMPRESA = ubigeoEmpresa?.Departamento ?? compania.CompaniaNomUBG?.Trim() ?? compania.CompaniaDistrito?.Trim(),
            PROVINCIA_EMPRESA = ubigeoEmpresa?.Provincia ?? compania.CompaniaNomUBG?.Trim() ?? compania.CompaniaDistrito?.Trim(),
            DISTRITO_EMPRESA = ubigeoEmpresa?.Distrito ?? compania.CompaniaDistrito?.Trim() ?? compania.CompaniaNomUBG?.Trim(),
            CODIGO_PAIS_EMPRESA = "PE",
            RAZON_SOCIAL_EMPRESA = compania.CompaniaRazonSocial?.Trim(),
            USUARIO_SOL_EMPRESA = credenciales.UsuarioSOL?.Trim(),
            PASS_SOL_EMPRESA = credenciales.ClaveSOL?.Trim(),
            CONTRA_FIRMA = credenciales.ClaveCertificado?.Trim(),
            TIPO_PROCESO = JsonSerializer.SerializeToElement(tipoProceso),
            FORMA_PAGO = string.IsNullOrWhiteSpace(origen.FormaPago) ? "Contado" : origen.FormaPago.Trim(),
            GLOSA = descripcionMotivo,
            RUTA_PFX = credenciales.CertificadoPFX?.Trim(),
            CODIGO_ANEXO = "0000",
            TIPO_COMPROBANTE_MODIFICA = tipoComprobanteModifica,
            NRO_DOCUMENTO_MODIFICA = referenciaModifica,
            COD_TIPO_MOTIVO = codTipoMotivo,
            DESCRIPCION_MOTIVO = descripcionMotivo,
            detalle = detalleNc
        };

        AplicarCalculoTributarioParaEnvio(requestNc);
        requestNc.TOTAL_LETRAS = Letras.enletras((requestNc.TOTAL ?? 0m).ToString("N2", CultureInfo.InvariantCulture)) + " SOLES";

        return (requestNc, (int)HttpStatusCode.OK, "OK");
    }

    private async Task<(string Serie, string Numero)> ObtenerSerieNumeroNotaCreditoAsync(
        int companiaId,
        string? seriePreferida,
        CancellationToken cancellationToken)
    {
        if (companiaId <= 0)
        {
            return (string.Empty, string.Empty);
        }

        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return (string.Empty, string.Empty);
        }

        const string sql = """
            DECLARE @SerieNC NVARCHAR(10) = NULLIF(LTRIM(RTRIM(@SeriePreferida)), '');

            IF @SerieNC IS NULL
            BEGIN
                SELECT TOP (1)
                    @SerieNC = NULLIF(LTRIM(RTRIM(d.DocuSerie)), '')
                FROM DocumentoVenta d
                WHERE d.CompaniaId = @CompaniaId
                  AND d.TipoCodigo = '07'
                  AND LEFT(ISNULL(d.DocuSerie, ''), 2) = 'BN'
                  AND NULLIF(LTRIM(RTRIM(d.DocuSerie)), '') IS NOT NULL
                ORDER BY d.DocuId DESC;
            END;

            IF @SerieNC IS NULL
            BEGIN
                SELECT TOP (1)
                    @SerieNC = NULLIF(LTRIM(RTRIM(SerieNC)), '')
                FROM MAQUINAS
                ORDER BY IdMaquina DESC;
            END;

            IF @SerieNC IS NULL
            BEGIN
                SELECT TOP (1)
                    @SerieNC = NULLIF(LTRIM(RTRIM(d.DocuSerie)), '')
                FROM DocumentoVenta d
                WHERE d.CompaniaId = @CompaniaId
                  AND d.TipoCodigo = '07'
                  AND NULLIF(LTRIM(RTRIM(d.DocuSerie)), '') IS NOT NULL
                ORDER BY d.DocuId DESC;
            END;

            IF @SerieNC IS NULL
            BEGIN
                SET @SerieNC = 'BN01';
            END;

            IF LEFT(@SerieNC, 2) <> 'BN'
            BEGIN
                SET @SerieNC = 'BN' + CASE WHEN LEN(@SerieNC) > 2 THEN RIGHT(@SerieNC, LEN(@SerieNC) - 2) ELSE '01' END;
            END;

            DECLARE @NumeroNC NVARCHAR(20);
            BEGIN TRY
                SELECT @NumeroNC = NULLIF(
                    LTRIM(RTRIM(dbo.genenerarNroFactura(@SerieNC, @CompaniaId, 'NOTA DE CREDITO'))),
                    ''
                );
            END TRY
            BEGIN CATCH
                SET @NumeroNC = NULL;
            END CATCH;

            IF @NumeroNC IS NULL
            BEGIN
                SELECT @NumeroNC = RIGHT(
                    '00000000' + CONVERT(
                        VARCHAR(20),
                        ISNULL(MAX(TRY_CONVERT(INT, RIGHT(d.DocuNumero, 8))), 0) + 1
                    ),
                    8
                )
                FROM DocumentoVenta d
                WHERE d.CompaniaId = @CompaniaId
                  AND d.DocuDocumento = 'NOTA DE CREDITO'
                  AND d.DocuSerie = @SerieNC;
            END;

            SELECT
                @SerieNC AS SerieNC,
                @NumeroNC AS NumeroNC;
            """;

        await using var con = new SqlConnection(connectionString);
        await using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@CompaniaId", companiaId);
        cmd.Parameters.AddWithValue("@SeriePreferida", (object?)seriePreferida ?? DBNull.Value);

        await con.OpenAsync(cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return (string.Empty, string.Empty);
        }

        var serie = reader["SerieNC"]?.ToString()?.Trim() ?? string.Empty;
        var numero = reader["NumeroNC"]?.ToString()?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(numero))
        {
            numero = "00000001";
        }
        return (serie, numero);
    }

    private static string ResolverSerieNcParaBoleta(string? serieBoleta)
    {
        var serieLimpia = (serieBoleta ?? string.Empty).Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(serieLimpia))
        {
            return "BN01";
        }

        if (serieLimpia.Length >= 4 && serieLimpia.StartsWith("BA", StringComparison.Ordinal))
        {
            return $"BN{serieLimpia.Substring(2)}";
        }

        if (serieLimpia.StartsWith("B", StringComparison.Ordinal) && serieLimpia.Length >= 2)
        {
            return $"BN{serieLimpia.Substring(1)}";
        }

        if (serieLimpia.StartsWith("BN", StringComparison.Ordinal))
        {
            return serieLimpia;
        }

        return "BN01";
    }

    private async Task<string> ObtenerTicketResumenBoletaAsync(
        NotaCreditoOrigenBd origen,
        CancellationToken cancellationToken)
    {
        if (origen.CompaniaId <= 0 ||
            string.IsNullOrWhiteSpace(origen.Serie) ||
            string.IsNullOrWhiteSpace(origen.Numero))
        {
            return string.Empty;
        }

        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return string.Empty;
        }

        var comprobante = $"{origen.Serie}-{origen.Numero}";

        const string sql = """
            SELECT TOP (1)
                ISNULL(LTRIM(RTRIM(ResumenTiket)), '') AS Ticket,
                ISNULL(LTRIM(RTRIM(RangoNumero)), '') AS RangoNumero,
                FechaReferencia,
                ResumenId
            FROM ResumenBoletas
            WHERE CompaniaId = @CompaniaId
              AND ISNULL(LTRIM(RTRIM(ResumenTiket)), '') <> ''
              AND (
                    UPPER(ISNULL(RangoNumero, '')) LIKE '%' + UPPER(@Comprobante) + '%'
                 OR UPPER(ISNULL(RangoNumero, '')) LIKE '%' + UPPER(@NumeroSolo) + '%'
                  )
            ORDER BY ResumenId DESC;
            """;

        await using var con = new SqlConnection(connectionString);
        await using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@CompaniaId", origen.CompaniaId);
        cmd.Parameters.AddWithValue("@Comprobante", comprobante);
        cmd.Parameters.AddWithValue("@NumeroSolo", origen.Numero);

        await con.OpenAsync(cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            var ticket = reader["Ticket"]?.ToString()?.Trim() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(ticket))
            {
                return ticket;
            }
        }

        const string sqlFallback = """
            SELECT TOP (200)
                ISNULL(LTRIM(RTRIM(ResumenTiket)), '') AS Ticket,
                ISNULL(LTRIM(RTRIM(RangoNumero)), '') AS RangoNumero,
                ResumenId
            FROM ResumenBoletas
            WHERE CompaniaId = @CompaniaId
              AND ISNULL(LTRIM(RTRIM(ResumenTiket)), '') <> ''
            ORDER BY ResumenId DESC;
            """;

        await using var cmdFallback = new SqlCommand(sqlFallback, con);
        cmdFallback.Parameters.AddWithValue("@CompaniaId", origen.CompaniaId);
        await using var readerFallback = await cmdFallback.ExecuteReaderAsync(cancellationToken);
        while (await readerFallback.ReadAsync(cancellationToken))
        {
            var ticket = readerFallback["Ticket"]?.ToString()?.Trim() ?? string.Empty;
            var rangoNumero = readerFallback["RangoNumero"]?.ToString()?.Trim() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(ticket) &&
                RangoResumenContieneBoleta(rangoNumero, origen.Serie, origen.Numero))
            {
                return ticket;
            }
        }

        return string.Empty;
    }

    private async Task<object> RegistrarResumenEnBaseDatosAsync(
        EnviarResumenBoletasRequest request,
        Dictionary<string, string>? respuestaLegacy,
        CancellationToken cancellationToken)
    {
        if (!request.COMPANIA_ID.HasValue || request.COMPANIA_ID.Value <= 0)
        {
            return new
            {
                ok = false,
                mensaje = "COMPANIA_ID es requerido para registrar el resumen en BD."
            };
        }

        if (!DateTime.TryParseExact(
                request.FECHA_REFERENCIA?.Trim(),
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var fechaReferencia))
        {
            return new
            {
                ok = false,
                mensaje = "FECHA_REFERENCIA es inválida para registrar el resumen en BD."
            };
        }

        if (!int.TryParse(request.SECUENCIA?.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var secuencia) &&
            !int.TryParse(request.SECUENCIA?.Trim(), out secuencia))
        {
            return new
            {
                ok = false,
                mensaje = "SECUENCIA es inválida para registrar el resumen en BD."
            };
        }

        var docuIds = (request.detalle ?? new List<EnviarResumenBoletasDetalleRequest>())
            .Where(x => x is not null && x.docuId.HasValue && x.docuId.Value > 0)
            .Select(x => x.docuId!.Value)
            .Distinct()
            .ToList();

        if (docuIds.Count == 0)
        {
            return new
            {
                ok = false,
                mensaje = "No se pudo registrar en BD: se requiere detalle.docuId para actualizar DocumentoVenta."
            };
        }

        var total = request.TOTAL ?? (request.detalle?.Sum(x => x?.total ?? 0m) ?? 0m);
        var igv = request.IGV ?? (request.detalle?.Sum(x => x?.igv ?? 0m) ?? 0m);
        var icbper = request.ICBPER ?? (request.detalle?.Sum(x => x?.icbper ?? 0m) ?? 0m);
        var subTotal = request.SUBTOTAL ?? (total - igv - icbper);
        if (subTotal < 0m) subTotal = 0m;

        var ticket = ResolverTicketRespuestaLegacy(respuestaLegacy);
        var codigoSunat = ObtenerValorLegacy(respuestaLegacy, "cod_sunat");
        var hashCdr = ObtenerValorLegacy(respuestaLegacy, "hash_cdr");
        var usuarioDocumento = await _mediator.ObtenerUsuarioDocumentoVentaAsync(
            docuIds.Select(x => (long)x),
            cancellationToken);
        var usuario = ResolverUsuarioRegistroResumen(request, usuarioDocumento, User);
        var estado = ResolverEstadoResumen(request);
        var rangoNumeros = ResolverRangoNumeros(request);
        var serie = string.IsNullOrWhiteSpace(request.SERIE) ? "RC" : request.SERIE.Trim();

        var cabecera = string.Join("|", new[]
        {
            request.COMPANIA_ID.Value.ToString(CultureInfo.InvariantCulture),
            SanitizarCampoListaOrden(serie),
            secuencia.ToString(CultureInfo.InvariantCulture),
            fechaReferencia.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            FormatearDecimalListaOrden(subTotal),
            FormatearDecimalListaOrden(igv),
            FormatearDecimalListaOrden(total),
            SanitizarCampoListaOrden(ticket),
            SanitizarCampoListaOrden(codigoSunat),
            SanitizarCampoListaOrden(hashCdr),
            SanitizarCampoListaOrden(usuario),
            estado.ToString(CultureInfo.InvariantCulture),
            SanitizarCampoListaOrden(rangoNumeros),
            FormatearDecimalListaOrden(icbper)
        });

        var detalle = string.Join(";", docuIds.Select(x => x.ToString(CultureInfo.InvariantCulture)));
        var listaOrden = $"{cabecera}[{detalle}";

        try
        {
            var resultado = await _mediator.RegistrarResumenBoletasAsync(listaOrden, cancellationToken);
            if (string.IsNullOrWhiteSpace(resultado))
            {
                return new
                {
                    ok = true,
                    mensaje = "SUNAT respondió OK y se ejecutó uspinsertarRB, pero el SP no devolvió payload."
                };
            }

            if (resultado == "~")
            {
                return new
                {
                    ok = true,
                    mensaje = "SUNAT respondió OK y se ejecutó uspinsertarRB. El SP devolvió '~' en el SELECT final.",
                    resultado
                };
            }

            return new
            {
                ok = true,
                resultado
            };
        }
        catch (Exception ex)
        {
            return new
            {
                ok = false,
                mensaje = $"SUNAT respondió OK, pero falló el registro en BD: {ex.Message}"
            };
        }
    }

    private static object NormalizarRespuestaResumen(
        Dictionary<string, string>? respuestaLegacy,
        string? mensajeError = null,
        object? registroBd = null)
    {
        var flgRta = ObtenerValorLegacy(respuestaLegacy, "flg_rta", "0");
        var mensaje = !string.IsNullOrWhiteSpace(mensajeError)
            ? mensajeError
            : ObtenerValorLegacy(respuestaLegacy, "mensaje");
        var codSunat = ObtenerValorLegacy(respuestaLegacy, "cod_sunat");
        var msjSunat = ObtenerValorLegacy(respuestaLegacy, "msj_sunat");
        var hashCpe = ObtenerValorLegacy(respuestaLegacy, "hash_cpe");
        var hashCdr = ObtenerValorLegacy(respuestaLegacy, "hash_cdr");
        var ticket = string.Equals(flgRta, "1", StringComparison.Ordinal) && string.IsNullOrWhiteSpace(codSunat)
            ? msjSunat
            : string.Empty;

        return new
        {
            ok = string.Equals(flgRta, "1", StringComparison.Ordinal),
            flg_rta = flgRta,
            mensaje,
            cod_sunat = codSunat,
            msj_sunat = msjSunat,
            hash_cpe = hashCpe,
            hash_cdr = hashCdr,
            aceptado = string.Equals(flgRta, "1", StringComparison.Ordinal) && string.IsNullOrWhiteSpace(codSunat),
            ticket,
            registro_bd = registroBd
        };
    }

    private static object NormalizarRespuestaFactura(
        Dictionary<string, string>? respuestaLegacy,
        string? mensajeError = null,
        object? registroBd = null)
    {
        var flgRta = ObtenerValorLegacy(respuestaLegacy, "flg_rta", "0");
        var mensaje = !string.IsNullOrWhiteSpace(mensajeError)
            ? mensajeError
            : ObtenerValorLegacy(respuestaLegacy, "mensaje");
        var codSunat = ObtenerValorLegacy(respuestaLegacy, "cod_sunat");
        var msjSunat = ObtenerValorLegacy(respuestaLegacy, "msj_sunat");
        var hashCpe = ObtenerValorLegacy(respuestaLegacy, "hash_cpe");
        var hashCdr = ObtenerValorLegacy(respuestaLegacy, "hash_cdr");
        var cdrBase64 = LimpiarBase64(ObtenerValorLegacy(respuestaLegacy, "cdr_base64"));

        return new
        {
            ok = string.Equals(flgRta, "1", StringComparison.Ordinal),
            flg_rta = flgRta,
            mensaje,
            cod_sunat = codSunat,
            msj_sunat = msjSunat,
            hash_cpe = hashCpe,
            hash_cdr = hashCdr,
            cdr_recibido = !string.IsNullOrWhiteSpace(cdrBase64),
            cdr_base64 = cdrBase64,
            aceptado = string.Equals(flgRta, "1", StringComparison.Ordinal) && EsCodigoFacturaAceptado(codSunat),
            ticket = string.Empty,
            registro_bd = registroBd
        };
    }

    private static bool EsCodigoFacturaAceptado(string? codSunat)
    {
        return string.Equals((codSunat ?? string.Empty).Trim(), "0", StringComparison.Ordinal);
    }

    private static object CrearRespuestaFacturaPendiente(string mensaje)
    {
        return NormalizarRespuestaFactura(
            null,
            mensaje,
            new
            {
                ok = false,
                mensaje = "La factura quedó pendiente de envío o reintento."
            });
    }

    private static bool EsFactura(string? notaDocu)
    {
        return string.Equals((notaDocu ?? string.Empty).Trim(), "FACTURA", StringComparison.OrdinalIgnoreCase);
    }

    private static bool EsBoleta(string? notaDocu)
    {
        return string.Equals((notaDocu ?? string.Empty).Trim(), "BOLETA", StringComparison.OrdinalIgnoreCase);
    }

    private static long? ExtraerNotaIdDeRegistro(string? resultadoRegistro)
    {
        if (string.IsNullOrWhiteSpace(resultadoRegistro))
        {
            return null;
        }

        var primerSegmento = resultadoRegistro.Split('¬', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        if (long.TryParse(primerSegmento, NumberStyles.Integer, CultureInfo.InvariantCulture, out var notaId))
        {
            return notaId;
        }

        return null;
    }

    private static string ResolverNumeroComprobanteDesdeRegistro(string? resultadoRegistro, string? numeroOriginal)
    {
        var numeroRegistro = ExtraerSegmentoRegistro(resultadoRegistro, 1);
        if (!string.IsNullOrWhiteSpace(numeroRegistro) && !EsNumeroComprobanteCero(numeroRegistro))
        {
            return numeroRegistro.Trim();
        }

        var numero = (numeroOriginal ?? string.Empty).Trim();
        return string.IsNullOrWhiteSpace(numero) ? "00000001" : numero;
    }

    private static string? ExtraerSegmentoRegistro(string? resultadoRegistro, int indice)
    {
        if (string.IsNullOrWhiteSpace(resultadoRegistro))
        {
            return null;
        }

        var segmentos = resultadoRegistro.Split('¬', StringSplitOptions.TrimEntries);
        return indice >= 0 && indice < segmentos.Length ? segmentos[indice] : null;
    }

    private static bool EsNumeroComprobanteCero(string numero)
    {
        var valor = numero.Trim();
        if (string.IsNullOrWhiteSpace(valor))
        {
            return true;
        }

        foreach (var caracter in valor)
        {
            if (caracter != '0')
            {
                return false;
            }
        }

        return true;
    }

    private static string ObtenerValorNormalizadoRespuesta(object respuesta, string propiedad)
    {
        try
        {
            var json = JsonSerializer.Serialize(respuesta);
            using var document = JsonDocument.Parse(json);
            if (!document.RootElement.TryGetProperty(propiedad, out var valor))
            {
                return string.Empty;
            }

            return valor.ValueKind switch
            {
                JsonValueKind.String => valor.GetString() ?? string.Empty,
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                JsonValueKind.Number => valor.ToString(),
                _ => valor.ToString()
            };
        }
        catch
        {
            return string.Empty;
        }
    }

    private static int ResolverTipoProcesoDesdeCredenciales(CredencialesSunat credenciales)
    {
        return int.TryParse((credenciales.Entorno ?? string.Empty).Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var tipoProceso)
            ? tipoProceso
            : 3;
    }

    private static string ObtenerDocumentoCliente(Cliente cliente)
    {
        return !string.IsNullOrWhiteSpace(cliente.ClienteRuc)
            ? cliente.ClienteRuc.Trim()
            : cliente.ClienteDni?.Trim() ?? string.Empty;
    }

    private static string FormatearFechaIso(DateTime? fecha)
    {
        return (fecha ?? DateTime.Today).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    private static string ResolverFormaPagoFactura(NotaPedido nota)
    {
        var valor = $"{nota.NotaCondicion} {nota.NotaFormaPago}".Trim();
        return valor.Contains("CRED", StringComparison.OrdinalIgnoreCase) ? "Credito" : "Contado";
    }

    private static string NormalizarUnidadMedidaSunat(string? unidadDetalle, string? unidadProducto)
    {
        var unidad = string.IsNullOrWhiteSpace(unidadDetalle) ? unidadProducto : unidadDetalle;
        var valor = (unidad ?? string.Empty).Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(valor))
        {
            return "NIU";
        }

        return valor switch
        {
            "UND" => "NIU",
            "UNID" => "NIU",
            "UNIDAD" => "NIU",
            "UNIDADES" => "NIU",
            "NIU" => "NIU",
            "CAJA" => "BX",
            "CAJAS" => "BX",
            "BX" => "BX",
            "KG" => "KGM",
            "KILO" => "KGM",
            "KGM" => "KGM",
            "LITRO" => "LTR",
            "LTR" => "LTR",
            _ => "NIU"
        };
    }

    private static decimal DecimalMax(decimal left, decimal right)
    {
        return left >= right ? left : right;
    }

    private static bool EsResultadoEdicionExitosa(string? resultado)
    {
        if (string.IsNullOrWhiteSpace(resultado))
        {
            return false;
        }

        var valor = resultado.Trim();
        return string.Equals(valor, "UPDATED", StringComparison.OrdinalIgnoreCase)
               || string.Equals(valor, "true", StringComparison.OrdinalIgnoreCase)
               || string.Equals(valor, "ok", StringComparison.OrdinalIgnoreCase);
    }

    private static bool EsDocumentoConIgv18(string? documento)
    {
        var valor = (documento ?? string.Empty).Trim().ToUpperInvariant();
        return valor is "FACTURA" or "BOLETA";
    }

    private static bool EsTipoDocumentoSunatConIgv18(string? codigoTipoDocumento)
    {
        var codigo = (codigoTipoDocumento ?? string.Empty).Trim();
        return codigo is "01" or "03" or "07" or "08";
    }

    private static TributacionDocumento CalcularTributacionDocumento(string? documento, decimal totalDocumento, decimal icbper)
    {
        return CalcularTributacionDocumento(totalDocumento, icbper, EsDocumentoConIgv18(documento), PorcentajeIgvDefault);
    }

    private static TributacionDocumento CalcularTributacionDocumento(decimal totalDocumento, decimal icbper, bool aplicaIgv, decimal porcentajeIgv)
    {
        var total = DecimalMax(0m, totalDocumento);
        var totalIcbper = DecimalMax(0m, icbper);
        var totalSinIcbper = DecimalMax(0m, total - totalIcbper);

        if (!aplicaIgv || porcentajeIgv <= 0m)
        {
            return new TributacionDocumento
            {
                Total = total,
                SubTotal = totalSinIcbper,
                Igv = 0m,
                Gravada = totalSinIcbper
            };
        }

        var divisor = 1m + (porcentajeIgv / 100m);
        var subTotal = totalSinIcbper / divisor;
        var igv = totalSinIcbper - subTotal;

        return new TributacionDocumento
        {
            Total = total,
            SubTotal = subTotal,
            Igv = igv,
            Gravada = subTotal
        };
    }

    private static void AplicarReglaTributariaDocumento(NotaPedido nota, IReadOnlyList<DetalleNota> detalles)
    {
        var totalIcbper = nota.ICBPER ?? 0m;
        var totalDetalle = detalles.Sum(x => x.DetalleImporte ?? 0m);
        var totalDocumento = nota.NotaTotal ?? (totalDetalle + totalIcbper);
        var calculo = CalcularTributacionDocumento(nota.NotaDocu, totalDocumento, totalIcbper);

        nota.NotaTotal = calculo.Total;
        nota.NotaSubtotal = calculo.SubTotal;
        nota.NotaPagar ??= calculo.Total;
    }

    private static decimal SumarImporteDetalleJson(JArray? detalleArray, JObject? detalleObjeto)
    {
        decimal total = 0m;

        if (detalleArray is not null)
        {
            foreach (var item in detalleArray)
            {
                total += ObtenerDecimalDesdeToken(item?["importe"] ?? item?["DetalleImporte"]);
            }

            return total;
        }

        if (detalleObjeto is null)
        {
            return total;
        }

        foreach (var property in detalleObjeto.Properties())
        {
            total += ObtenerDecimalDesdeToken(property.Value?["importe"] ?? property.Value?["DetalleImporte"]);
        }

        return total;
    }

    private static decimal ObtenerDecimalDesdeToken(JToken? token)
    {
        if (token is null)
        {
            return 0m;
        }

        if (decimal.TryParse(token.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var valueInvariant))
        {
            return valueInvariant;
        }

        return decimal.TryParse(token.ToString(), NumberStyles.Any, CultureInfo.CurrentCulture, out var valueCurrent)
            ? valueCurrent
            : 0m;
    }

    private static void AplicarCalculoTributarioParaEnvio(EnviarFacturaRequest request)
    {
        request.detalle ??= new List<EnviarFacturaDetalleRequest>();
        if (request.detalle.Count == 0)
        {
            request.SUB_TOTAL ??= 0m;
            request.TOTAL_GRAVADAS ??= 0m;
            request.TOTAL_IGV ??= 0m;
            request.TOTAL ??= request.TOTAL_ICBPER ?? 0m;
            return;
        }

        var porcentajeIgv = request.POR_IGV ?? PorcentajeIgvDefault;
        request.POR_IGV = porcentajeIgv;

        foreach (var item in request.detalle)
        {
            item.item ??= 0;
            item.cantidad ??= 0m;
            item.codTipoOperacion = string.IsNullOrWhiteSpace(item.codTipoOperacion) ? "10" : item.codTipoOperacion.Trim();
            if (!item.importe.HasValue && item.cantidad > 0 && item.precio.HasValue)
            {
                item.importe = item.cantidad.Value * item.precio.Value;
            }

            if (!item.importe.HasValue && item.cantidad > 0 && item.precioSinImpuesto.HasValue)
            {
                item.importe = item.cantidad.Value * item.precioSinImpuesto.Value;
            }

            if (!item.precio.HasValue && item.cantidad > 0 && item.importe.HasValue)
            {
                item.precio = item.importe.Value / item.cantidad.Value;
            }

            item.importe ??= 0m;
            item.precio ??= 0m;
            item.igv ??= 0m;
            item.subTotal ??= 0m;
            item.precioSinImpuesto ??= 0m;
            item.descuento ??= 0m;
            item.isc ??= 0m;
        }

        request.TOTAL_ICBPER ??= request.detalle.Sum(x => Convert.ToDecimal(x.impuestoIcbper ?? 0d));
        request.TOTAL_ISC ??= request.detalle.Sum(x => x.isc ?? 0m);
        request.TOTAL_OTR_IMP ??= 0m;
        request.TOTAL_GRATUITAS ??= 0m;
        request.TOTAL_DESCUENTO ??= request.detalle.Sum(x => x.descuento ?? 0m);

        var totalBrutoDetalle = request.detalle.Sum(x => x.importe ?? 0m);
        request.TOTAL ??= totalBrutoDetalle + (request.TOTAL_ICBPER ?? 0m);

        var totalDocumento = request.TOTAL ?? 0m;
        var totalIcbper = request.TOTAL_ICBPER ?? 0m;
        var totalSinIcbper = DecimalMax(0m, totalDocumento - totalIcbper);
        var aplicaIgv = EsTipoDocumentoSunatConIgv18(request.COD_TIPO_DOCUMENTO);

        var detalleGravado = new List<EnviarFacturaDetalleRequest>();
        decimal totalExonerado = 0m;
        decimal totalInafecto = 0m;
        decimal totalExportacion = 0m;

        foreach (var item in request.detalle)
        {
            var operacion = (item.codTipoOperacion ?? "10").Trim();
            var importeBruto = item.importe ?? 0m;

            switch (operacion)
            {
                case "20":
                    totalExonerado += importeBruto;
                    break;
                case "30":
                    totalInafecto += importeBruto;
                    break;
                case "40":
                    totalExportacion += importeBruto;
                    break;
                default:
                    detalleGravado.Add(item);
                    break;
            }
        }

        var totalNoGravado = totalExonerado + totalInafecto + totalExportacion;
        var totalGravadoConIgv = DecimalMax(0m, totalSinIcbper - totalNoGravado);
        var calculoGravado = CalcularTributacionDocumento(totalGravadoConIgv, 0m, aplicaIgv, porcentajeIgv);
        var sumaBrutaGravada = detalleGravado.Sum(x => x.importe ?? 0m);

        decimal restanteBruto = totalGravadoConIgv;
        decimal restanteSubTotal = calculoGravado.SubTotal;
        decimal restanteIgv = calculoGravado.Igv;

        for (var i = 0; i < detalleGravado.Count; i++)
        {
            var item = detalleGravado[i];
            var cantidad = item.cantidad ?? 0m;
            var brutoOriginal = item.importe ?? 0m;
            var esUltimo = i == detalleGravado.Count - 1;

            var importeBrutoLinea = esUltimo
                ? restanteBruto
                : (sumaBrutaGravada > 0m ? totalGravadoConIgv * (brutoOriginal / sumaBrutaGravada) : 0m);

            if (importeBrutoLinea < 0m)
            {
                importeBrutoLinea = 0m;
            }

            var subTotalLinea = esUltimo
                ? restanteSubTotal
                : (aplicaIgv && porcentajeIgv > 0m
                    ? importeBrutoLinea / (1m + (porcentajeIgv / 100m))
                    : importeBrutoLinea);

            var igvLinea = esUltimo
                ? restanteIgv
                : (aplicaIgv ? (importeBrutoLinea - subTotalLinea) : 0m);

            item.importe = subTotalLinea;
            item.igv = igvLinea;
            item.subTotal = subTotalLinea;
            item.precioSinImpuesto = cantidad > 0m ? subTotalLinea / cantidad : subTotalLinea;
            item.precio = cantidad > 0m ? importeBrutoLinea / cantidad : (item.precio ?? 0m);
            item.biIsc ??= subTotalLinea;

            restanteBruto -= importeBrutoLinea;
            restanteSubTotal -= subTotalLinea;
            restanteIgv -= igvLinea;
        }

        foreach (var item in request.detalle.Where(x => !detalleGravado.Contains(x)))
        {
            var importeBruto = item.importe ?? 0m;
            var cantidad = item.cantidad ?? 0m;
            item.igv = 0m;
            item.subTotal = importeBruto;
            item.precioSinImpuesto = cantidad > 0m ? importeBruto / cantidad : importeBruto;
            item.precio = cantidad > 0m ? importeBruto / cantidad : (item.precio ?? 0m);
            item.importe = importeBruto;
            item.biIsc ??= importeBruto;
        }

        request.TOTAL_GRAVADAS = request.detalle
            .Where(x => string.Equals((x.codTipoOperacion ?? string.Empty).Trim(), "10", StringComparison.Ordinal))
            .Sum(x => x.importe ?? 0m);
        request.TOTAL_EXONERADAS = request.detalle
            .Where(x => string.Equals((x.codTipoOperacion ?? string.Empty).Trim(), "20", StringComparison.Ordinal))
            .Sum(x => x.importe ?? 0m);
        request.TOTAL_INAFECTA = request.detalle
            .Where(x => string.Equals((x.codTipoOperacion ?? string.Empty).Trim(), "30", StringComparison.Ordinal))
            .Sum(x => x.importe ?? 0m);
        request.TOTAL_EXPORTACION = request.detalle
            .Where(x => string.Equals((x.codTipoOperacion ?? string.Empty).Trim(), "40", StringComparison.Ordinal))
            .Sum(x => x.importe ?? 0m);
        request.TOTAL_IGV = request.detalle.Sum(x => x.igv ?? 0m);
        request.SUB_TOTAL = request.detalle.Sum(x => x.importe ?? 0m);

        request.TOTAL = (request.SUB_TOTAL ?? 0m)
            + (request.TOTAL_IGV ?? 0m)
            + (request.TOTAL_ISC ?? 0m)
            + (request.TOTAL_ICBPER ?? 0m)
            + (request.TOTAL_OTR_IMP ?? 0m);

        NormalizarEscalaSunat(request);

        if (string.IsNullOrWhiteSpace(request.TOTAL_LETRAS))
        {
            request.TOTAL_LETRAS = Letras.enletras((request.TOTAL ?? 0m).ToString("N2", CultureInfo.InvariantCulture)) + " SOLES";
        }
    }

    private static void NormalizarEscalaSunat(EnviarFacturaRequest request)
    {
        request.detalle ??= new List<EnviarFacturaDetalleRequest>();

        foreach (var item in request.detalle)
        {
            item.cantidad = item.cantidad.HasValue
                ? RedondearSunat(item.cantidad.Value, DecimalesSunatPrecioUnitario)
                : null;
            item.precio = item.precio.HasValue
                ? RedondearSunat(item.precio.Value, DecimalesSunatPrecioUnitario)
                : null;
            item.precioSinImpuesto = item.precioSinImpuesto.HasValue
                ? RedondearSunat(item.precioSinImpuesto.Value, DecimalesSunatPrecioUnitario)
                : null;
            item.importe = item.importe.HasValue
                ? RedondearSunat(item.importe.Value, DecimalesSunatMonto)
                : null;
            item.subTotal = item.subTotal.HasValue
                ? RedondearSunat(item.subTotal.Value, DecimalesSunatMonto)
                : null;
            item.igv = item.igv.HasValue
                ? RedondearSunat(item.igv.Value, DecimalesSunatMonto)
                : null;
            item.biIsc = item.biIsc.HasValue
                ? RedondearSunat(item.biIsc.Value, DecimalesSunatMonto)
                : null;
            item.isc = item.isc.HasValue
                ? RedondearSunat(item.isc.Value, DecimalesSunatMonto)
                : null;
            item.descuento = item.descuento.HasValue
                ? RedondearSunat(item.descuento.Value, DecimalesSunatMonto)
                : null;
        }

        request.TOTAL_GRAVADAS = RedondearSunat(request.detalle
            .Where(x => string.Equals((x.codTipoOperacion ?? string.Empty).Trim(), "10", StringComparison.Ordinal))
            .Sum(x => x.importe ?? 0m), DecimalesSunatMonto);
        request.TOTAL_EXONERADAS = RedondearSunat(request.detalle
            .Where(x => string.Equals((x.codTipoOperacion ?? string.Empty).Trim(), "20", StringComparison.Ordinal))
            .Sum(x => x.importe ?? 0m), DecimalesSunatMonto);
        request.TOTAL_INAFECTA = RedondearSunat(request.detalle
            .Where(x => string.Equals((x.codTipoOperacion ?? string.Empty).Trim(), "30", StringComparison.Ordinal))
            .Sum(x => x.importe ?? 0m), DecimalesSunatMonto);
        request.TOTAL_EXPORTACION = RedondearSunat(request.detalle
            .Where(x => string.Equals((x.codTipoOperacion ?? string.Empty).Trim(), "40", StringComparison.Ordinal))
            .Sum(x => x.importe ?? 0m), DecimalesSunatMonto);
        request.TOTAL_IGV = RedondearSunat(request.detalle.Sum(x => x.igv ?? 0m), DecimalesSunatMonto);
        request.TOTAL_ISC = RedondearSunat(request.detalle.Sum(x => x.isc ?? 0m), DecimalesSunatMonto);
        request.SUB_TOTAL = RedondearSunat(request.detalle.Sum(x => x.importe ?? 0m), DecimalesSunatMonto);
        request.TOTAL_DESCUENTO = RedondearSunat(request.TOTAL_DESCUENTO ?? request.detalle.Sum(x => x.descuento ?? 0m), DecimalesSunatMonto);
        request.TOTAL_ICBPER = RedondearSunat(request.TOTAL_ICBPER ?? request.detalle.Sum(x => Convert.ToDecimal(x.impuestoIcbper ?? 0d)), DecimalesSunatMonto);
        request.TOTAL_OTR_IMP = RedondearSunat(request.TOTAL_OTR_IMP ?? 0m, DecimalesSunatMonto);
        request.TOTAL_GRATUITAS = RedondearSunat(request.TOTAL_GRATUITAS ?? 0m, DecimalesSunatMonto);

        request.TOTAL = RedondearSunat((request.SUB_TOTAL ?? 0m)
            + (request.TOTAL_IGV ?? 0m)
            + (request.TOTAL_ISC ?? 0m)
            + (request.TOTAL_ICBPER ?? 0m)
            + (request.TOTAL_OTR_IMP ?? 0m), DecimalesSunatMonto);
    }

    private static decimal RedondearSunat(decimal value, int decimales)
    {
        return Math.Round(value, decimales, MidpointRounding.AwayFromZero);
    }

    private async Task ActualizarTributacionPostEdicionAsync(NotaPedido nota, IReadOnlyList<DetalleNota> detalles, CancellationToken cancellationToken)
    {
        if (nota.NotaId <= 0)
        {
            return;
        }

        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return;
            }

            var totalIcbper = nota.ICBPER ?? 0m;
            var totalDetalle = detalles.Sum(x => x.DetalleImporte ?? 0m);
            var totalDocumento = nota.NotaTotal ?? (totalDetalle + totalIcbper);
            var calculo = CalcularTributacionDocumento(nota.NotaDocu, totalDocumento, totalIcbper);

            await using var con = new SqlConnection(connectionString);
            await con.OpenAsync(cancellationToken);

            const string sqlNota = """
                UPDATE NotaPedido
                SET NotaSubtotal = @NotaSubtotal,
                    NotaTotal = @NotaTotal,
                    ICBPER = @ICBPER,
                    NotaPagar = CASE WHEN ISNULL(NotaPagar, 0) = 0 THEN @NotaTotal ELSE NotaPagar END
                WHERE NotaId = @NotaId;
                """;

            await using (var cmdNota = new SqlCommand(sqlNota, con))
            {
                cmdNota.Parameters.AddWithValue("@NotaId", nota.NotaId);
                cmdNota.Parameters.AddWithValue("@NotaSubtotal", calculo.SubTotal);
                cmdNota.Parameters.AddWithValue("@NotaTotal", calculo.Total);
                cmdNota.Parameters.AddWithValue("@ICBPER", totalIcbper);
                await cmdNota.ExecuteNonQueryAsync(cancellationToken);
            }

            const string sqlDocumentoVenta = """
                UPDATE DocumentoVenta
                SET DocuSubTotal = @DocuSubTotal,
                    DocuIgv = @DocuIgv,
                    DocuGravada = @DocuGravada,
                    DocuDescuento = @DocuDescuento,
                    ICBPER = @ICBPER,
                    DocuTotal = @DocuTotal
                WHERE NotaId = @NotaId
                  AND TipoCodigo IN ('01', '03');
                """;

            await using var cmdDocu = new SqlCommand(sqlDocumentoVenta, con);
            cmdDocu.Parameters.AddWithValue("@NotaId", nota.NotaId);
            cmdDocu.Parameters.AddWithValue("@DocuSubTotal", calculo.SubTotal);
            cmdDocu.Parameters.AddWithValue("@DocuIgv", calculo.Igv);
            cmdDocu.Parameters.AddWithValue("@DocuGravada", calculo.Gravada);
            cmdDocu.Parameters.AddWithValue("@DocuDescuento", nota.NotaDescuento ?? 0m);
            cmdDocu.Parameters.AddWithValue("@ICBPER", totalIcbper);
            cmdDocu.Parameters.AddWithValue("@DocuTotal", calculo.Total);
            await cmdDocu.ExecuteNonQueryAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "No se pudo ajustar la cabecera tributaria tras editar la nota {NotaId}.", nota.NotaId);
        }
    }

    private async Task<UbigeoInfo?> ObtenerUbigeoAsync(string? codigoUbigeo, CancellationToken cancellationToken)
    {
        var codigo = (codigoUbigeo ?? string.Empty).Trim();
        if (codigo.Length != 6)
        {
            return null;
        }

        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return null;
        }

        var dep = codigo[..2];
        var prov = codigo.Substring(2, 2);
        var dist = codigo.Substring(4, 2);

        const string sql = """
            SELECT IdDepa, IdProv, IdDist, Nombre
            FROM Ubigeo
            WHERE IdDepa = @IdDepa
              AND (
                    (IdProv = '00' AND IdDist = '00')
                 OR (IdProv = @IdProv AND IdDist = '00')
                 OR (IdProv = @IdProv AND IdDist = @IdDist)
              );
            """;

        await using var con = new SqlConnection(connectionString);
        await using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@IdDepa", dep);
        cmd.Parameters.AddWithValue("@IdProv", prov);
        cmd.Parameters.AddWithValue("@IdDist", dist);

        await con.OpenAsync(cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

        string? departamento = null;
        string? provincia = null;
        string? distrito = null;

        while (await reader.ReadAsync(cancellationToken))
        {
            var idProv = reader["IdProv"]?.ToString()?.Trim();
            var idDist = reader["IdDist"]?.ToString()?.Trim();
            var nombre = reader["Nombre"]?.ToString()?.Trim();
            if (string.IsNullOrWhiteSpace(nombre))
            {
                continue;
            }

            if (idProv == "00" && idDist == "00")
            {
                departamento = nombre;
            }
            else if (idDist == "00")
            {
                provincia = nombre;
            }
            else
            {
                distrito = nombre;
            }
        }

        if (string.IsNullOrWhiteSpace(departamento) &&
            string.IsNullOrWhiteSpace(provincia) &&
            string.IsNullOrWhiteSpace(distrito))
        {
            return null;
        }

        return new UbigeoInfo(
            codigo,
            departamento ?? string.Empty,
            provincia ?? string.Empty,
            distrito ?? string.Empty);
    }

    private static string NormalizarFormaPago(string? formaPago)
    {
        var valor = (formaPago ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(valor))
        {
            return "Contado";
        }

        return valor.Equals("contado", StringComparison.OrdinalIgnoreCase)
            ? "Contado"
            : valor.Equals("credito", StringComparison.OrdinalIgnoreCase)
                ? "Credito"
                : valor;
    }

    private static string InferirTipoDocumentoCliente(string? tipoDocumentoCliente, string? nroDocumentoCliente)
    {
        if (!string.IsNullOrWhiteSpace(tipoDocumentoCliente))
        {
            return tipoDocumentoCliente.Trim();
        }

        var nro = (nroDocumentoCliente ?? string.Empty).Trim();
        if (nro.Length == 11)
        {
            return "6";
        }

        if (nro.Length == 8)
        {
            return "1";
        }

        return string.Empty;
    }

    private static int ResolverEstadoResumen(EnviarResumenBoletasRequest request)
    {
        if (request.STATUS.HasValue)
        {
            return request.STATUS.Value == 3 ? 3 : 1;
        }

        var primerEstado = request.detalle?
            .Select(x => x?.statu)
            .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

        if (int.TryParse(primerEstado?.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var estado))
        {
            return estado == 3 ? 3 : 1;
        }

        return 1;
    }

    private static string ResolverRangoNumeros(EnviarResumenBoletasRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.RANGO_NUMEROS))
        {
            return request.RANGO_NUMEROS.Trim();
        }

        var comprobantes = (request.detalle ?? new List<EnviarResumenBoletasDetalleRequest>())
            .Where(x => !string.IsNullOrWhiteSpace(x?.nroComprobante))
            .Select(x => x!.nroComprobante!.Trim())
            .ToList();

        if (comprobantes.Count == 0) return string.Empty;
        if (comprobantes.Count == 1) return comprobantes[0];
        return $"{comprobantes[0]}-{comprobantes[^1]}";
    }

    private static string ResolverTicketRespuestaLegacy(Dictionary<string, string>? respuestaLegacy)
    {
        var ticketDirecto = ObtenerValorLegacy(respuestaLegacy, "ticket");
        if (!string.IsNullOrWhiteSpace(ticketDirecto))
        {
            return ticketDirecto;
        }

        var flgRta = ObtenerValorLegacy(respuestaLegacy, "flg_rta", "0");
        var codSunat = ObtenerValorLegacy(respuestaLegacy, "cod_sunat");
        var msjSunat = ObtenerValorLegacy(respuestaLegacy, "msj_sunat");
        return string.Equals(flgRta, "1", StringComparison.Ordinal) && string.IsNullOrWhiteSpace(codSunat)
            ? msjSunat
            : string.Empty;
    }

    private static string FormatearDecimalListaOrden(decimal valor)
    {
        return FormatDecimalSinRedondeo(valor);
    }

    private static string FormatearDecimalListaOrden(decimal valor, int decimales)
    {
        _ = decimales;
        return FormatDecimalSinRedondeo(valor);
    }

    private static EnviarResumenBoletasRequest NormalizarRequestParaBaja(EnviarResumenBoletasRequest request)
    {
        request.STATUS = 3;
        var modoEnvio = ResolverModoEnvioBaja(request);

        if (modoEnvio == ModoEnvioBaja.ResumenBoletas)
        {
            request.CODIGO = "RC";
        }
        else if (modoEnvio == ModoEnvioBaja.ComunicacionBaja)
        {
            request.CODIGO = "RA";
        }

        if (string.IsNullOrWhiteSpace(request.FECHA_DOCUMENTO))
        {
            request.FECHA_DOCUMENTO = DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        if (string.IsNullOrWhiteSpace(request.SERIE))
        {
            request.SERIE = ResolverSerieDesdeFecha(request.FECHA_DOCUMENTO);
        }

        if (request.detalle is null)
        {
            request.detalle = new List<EnviarResumenBoletasDetalleRequest>();
            return request;
        }

        for (var i = 0; i < request.detalle.Count; i++)
        {
            var item = request.detalle[i];
            if (item is null)
            {
                continue;
            }

            item.item ??= i + 1;
            item.statu = "3";
            if (string.IsNullOrWhiteSpace(item.tipoComprobante)) item.tipoComprobante = "03";
            if (modoEnvio == ModoEnvioBaja.ResumenBoletas)
            {
                if (string.IsNullOrWhiteSpace(item.tipoDocumento)) item.tipoDocumento = "1";
                if (string.IsNullOrWhiteSpace(item.nroDocumento)) item.nroDocumento = "00000000";
                if (string.IsNullOrWhiteSpace(item.codMoneda)) item.codMoneda = "PEN";
                item.descripcion ??= "ANULACION DE BOLETA";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(item.descripcion)) item.descripcion = "ANULACION DE DOCUMENTO";
            }
        }

        return request;
    }

    private static ModoEnvioBaja ResolverModoEnvioBaja(EnviarResumenBoletasRequest request)
    {
        var detalles = request.detalle?
            .Where(x => x is not null)
            .ToList() ?? new List<EnviarResumenBoletasDetalleRequest>();

        if (detalles.Count == 0)
        {
            return ModoEnvioBaja.ResumenBoletas;
        }

        var tieneBoletas = detalles.Any(x => string.Equals((x.tipoComprobante ?? string.Empty).Trim(), "03", StringComparison.Ordinal));
        var tieneOtros = detalles.Any(x => !string.IsNullOrWhiteSpace(x.tipoComprobante) &&
                                           !string.Equals(x.tipoComprobante.Trim(), "03", StringComparison.Ordinal));

        if (tieneBoletas && tieneOtros)
        {
            return ModoEnvioBaja.MezclaNoSoportada;
        }

        return tieneBoletas ? ModoEnvioBaja.ResumenBoletas : ModoEnvioBaja.ComunicacionBaja;
    }

    private static string ResolverSerieBaja(EnviarResumenBoletasRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.SERIE))
        {
            return request.SERIE.Trim();
        }

        return ResolverSerieDesdeFecha(request.FECHA_DOCUMENTO);
    }

    private static string ResolverSerieDesdeFecha(string? fechaTexto)
    {
        if (DateTime.TryParseExact(
                fechaTexto?.Trim(),
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var fecha))
        {
            return fecha.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        }

        return DateTime.Today.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
    }

    private static (string Serie, string Numero) ResolverSerieNumeroBaja(EnviarResumenBoletasDetalleRequest detalle)
    {
        var serie = (detalle.serie ?? string.Empty).Trim();
        var numero = (detalle.numero ?? string.Empty).Trim();
        if (!string.IsNullOrWhiteSpace(serie) && !string.IsNullOrWhiteSpace(numero))
        {
            return (serie, numero);
        }

        var nroComprobante = (detalle.nroComprobante ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(nroComprobante))
        {
            return (serie, numero);
        }

        var separador = nroComprobante.IndexOf('-', StringComparison.Ordinal);
        if (separador <= 0 || separador >= nroComprobante.Length - 1)
        {
            return (serie, numero);
        }

        return (
            nroComprobante[..separador].Trim(),
            nroComprobante[(separador + 1)..].Trim());
    }

    private static string ResolverDescripcionBaja(EnviarResumenBoletasDetalleRequest detalle)
    {
        if (!string.IsNullOrWhiteSpace(detalle.descripcion))
        {
            return detalle.descripcion.Trim();
        }

        return "ANULACION DE DOCUMENTO";
    }

    private async Task<string?> ConstruirListaOrdenNotaCreditoDesdeBdAsync(
        EnviarFacturaRequest request,
        string codSunat,
        string mensajeSunat,
        string hashCpe,
        CancellationToken cancellationToken)
    {
        var origen = await ObtenerOrigenNotaCreditoDesdeBdAsync(request, cancellationToken);
        if (origen is null || origen.Detalles.Count == 0)
        {
            return null;
        }

        var (serieNc, numeroNc) = SepararSerieNumeroComprobante(request.NRO_COMPROBANTE);
        if (string.IsNullOrWhiteSpace(serieNc) || string.IsNullOrWhiteSpace(numeroNc))
        {
            return null;
        }

        var fechaDocumento = DateTime.TryParseExact(
            request.FECHA_DOCUMENTO?.Trim(),
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var fechaDoc)
            ? fechaDoc
            : DateTime.Today;

        var subTotal = request.SUB_TOTAL ?? origen.SubTotal;
        var igv = request.TOTAL_IGV ?? origen.Igv;
        var total = request.TOTAL ?? origen.Total;
        var gravada = request.TOTAL_GRAVADAS ?? (subTotal > 0m ? subTotal : origen.Gravada);
        var descuento = request.TOTAL_DESCUENTO ?? origen.Descuento;
        var icbper = request.TOTAL_ICBPER ?? origen.Icbper;
        var formaPago = string.IsNullOrWhiteSpace(request.FORMA_PAGO)
            ? (string.IsNullOrWhiteSpace(origen.FormaPago) ? "Contado" : origen.FormaPago)
            : request.FORMA_PAGO!.Trim();
        var concepto = DocuConceptoNotaCreditoDefault;
        var referencia = string.IsNullOrWhiteSpace(request.NRO_DOCUMENTO_MODIFICA)
            ? $"{origen.Serie}-{origen.Numero}"
            : request.NRO_DOCUMENTO_MODIFICA!.Trim();
        var totalLetras = string.IsNullOrWhiteSpace(request.TOTAL_LETRAS)
            ? "CERO CON 00/100 SOLES"
            : request.TOTAL_LETRAS!.Trim();

        var clienteRuc = origen.ClienteRuc;
        var clienteDni = origen.ClienteDni;
        if (!string.IsNullOrWhiteSpace(request.NRO_DOCUMENTO_CLIENTE))
        {
            var tipoDocCliente = (request.TIPO_DOCUMENTO_CLIENTE ?? string.Empty).Trim();
            if (tipoDocCliente == "6")
            {
                clienteRuc = request.NRO_DOCUMENTO_CLIENTE.Trim();
                clienteDni = string.Empty;
            }
            else if (tipoDocCliente == "1")
            {
                clienteDni = request.NRO_DOCUMENTO_CLIENTE.Trim();
                clienteRuc = string.Empty;
            }
        }

        var efectivo = origen.Efectivo > 0m ? -origen.Efectivo : origen.Efectivo;
        var deposito = origen.Deposito > 0m ? -origen.Deposito : origen.Deposito;

        var cabecera = string.Join("|", new[]
        {
            origen.CompaniaId.ToString(CultureInfo.InvariantCulture),                                    // 1
            origen.NotaId.ToString(CultureInfo.InvariantCulture),                                        // 2
            "NOTA DE CREDITO",                                                                           // 3
            SanitizarCampoListaOrden(numeroNc),                                                          // 4
            origen.ClienteId.ToString(CultureInfo.InvariantCulture),                                     // 5
            fechaDocumento.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),                         // 6
            FormatearDecimalListaOrden(subTotal),                                                        // 7
            FormatearDecimalListaOrden(igv),                                                             // 8
            FormatearDecimalListaOrden(total),                                                           // 9
            SanitizarCampoListaOrden(origen.Usuario),                                                    // 10
            SanitizarCampoListaOrden(serieNc),                                                           // 11
            "07",                                                                                         // 12
            FormatearDecimalListaOrden(descuento),                                                       // 13
            origen.DocuId.ToString(CultureInfo.InvariantCulture),                                        // 14
            SanitizarCampoListaOrden(concepto),                                                          // 15
            SanitizarCampoListaOrden(hashCpe),                                                           // 16
            "ENVIADO",                                                                                    // 17
            SanitizarCampoListaOrden(totalLetras),                                                       // 18
            SanitizarCampoListaOrden(referencia),                                                        // 19
            FormatearDecimalListaOrden(icbper),                                                          // 20
            SanitizarCampoListaOrden(codSunat),                                                          // 21
            SanitizarCampoListaOrden(mensajeSunat),                                                      // 22
            FormatearDecimalListaOrden(gravada),                                                         // 23
            FormatearDecimalListaOrden(descuento),                                                       // 24
            FormatearDecimalListaOrden(efectivo),                                                        // 25
            FormatearDecimalListaOrden(deposito),                                                        // 26
            SanitizarCampoListaOrden(origen.ClienteRazon),                                               // 27
            SanitizarCampoListaOrden(clienteRuc),                                                        // 28
            SanitizarCampoListaOrden(clienteDni),                                                        // 29
            SanitizarCampoListaOrden(origen.DireccionFiscal),                                            // 30
            SanitizarCampoListaOrden(formaPago),                                                         // 31
            SanitizarCampoListaOrden(origen.EntidadBancaria),                                            // 32
            SanitizarCampoListaOrden(origen.NroOperacion)                                                // 33
        });

        var detalle = string.Join(";", origen.Detalles.Select(d => string.Join("|", new[]
        {
            FormatearDecimalListaOrden(d.Cantidad),
            SanitizarCampoListaOrden(d.Um),
            SanitizarCampoListaOrden(d.Descripcion),
            FormatearDecimalListaOrden(d.Precio),
            FormatearDecimalListaOrden(d.Importe),
            d.DetalleNotaId.ToString(CultureInfo.InvariantCulture),
            d.IdProducto.ToString(CultureInfo.InvariantCulture),
            FormatearDecimalListaOrden(d.ValorUm, 4),
            FormatearDecimalListaOrden(d.Costo, 4),
            SanitizarCampoListaOrden(d.AplicaInv)
        })));

        if (string.IsNullOrWhiteSpace(detalle))
        {
            return null;
        }

        // uspinsertarNC del script actual espera tres bloques: CABECERA[DETALLE[GUIA
        return $"{cabecera}[{detalle}[";
    }

    private async Task<NotaCreditoOrigenBd?> ObtenerOrigenNotaCreditoDesdeBdAsync(
        EnviarFacturaRequest request,
        CancellationToken cancellationToken)
    {
        var (serieRef, numeroRef) = SepararSerieNumeroComprobante(request.NRO_DOCUMENTO_MODIFICA);
        if ((!request.DOCU_ID.HasValue || request.DOCU_ID.Value <= 0) &&
            (string.IsNullOrWhiteSpace(serieRef) || string.IsNullOrWhiteSpace(numeroRef)))
        {
            return null;
        }

        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return null;
        }

        const string sql = """
            SELECT TOP (1)
                d.DocuId,
                d.TipoCodigo,
                d.DocuEstado,
                d.CompaniaId,
                d.NotaId,
                d.ClienteId,
                d.DocuUsuario,
                d.FormaPago,
                d.EntidadBancaria,
                d.NroOperacion,
                d.Efectivo,
                d.Deposito,
                d.DocuSubTotal,
                d.DocuIgv,
                d.DocuTotal,
                d.DocuGravada,
                d.DocuDescuento,
                d.ICBPER,
                d.ClienteRazon,
                d.ClienteRuc,
                d.ClienteDni,
                d.DireccionFiscal,
                d.DocuSerie,
                d.DocuNumero
            FROM DocumentoVenta d
            WHERE d.TipoCodigo IN ('01', '03')
              AND (
                    (@SerieRef <> '' AND @NumeroRef <> '' AND LTRIM(RTRIM(d.DocuSerie)) = @SerieRef AND LTRIM(RTRIM(d.DocuNumero)) = @NumeroRef)
                    OR ((@SerieRef = '' OR @NumeroRef = '') AND @DocuId > 0 AND d.DocuId = @DocuId)
                  )
            ORDER BY CASE
                        WHEN @SerieRef <> '' AND @NumeroRef <> '' AND LTRIM(RTRIM(d.DocuSerie)) = @SerieRef AND LTRIM(RTRIM(d.DocuNumero)) = @NumeroRef THEN 0
                        WHEN @DocuId > 0 AND d.DocuId = @DocuId THEN 1
                        ELSE 2
                     END,
                     d.DocuId DESC;
            """;

        await using var con = new SqlConnection(connectionString);
        await using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@DocuId", request.DOCU_ID ?? 0L);
        cmd.Parameters.AddWithValue("@SerieRef", serieRef);
        cmd.Parameters.AddWithValue("@NumeroRef", numeroRef);

        await con.OpenAsync(cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        var docuId = reader["DocuId"] == DBNull.Value ? 0L : Convert.ToInt64(reader["DocuId"]);
        var notaId = reader["NotaId"] == DBNull.Value ? 0L : Convert.ToInt64(reader["NotaId"]);
        if (docuId <= 0 || notaId <= 0)
        {
            return null;
        }

        var origen = new NotaCreditoOrigenBd
        {
            DocuId = docuId,
            TipoCodigo = reader["TipoCodigo"]?.ToString()?.Trim() ?? string.Empty,
            Estado = reader["DocuEstado"]?.ToString()?.Trim() ?? string.Empty,
            CompaniaId = reader["CompaniaId"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CompaniaId"]),
            NotaId = notaId,
            ClienteId = reader["ClienteId"] == DBNull.Value ? 0L : Convert.ToInt64(reader["ClienteId"]),
            Usuario = reader["DocuUsuario"]?.ToString()?.Trim() ?? string.Empty,
            FormaPago = reader["FormaPago"]?.ToString()?.Trim() ?? string.Empty,
            EntidadBancaria = reader["EntidadBancaria"]?.ToString()?.Trim() ?? string.Empty,
            NroOperacion = reader["NroOperacion"]?.ToString()?.Trim() ?? string.Empty,
            Efectivo = reader["Efectivo"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["Efectivo"]),
            Deposito = reader["Deposito"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["Deposito"]),
            SubTotal = reader["DocuSubTotal"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["DocuSubTotal"]),
            Igv = reader["DocuIgv"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["DocuIgv"]),
            Total = reader["DocuTotal"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["DocuTotal"]),
            Gravada = reader["DocuGravada"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["DocuGravada"]),
            Descuento = reader["DocuDescuento"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["DocuDescuento"]),
            Icbper = reader["ICBPER"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["ICBPER"]),
            ClienteRazon = reader["ClienteRazon"]?.ToString()?.Trim() ?? string.Empty,
            ClienteRuc = reader["ClienteRuc"]?.ToString()?.Trim() ?? string.Empty,
            ClienteDni = reader["ClienteDni"]?.ToString()?.Trim() ?? string.Empty,
            DireccionFiscal = reader["DireccionFiscal"]?.ToString()?.Trim() ?? string.Empty,
            Serie = reader["DocuSerie"]?.ToString()?.Trim() ?? string.Empty,
            Numero = reader["DocuNumero"]?.ToString()?.Trim() ?? string.Empty
        };

        origen.Detalles = (await ObtenerDetallesOrigenNotaCreditoDesdeBdAsync(origen.DocuId, cancellationToken)).ToList();
        return origen.Detalles.Count == 0 ? null : origen;
    }

    private async Task<IReadOnlyList<NotaCreditoOrigenDetalleBd>> ObtenerDetallesOrigenNotaCreditoDesdeBdAsync(
        long docuIdOrigen,
        CancellationToken cancellationToken)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString) || docuIdOrigen <= 0)
        {
            return Array.Empty<NotaCreditoOrigenDetalleBd>();
        }

        const string sql = """
            SELECT
                dd.IdProducto,
                dd.DetalleCantidad,
                dd.DetalleUM,
                COALESCE(NULLIF(dd.DetalleDescripcion, ''), p.ProductoNombre, '') AS Descripcion,
                dd.DetallPrecio,
                dd.DetalleImporte,
                COALESCE(dd.DetalleNotaId, dp.DetalleId, 0) AS DetalleNotaId,
                COALESCE(dd.ValorUM, dp.ValorUM, 1) AS ValorUM,
                COALESCE(dp.DetalleCosto, p.ProductoCosto, 0) AS Costo,
                COALESCE(NULLIF(p.AplicaINV, ''), 'S') AS AplicaINV,
                COALESCE(NULLIF(s.CodigoSunat, ''), '') AS CodigoSunat
            FROM DetalleDocumento dd
            LEFT JOIN DetallePedido dp
                ON dp.DetalleId = dd.DetalleNotaId
            LEFT JOIN Producto p
                ON p.IdProducto = dd.IdProducto
            LEFT JOIN Sublinea s
                ON s.IdSubLinea = p.IdSubLinea
            WHERE dd.DocuId = @DocuId
            ORDER BY dd.DetalleId ASC;
            """;

        await using var con = new SqlConnection(connectionString);
        await using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@DocuId", docuIdOrigen);

        await con.OpenAsync(cancellationToken);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        var lista = new List<NotaCreditoOrigenDetalleBd>();
        while (await reader.ReadAsync(cancellationToken))
        {
            var idProducto = reader["IdProducto"] == DBNull.Value ? 0L : Convert.ToInt64(reader["IdProducto"]);
            if (idProducto <= 0)
            {
                continue;
            }

            var aplicaInvRaw = reader["AplicaINV"]?.ToString()?.Trim().ToUpperInvariant();
            var aplicaInv = aplicaInvRaw == "N" ? "N" : "S";

            lista.Add(new NotaCreditoOrigenDetalleBd
            {
                IdProducto = idProducto,
                Cantidad = reader["DetalleCantidad"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["DetalleCantidad"]),
                Um = reader["DetalleUM"]?.ToString()?.Trim() ?? string.Empty,
                Descripcion = reader["Descripcion"]?.ToString()?.Trim() ?? string.Empty,
                Precio = reader["DetallPrecio"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["DetallPrecio"]),
                Importe = reader["DetalleImporte"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["DetalleImporte"]),
                DetalleNotaId = reader["DetalleNotaId"] == DBNull.Value ? 0L : Convert.ToInt64(reader["DetalleNotaId"]),
                ValorUm = reader["ValorUM"] == DBNull.Value ? 1m : Convert.ToDecimal(reader["ValorUM"]),
                Costo = reader["Costo"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["Costo"]),
                AplicaInv = aplicaInv,
                CodigoSunat = reader["CodigoSunat"]?.ToString()?.Trim() ?? string.Empty
            });
        }

        return lista;
    }

    private static (string Serie, string Numero) SepararSerieNumeroComprobante(string? nroComprobante)
    {
        var valor = (nroComprobante ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(valor))
        {
            return (string.Empty, string.Empty);
        }

        var pos = valor.IndexOf('-', StringComparison.Ordinal);
        if (pos <= 0 || pos >= valor.Length - 1)
        {
            return (string.Empty, valor);
        }

        return (valor[..pos].Trim(), valor[(pos + 1)..].Trim());
    }

    private static string SanitizarCampoListaOrden(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return string.Empty;
        }

        return valor
            .Trim()
            .Replace("|", " ", StringComparison.Ordinal)
            .Replace("[", "(", StringComparison.Ordinal)
            .Replace("]", ")", StringComparison.Ordinal)
            .Replace(";", ",", StringComparison.Ordinal);
    }

    private static string ForzarDocuConceptoListaOrdenNotaCredito(string listaOrdenNc)
    {
        if (string.IsNullOrWhiteSpace(listaOrdenNc))
        {
            return listaOrdenNc;
        }

        var posSeparadorCabecera = listaOrdenNc.IndexOf('[', StringComparison.Ordinal);
        if (posSeparadorCabecera <= 0)
        {
            return listaOrdenNc;
        }

        var cabecera = listaOrdenNc[..posSeparadorCabecera];
        var resto = listaOrdenNc[posSeparadorCabecera..];
        var campos = cabecera.Split('|');
        if (campos.Length < 15)
        {
            return listaOrdenNc;
        }

        campos[14] = SanitizarCampoListaOrden(DocuConceptoNotaCreditoDefault);
        return string.Join("|", campos) + resto;
    }

    private static bool RangoResumenContieneBoleta(string? rangoNumero, string serieBoleta, string numeroBoleta)
    {
        var serie = (serieBoleta ?? string.Empty).Trim().ToUpperInvariant();
        var numero = (numeroBoleta ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(serie) || string.IsNullOrWhiteSpace(numero))
        {
            return false;
        }

        var rango = (rangoNumero ?? string.Empty).Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(rango))
        {
            return false;
        }

        var comprobante = $"{serie}-{numero}";
        if (string.Equals(rango, comprobante, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (rango.Contains(comprobante, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var partes = rango.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (partes.Length != 4)
        {
            return false;
        }

        var serieInicio = partes[0].Trim().ToUpperInvariant();
        var numeroInicio = partes[1].Trim();
        var serieFin = partes[2].Trim().ToUpperInvariant();
        var numeroFin = partes[3].Trim();

        if (!string.Equals(serieInicio, serie, StringComparison.Ordinal) ||
            !string.Equals(serieFin, serie, StringComparison.Ordinal))
        {
            return false;
        }

        if (!int.TryParse(numeroInicio, NumberStyles.Integer, CultureInfo.InvariantCulture, out var ini) ||
            !int.TryParse(numeroFin, NumberStyles.Integer, CultureInfo.InvariantCulture, out var fin) ||
            !int.TryParse(numero, NumberStyles.Integer, CultureInfo.InvariantCulture, out var actual))
        {
            return false;
        }

        if (ini > fin)
        {
            (ini, fin) = (fin, ini);
        }

        return actual >= ini && actual <= fin;
    }

    private static string ResolverUsuarioRegistroResumen(
        EnviarResumenBoletasRequest request,
        string? usuarioDocumento,
        ClaimsPrincipal? principal)
    {
        if (!string.IsNullOrWhiteSpace(request.USUARIO))
        {
            return request.USUARIO.Trim();
        }

        if (!string.IsNullOrWhiteSpace(usuarioDocumento))
        {
            return usuarioDocumento.Trim();
        }

        var usuarioClaim = ObtenerUsuarioDesdeClaims(principal);
        if (!string.IsNullOrWhiteSpace(usuarioClaim))
        {
            return usuarioClaim.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.USUARIO_SOL_EMPRESA))
        {
            return request.USUARIO_SOL_EMPRESA.Trim();
        }

        return "SYSTEM";
    }

    private static string? ObtenerUsuarioDesdeClaims(ClaimsPrincipal? principal)
    {
        if (principal is null)
        {
            return null;
        }

        var tipos = new[]
        {
            ClaimTypes.NameIdentifier,
            ClaimTypes.Name,
            "nameid",
            "unique_name",
            "preferred_username",
            "username",
            "usuario",
            "Usuario",
            "user"
        };

        foreach (var tipo in tipos)
        {
            var valor = principal.Claims
                .FirstOrDefault(c => string.Equals(c.Type, tipo, StringComparison.OrdinalIgnoreCase))
                ?.Value;
            if (!string.IsNullOrWhiteSpace(valor))
            {
                return valor.Trim();
            }
        }

        return null;
    }

    private static string ObtenerValorLegacy(Dictionary<string, string>? data, string key, string fallback = "")
    {
        if (data is null)
        {
            return fallback;
        }

        return data.TryGetValue(key, out var valor) ? valor ?? fallback : fallback;
    }

    private static int? ParseTipoProceso(JsonElement tipoProceso)
    {
        if (tipoProceso.ValueKind == JsonValueKind.Number && tipoProceso.TryGetInt32(out var tipoProcesoNumerico))
        {
            return tipoProcesoNumerico;
        }

        if (tipoProceso.ValueKind == JsonValueKind.String &&
            int.TryParse(tipoProceso.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var tipoProcesoTexto))
        {
            return tipoProcesoTexto;
        }

        return null;
    }

    private static bool EsFechaIsoValida(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return false;
        }

        return DateTime.TryParseExact(
            valor.Trim(),
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out _);
    }

    private static void AgregarErrorSiVacio(List<string> errores, string? valor, string mensaje)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            errores.Add(mensaje);
        }
    }

    private static string ResolverRutaPfx(string rutaPfx)
    {
        var valor = (rutaPfx ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(valor))
        {
            return valor;
        }

        if (!TryObtenerBytesDesdeBase64(valor, out var bytesPfx))
        {
            return valor;
        }

        var directorioCertificados = ObtenerDirectorioCertificados();
        Directory.CreateDirectory(directorioCertificados);
        var fileName = $"cert_{DateTime.UtcNow:yyyyMMddHHmmssfff}_{Guid.NewGuid():N}.pfx";
        var rutaCompleta = Path.Combine(directorioCertificados, fileName);
        System.IO.File.WriteAllBytes(rutaCompleta, bytesPfx);
        return rutaCompleta;
    }

    private static bool TryObtenerBytesDesdeBase64(string valor, out byte[] bytes)
    {
        bytes = Array.Empty<byte>();
        var candidato = valor.Trim();

        const string marcadorBase64 = "base64,";
        var indiceMarcador = candidato.IndexOf(marcadorBase64, StringComparison.OrdinalIgnoreCase);
        if (candidato.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && indiceMarcador >= 0)
        {
            candidato = candidato[(indiceMarcador + marcadorBase64.Length)..];
        }

        candidato = candidato.Replace("\r", string.Empty).Replace("\n", string.Empty).Trim();

        if (candidato.EndsWith(".pfx", StringComparison.OrdinalIgnoreCase) ||
            candidato.EndsWith(".p12", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (candidato.Length < 128 || !EsCadenaBase64(candidato))
        {
            return false;
        }

        try
        {
            if (candidato.Length % 4 != 0)
            {
                candidato = candidato.PadRight(candidato.Length + (4 - (candidato.Length % 4)), '=');
            }

            bytes = Convert.FromBase64String(candidato);
            return bytes.Length > 0;
        }
        catch
        {
            return false;
        }
    }

    private static bool EsCadenaBase64(string value)
    {
        for (int i = 0; i < value.Length; i++)
        {
            var ch = value[i];
            if ((ch >= 'A' && ch <= 'Z') ||
                (ch >= 'a' && ch <= 'z') ||
                (ch >= '0' && ch <= '9') ||
                ch == '+' || ch == '/' || ch == '=')
            {
                continue;
            }
            return false;
        }
        return true;
    }

    private static string ObtenerDirectorioCertificados()
    {
        var configurado = Environment.GetEnvironmentVariable("CPE_PFX_DIRECTORY");
        if (!string.IsNullOrWhiteSpace(configurado))
        {
            return configurado.Trim();
        }

        var preferido = @"D:\CPE\FIRMABETA";
        try
        {
            Directory.CreateDirectory(preferido);
            return preferido;
        }
        catch
        {
            var fallback = Path.Combine(AppContext.BaseDirectory, "legacy-cpe", "FIRMABETA");
            Directory.CreateDirectory(fallback);
            return fallback;
        }
    }

    private sealed class TributacionDocumento
    {
        public decimal Total { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Igv { get; set; }
        public decimal Gravada { get; set; }
    }

    private sealed class ClienteDocumentoInfo
    {
        public string ClienteRazon { get; set; } = string.Empty;
        public string ClienteRuc { get; set; } = string.Empty;
        public string ClienteDni { get; set; } = string.Empty;
        public string DireccionFiscal { get; set; } = string.Empty;
    }

    private sealed class NotaCreditoOrigenBd
    {
        public long DocuId { get; set; }
        public string TipoCodigo { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public int CompaniaId { get; set; }
        public long NotaId { get; set; }
        public long ClienteId { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string FormaPago { get; set; } = string.Empty;
        public string EntidadBancaria { get; set; } = string.Empty;
        public string NroOperacion { get; set; } = string.Empty;
        public decimal Efectivo { get; set; }
        public decimal Deposito { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Igv { get; set; }
        public decimal Total { get; set; }
        public decimal Gravada { get; set; }
        public decimal Descuento { get; set; }
        public decimal Icbper { get; set; }
        public string ClienteRazon { get; set; } = string.Empty;
        public string ClienteRuc { get; set; } = string.Empty;
        public string ClienteDni { get; set; } = string.Empty;
        public string DireccionFiscal { get; set; } = string.Empty;
        public string Serie { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public List<NotaCreditoOrigenDetalleBd> Detalles { get; set; } = new();
    }

    private sealed class NotaCreditoOrigenDetalleBd
    {
        public long IdProducto { get; set; }
        public decimal Cantidad { get; set; }
        public string Um { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public decimal Importe { get; set; }
        public long DetalleNotaId { get; set; }
        public decimal ValorUm { get; set; }
        public decimal Costo { get; set; }
        public string AplicaInv { get; set; } = "S";
        public string CodigoSunat { get; set; } = string.Empty;
    }

    private sealed record UbigeoInfo(string Codigo, string Departamento, string Provincia, string Distrito);

    private static bool TryObtenerRangoDesdeData(string data, out DateTime fechaInicio, out DateTime fechaFin, out string? error)
    {
        fechaInicio = default;
        fechaFin = default;
        error = null;

        var partes = data.Split('|');
        if (partes.Length == 0 || string.IsNullOrWhiteSpace(partes[0]))
        {
            error = "Data inválido. Formato esperado: MM/dd/yyyy|MM/dd/yyyy.";
            return false;
        }

        var formatos = new[] { "MM/dd/yyyy", "M/d/yyyy", "dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd", "yyyy/MM/dd" };
        if (!DateTime.TryParseExact(
                partes[0].Trim(),
                formatos,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var fechaInicioTmp)
            && !DateTime.TryParse(partes[0].Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaInicioTmp))
        {
            error = "No se pudo interpretar la fecha inicial en Data.";
            return false;
        }

        var fechaFinTmp = fechaInicioTmp;
        if (partes.Length > 1 && !string.IsNullOrWhiteSpace(partes[1]))
        {
            if (!DateTime.TryParseExact(
                    partes[1].Trim(),
                    formatos,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out fechaFinTmp)
                && !DateTime.TryParse(partes[1].Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaFinTmp))
            {
                error = "No se pudo interpretar la fecha final en Data.";
                return false;
            }
        }

        if (fechaInicioTmp.Date > fechaFinTmp.Date)
        {
            error = "La fecha inicial no puede ser mayor que la fecha final.";
            return false;
        }

        fechaInicio = fechaInicioTmp.Date;
        fechaFin = fechaFinTmp.Date;
        return true;
    }
}

public class EnviarFacturaRequest
{
    public long? NOTA_ID { get; set; }
    public long? DOCU_ID { get; set; }
    public string? TIPO_OPERACION { get; set; }
    public string? HORA_REGISTRO { get; set; }
    public decimal? TOTAL_GRAVADAS { get; set; }
    public decimal? TOTAL_INAFECTA { get; set; }
    public decimal? TOTAL_EXONERADAS { get; set; }
    public decimal? TOTAL_GRATUITAS { get; set; }
    public decimal? TOTAL_DESCUENTO { get; set; }
    public decimal? SUB_TOTAL { get; set; }
    public decimal? POR_IGV { get; set; }
    public decimal? TOTAL_IGV { get; set; }
    public decimal? TOTAL_ISC { get; set; }
    public decimal? TOTAL_EXPORTACION { get; set; }
    public decimal? TOTAL_OTR_IMP { get; set; }
    public decimal? TOTAL_ICBPER { get; set; }
    public decimal? TOTAL { get; set; }
    public string? TOTAL_LETRAS { get; set; }
    public string? NRO_GUIA_REMISION { get; set; }
    public string? FECHA_GUIA_REMISION { get; set; }
    public string? COD_GUIA_REMISION { get; set; }
    public string? NRO_OTR_COMPROBANTE { get; set; }
    public string? COD_OTR_COMPROBANTE { get; set; }
    public string? TIPO_COMPROBANTE_MODIFICA { get; set; }
    public string? NRO_DOCUMENTO_MODIFICA { get; set; }
    public string? COD_TIPO_MOTIVO { get; set; }
    public string? DESCRIPCION_MOTIVO { get; set; }
    public string? NRO_COMPROBANTE { get; set; }
    public string? FECHA_DOCUMENTO { get; set; }
    public string? COD_TIPO_DOCUMENTO { get; set; }
    public string? COD_MONEDA { get; set; }
    public string? NRO_DOCUMENTO_CLIENTE { get; set; }
    public string? RAZON_SOCIAL_CLIENTE { get; set; }
    public string? TIPO_DOCUMENTO_CLIENTE { get; set; }
    public string? DIRECCION_CLIENTE { get; set; }
    public string? CIUDAD_CLIENTE { get; set; }
    public string? COD_PAIS_CLIENTE { get; set; }
    public string? COD_UBIGEO_CLIENTE { get; set; }
    public string? DEPARTAMENTO_CLIENTE { get; set; }
    public string? PROVINCIA_CLIENTE { get; set; }
    public string? DISTRITO_CLIENTE { get; set; }
    public string? NRO_DOCUMENTO_EMPRESA { get; set; }
    public string? TIPO_DOCUMENTO_EMPRESA { get; set; }
    public string? NOMBRE_COMERCIAL_EMPRESA { get; set; }
    public string? CODIGO_UBIGEO_EMPRESA { get; set; }
    public string? DIRECCION_EMPRESA { get; set; }
    public string? CONTACTO_EMPRESA { get; set; }
    public string? DEPARTAMENTO_EMPRESA { get; set; }
    public string? PROVINCIA_EMPRESA { get; set; }
    public string? DISTRITO_EMPRESA { get; set; }
    public string? CODIGO_PAIS_EMPRESA { get; set; }
    public string? RAZON_SOCIAL_EMPRESA { get; set; }
    public string? USUARIO_SOL_EMPRESA { get; set; }
    public string? PASS_SOL_EMPRESA { get; set; }
    public string? CONTRA_FIRMA { get; set; }
    public JsonElement TIPO_PROCESO { get; set; }
    public string? FECHA_VTO { get; set; }
    public string? FORMA_PAGO { get; set; }
    public string? GLOSA { get; set; }
    public string? RUTA_PFX { get; set; }
    public string? CODIGO_ANEXO { get; set; }
    public string? CUENTA_DETRACCION { get; set; }
    public decimal? MONTO_DETRACCION { get; set; }
    public decimal? PORCENTAJE_DES { get; set; }
    public string? LISTA_ORDEN_NC { get; set; }
    public List<EnviarFacturaDetalleRequest> detalle { get; set; } = new();
}

public class EnviarFacturaDetalleRequest
{
    public int? item { get; set; }
    public string? unidadMedida { get; set; }
    public decimal? cantidad { get; set; }
    public decimal? precio { get; set; }
    public decimal? importe { get; set; }
    public double? impuestoIcbper { get; set; }
    public int? cantidadBolsas { get; set; }
    public double? sunatIcbper { get; set; }
    public string? precioTipoCodigo { get; set; }
    public decimal? igv { get; set; }
    public decimal? biIsc { get; set; }
    public decimal? porIsc { get; set; }
    public string? tipoIsc { get; set; }
    public decimal? isc { get; set; }
    public string? codTipoOperacion { get; set; }
    public string? codigo { get; set; }
    public string? codigoSunat { get; set; }
    public string? descripcion { get; set; }
    public decimal? descuento { get; set; }
    public decimal? subTotal { get; set; }
    public decimal? precioSinImpuesto { get; set; }
}

public class NotaPedidoConDetalleRequest
{
    public NotaPedido? Nota { get; set; }
    public List<DetalleNota>? Detalles { get; set; }
}

public class AnularDocumentoRequest
{
    public string? ListaOrden { get; set; }
    public string? Data { get; set; }
}

public class AnularBoletaIndividualRequest
{
    public long? DOCU_ID { get; set; }
    public string? NRO_DOCUMENTO_MODIFICA { get; set; }
    public string? TICKET_REFERENCIA { get; set; }
    public string? DESCRIPCION_MOTIVO { get; set; }
    public string? FECHA_DOCUMENTO { get; set; }
}

public class ListaDocumentosRequest
{
    public string Data { get; set; } = string.Empty;
}

public class ListaBajasRequest
{
    public string Data { get; set; } = string.Empty;
}

public class RegistrarResumenBoletasRequest
{
    public string? ListaOrden { get; set; }
    public string? Data { get; set; }
}

public class LdDocumentosLegacyRequest
{
    public string Data { get; set; } = string.Empty;
}

public class ConsultarResumenTicketRequest
{
    public long? RESUMEN_ID { get; set; }
    public string? TICKET { get; set; }
    public string? CODIGO_SUNAT { get; set; }
    public string? MENSAJE_SUNAT { get; set; }
    public string? ESTADO { get; set; }
    public string? SECUENCIA { get; set; }
    public string? RUC { get; set; }
    public string? USUARIO_SOL_EMPRESA { get; set; }
    public string? PASS_SOL_EMPRESA { get; set; }
    public string? TIPO_DOCUMENTO { get; set; }
    public JsonElement TIPO_PROCESO { get; set; }
    public int? INTENTOS { get; set; }
}

public class GuardarCredencialesSunatRequest
{
    public int CompaniaId { get; set; }
    public string UsuarioSOL { get; set; } = string.Empty;
    public string ClaveSOL { get; set; } = string.Empty;
    public IFormFile? Certificado { get; set; }
    public string ClaveCertificado { get; set; } = string.Empty;
    public int Entorno { get; set; }
}

public class EnviarResumenBoletasRequest
{
    public string? NRO_DOCUMENTO_EMPRESA { get; set; }
    public string? RAZON_SOCIAL { get; set; }
    public string? TIPO_DOCUMENTO { get; set; }
    public string? CODIGO { get; set; }
    public string? SERIE { get; set; }
    public string? SECUENCIA { get; set; }
    public string? FECHA_REFERENCIA { get; set; }
    public string? FECHA_DOCUMENTO { get; set; }
    public JsonElement TIPO_PROCESO { get; set; }
    public string? CONTRA_FIRMA { get; set; }
    public string? USUARIO_SOL_EMPRESA { get; set; }
    public string? USUARIO { get; set; }
    public string? PASS_SOL_EMPRESA { get; set; }
    public string? RUTA_PFX { get; set; }
    public int? COMPANIA_ID { get; set; }
    public string? RANGO_NUMEROS { get; set; }
    public decimal? SUBTOTAL { get; set; }
    public decimal? IGV { get; set; }
    public decimal? ICBPER { get; set; }
    public decimal? TOTAL { get; set; }
    public int? STATUS { get; set; }
    public List<EnviarResumenBoletasDetalleRequest> detalle { get; set; } = new();
}

public class EnviarResumenBoletasDetalleRequest
{
    public int? item { get; set; }
    public string? tipoComprobante { get; set; }
    public string? nroComprobante { get; set; }
    public string? serie { get; set; }
    public string? numero { get; set; }
    public string? descripcion { get; set; }
    public string? tipoDocumento { get; set; }
    public string? nroDocumento { get; set; }
    public string? tipoComprobanteRef { get; set; }
    public string? nroComprobanteRef { get; set; }
    public string? statu { get; set; }
    public string? codMoneda { get; set; }
    public decimal? total { get; set; }
    public decimal? icbper { get; set; }
    public decimal? gravada { get; set; }
    public decimal? isc { get; set; }
    public decimal? igv { get; set; }
    public decimal? otros { get; set; }
    public int? cargoXAsignacion { get; set; }
    public decimal? montoCargoXAsig { get; set; }
    public decimal? exonerado { get; set; }
    public decimal? inafecto { get; set; }
    public decimal? exportacion { get; set; }
    public decimal? gratuitas { get; set; }
    public int? docuId { get; set; }
    public int? notaId { get; set; }
}
