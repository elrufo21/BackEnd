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

    private const long MaxCertificateSizeBytes = 2 * 1024 * 1024; // 2 MB
    private const string CodigoSunatFacturaFallback = "50161509";
    private const string CodigoSunatNotaCreditoFallback = "01010101";
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
            var envioOk = string.Equals(ObtenerValorLegacy(respuestaLegacy, "flg_rta", "0"), "1", StringComparison.Ordinal);

            object? registroBd;
            if (envioOk)
            {
                registroBd = await RegistrarResumenEnBaseDatosAsync(requestBaja, respuestaLegacy, cancellationToken);
            }
            else
            {
                registroBd = new
                {
                    ok = false,
                    mensaje = "No se registró en BD porque el envío de baja a OCE/SUNAT no fue exitoso."
                };
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
            var envioOk = string.Equals(ObtenerValorLegacy(respuestaLegacy, "flg_rta", "0"), "1", StringComparison.Ordinal);

            object? registroBd;
            if (envioOk)
            {
                registroBd = await RegistrarResumenEnBaseDatosAsync(request, respuestaLegacy, cancellationToken);
            }
            else
            {
                registroBd = new
                {
                    ok = false,
                    mensaje = "No se registró en BD porque el envío a OCE/SUNAT no fue exitoso."
                };
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
            var vdataNota = BuildOrdenPayload(request.Nota, detalles);
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
            var vdataNota = BuildOrdenPayload(request.Nota, detalles);
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

            if (!item.precioSinImpuesto.HasValue || item.precioSinImpuesto < 0)
            {
                errores.Add($"detalle[{i}].precioSinImpuesto es requerido y debe ser mayor o igual a 0.");
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

            if (!item.precioSinImpuesto.HasValue || item.precioSinImpuesto < 0)
            {
                errores.Add($"detalle[{i}].precioSinImpuesto es requerido y debe ser mayor o igual a 0.");
            }

            AgregarErrorSiVacio(errores, item.descripcion, $"detalle[{i}].descripcion es requerido.");
            AgregarErrorSiVacio(errores, item.codigoSunat, $"detalle[{i}].codigoSunat es requerido.");
            AgregarErrorSiVacio(errores, item.codTipoOperacion, $"detalle[{i}].codTipoOperacion es requerido.");
        }

        return errores;
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
        request.HORA_REGISTRO = string.IsNullOrWhiteSpace(request.HORA_REGISTRO)
            ? DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture)
            : request.HORA_REGISTRO.Trim();
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
            request.detalle[i] = NormalizarDetalleFactura(request.detalle[i], i + 1, request.POR_IGV ?? 18m);
        }

        request.SUB_TOTAL ??= request.detalle.Sum(x => x.importe ?? 0m);
        request.TOTAL_IGV ??= request.detalle.Sum(x => x.igv ?? 0m);
        request.TOTAL_ISC ??= request.detalle.Sum(x => x.isc ?? 0m);
        request.TOTAL_ICBPER ??= request.detalle.Sum(x => Convert.ToDecimal(x.impuestoIcbper ?? 0d));
        request.TOTAL_OTR_IMP ??= 0m;
        request.TOTAL_GRAVADAS ??= request.detalle
            .Where(x => string.Equals(x.codTipoOperacion, "10", StringComparison.Ordinal))
            .Sum(x => x.importe ?? 0m);
        request.TOTAL_EXONERADAS ??= request.detalle
            .Where(x => string.Equals(x.codTipoOperacion, "20", StringComparison.Ordinal))
            .Sum(x => x.importe ?? 0m);
        request.TOTAL_INAFECTA ??= request.detalle
            .Where(x => string.Equals(x.codTipoOperacion, "30", StringComparison.Ordinal))
            .Sum(x => x.importe ?? 0m);
        request.TOTAL_EXPORTACION ??= request.detalle
            .Where(x => string.Equals(x.codTipoOperacion, "40", StringComparison.Ordinal))
            .Sum(x => x.importe ?? 0m);
        request.TOTAL_GRATUITAS ??= 0m;
        request.TOTAL_DESCUENTO ??= request.detalle.Sum(x => x.descuento ?? 0m);
        request.POR_IGV ??= 18m;
        request.TOTAL ??= (request.SUB_TOTAL ?? 0m)
            + (request.TOTAL_IGV ?? 0m)
            + (request.TOTAL_ISC ?? 0m)
            + (request.TOTAL_ICBPER ?? 0m)
            + (request.TOTAL_OTR_IMP ?? 0m);

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
        request.HORA_REGISTRO = string.IsNullOrWhiteSpace(request.HORA_REGISTRO)
            ? DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture)
            : request.HORA_REGISTRO.Trim();
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
            request.detalle[i] = NormalizarDetalleFactura(request.detalle[i], i + 1, request.POR_IGV ?? 18m);
        }

        request.SUB_TOTAL ??= request.detalle.Sum(x => x.importe ?? 0m);
        request.TOTAL_IGV ??= request.detalle.Sum(x => x.igv ?? 0m);
        request.TOTAL_ISC ??= request.detalle.Sum(x => x.isc ?? 0m);
        request.TOTAL_ICBPER ??= request.detalle.Sum(x => Convert.ToDecimal(x.impuestoIcbper ?? 0d));
        request.TOTAL_OTR_IMP ??= 0m;
        request.TOTAL_GRAVADAS ??= request.detalle
            .Where(x => string.Equals(x.codTipoOperacion, "10", StringComparison.Ordinal))
            .Sum(x => x.importe ?? 0m);
        request.TOTAL_EXONERADAS ??= request.detalle
            .Where(x => string.Equals(x.codTipoOperacion, "20", StringComparison.Ordinal))
            .Sum(x => x.importe ?? 0m);
        request.TOTAL_INAFECTA ??= request.detalle
            .Where(x => string.Equals(x.codTipoOperacion, "30", StringComparison.Ordinal))
            .Sum(x => x.importe ?? 0m);
        request.TOTAL_EXPORTACION ??= request.detalle
            .Where(x => string.Equals(x.codTipoOperacion, "40", StringComparison.Ordinal))
            .Sum(x => x.importe ?? 0m);
        request.TOTAL_GRATUITAS ??= 0m;
        request.TOTAL_DESCUENTO ??= request.detalle.Sum(x => x.descuento ?? 0m);
        request.POR_IGV ??= 18m;
        request.TOTAL ??= (request.SUB_TOTAL ?? 0m)
            + (request.TOTAL_IGV ?? 0m)
            + (request.TOTAL_ISC ?? 0m)
            + (request.TOTAL_ICBPER ?? 0m)
            + (request.TOTAL_OTR_IMP ?? 0m);

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

    private static EnviarFacturaDetalleRequest NormalizarDetalleFactura(EnviarFacturaDetalleRequest? item, int itemIndex, decimal porcentajeIgv)
    {
        item ??= new EnviarFacturaDetalleRequest();
        item.item ??= itemIndex;
        item.unidadMedida = string.IsNullOrWhiteSpace(item.unidadMedida) ? "NIU" : item.unidadMedida.Trim().ToUpperInvariant();
        item.precioTipoCodigo = string.IsNullOrWhiteSpace(item.precioTipoCodigo) ? "01" : item.precioTipoCodigo.Trim();
        item.codTipoOperacion = string.IsNullOrWhiteSpace(item.codTipoOperacion) ? "10" : item.codTipoOperacion.Trim();
        item.codigo = string.IsNullOrWhiteSpace(item.codigo) ? $"ITEM{item.item}" : item.codigo.Trim();
        item.codigoSunat = item.codigoSunat?.Trim();
        item.descripcion = item.descripcion?.Trim();
        item.cantidad ??= 0m;
        item.igv ??= 0m;
        item.isc ??= 0m;
        item.descuento ??= 0m;
        item.subTotal ??= 0m;
        item.impuestoIcbper ??= 0d;
        item.cantidadBolsas ??= 0;
        item.sunatIcbper ??= 0d;
        item.tipoIsc = string.IsNullOrWhiteSpace(item.tipoIsc) ? string.Empty : item.tipoIsc.Trim();
        item.biIsc ??= item.importe ?? 0m;
        item.porIsc ??= 0m;

        if (!item.importe.HasValue && item.cantidad > 0 && item.precioSinImpuesto.HasValue)
        {
            item.importe = decimal.Round(item.cantidad.Value * item.precioSinImpuesto.Value, 2, MidpointRounding.AwayFromZero);
        }

        if (!item.precioSinImpuesto.HasValue && item.cantidad > 0 && item.importe.HasValue)
        {
            item.precioSinImpuesto = decimal.Round(item.importe.Value / item.cantidad.Value, 10, MidpointRounding.AwayFromZero);
        }

        if (!item.precio.HasValue && item.cantidad > 0)
        {
            var importe = item.importe ?? 0m;
            var igv = item.igv ?? 0m;
            item.precio = decimal.Round((importe + igv) / item.cantidad.Value, 10, MidpointRounding.AwayFromZero);
        }

        if (!item.subTotal.HasValue)
        {
            item.subTotal = item.importe ?? 0m;
        }

        if (item.igv.GetValueOrDefault() == 0m &&
            string.Equals(item.codTipoOperacion, "10", StringComparison.Ordinal) &&
            item.importe.HasValue)
        {
            item.igv = decimal.Round(item.importe.Value * (porcentajeIgv / 100m), 2, MidpointRounding.AwayFromZero);
            if (!item.precio.HasValue && item.cantidad.GetValueOrDefault() > 0)
            {
                item.precio = decimal.Round((item.importe.Value + item.igv.Value) / item.cantidad.Value, 10, MidpointRounding.AwayFromZero);
            }
        }

        return item;
    }

    private static CPE MapearFacturaLegacy(EnviarFacturaRequest request, int tipoProceso, string rutaPfxNormalizada)
    {
        var factura = new CPE
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
            COD_TIPO_DOCUMENTO = "01",
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
            factura.detalle.Add(new CPE_DETALLE
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

        return factura;
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

        if (!string.IsNullOrWhiteSpace(request.LISTA_ORDEN_NC))
        {
            try
            {
                var resultado = await _mediator.RegistrarNotaCreditoAsync(request.LISTA_ORDEN_NC.Trim(), cancellationToken);
                var ok = string.Equals(resultado?.Trim(), "true", StringComparison.OrdinalIgnoreCase);
                return new
                {
                    ok,
                    accion_bd = "uspinsertarNC",
                    resultado = string.IsNullOrWhiteSpace(resultado) ? string.Empty : resultado,
                    mensaje = ok
                        ? "SUNAT/OCE aceptó la nota de crédito y se ejecutó uspinsertarNC."
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
                return new
                {
                    ok,
                    accion_bd = "uspReEnviarNotaCredito",
                    resultado = string.IsNullOrWhiteSpace(resultado) ? string.Empty : resultado,
                    mensaje = ok
                        ? "SUNAT/OCE aceptó la nota de crédito y se ejecutó uspReEnviarNotaCredito."
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
            registroBd = new
            {
                ok = false,
                mensaje = $"SUNAT/OCE respondió con código {codSunat}. No se actualizó BD porque la nota de crédito no quedó aceptada."
            };
        }
        else
        {
            registroBd = new
            {
                ok = false,
                mensaje = "No se registró en BD porque el envío de la nota de crédito a OCE/SUNAT no fue exitoso."
            };
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
            registroBd = new
            {
                ok = false,
                mensaje = $"SUNAT/OCE respondió con código {codSunat}. No se actualizó BD porque la factura no quedó aceptada."
            };
        }
        else
        {
            registroBd = new
            {
                ok = false,
                mensaje = "No se registró en BD porque el envío de la factura a OCE/SUNAT no fue exitoso."
            };
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

        var request = new EnviarFacturaRequest
        {
            NOTA_ID = notaId,
            TIPO_OPERACION = "0101",
            HORA_REGISTRO = (nota.NotaFecha ?? DateTime.Now).ToString("HH:mm:ss", CultureInfo.InvariantCulture),
            SUB_TOTAL = nota.NotaSubtotal ?? detalles.Sum(x => x.DetalleImporte ?? 0m),
            TOTAL = nota.NotaTotal ?? 0m,
            TOTAL_IGV = DecimalMax(0m, (nota.NotaTotal ?? 0m) - (nota.NotaSubtotal ?? 0m)),
            TOTAL_ISC = 0m,
            TOTAL_ICBPER = nota.ICBPER ?? 0m,
            TOTAL_OTR_IMP = 0m,
            TOTAL_EXPORTACION = 0m,
            TOTAL_DESCUENTO = nota.NotaDescuento ?? 0m,
            TOTAL_GRATUITAS = 0m,
            TOTAL_GRAVADAS = nota.NotaSubtotal ?? detalles.Sum(x => x.DetalleImporte ?? 0m),
            TOTAL_EXONERADAS = 0m,
            TOTAL_INAFECTA = 0m,
            POR_IGV = 18m,
            TOTAL_LETRAS = Letras.enletras((nota.NotaTotal ?? 0m).ToString("N2", CultureInfo.InvariantCulture)) + " SOLES",
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

        var porcentajeIgv = request.POR_IGV ?? 18m;
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
            var importe = detalle.DetalleImporte ?? 0m;
            var precioSinImpuesto = detalle.DetallePrecio ?? (cantidad > 0 ? decimal.Round(importe / cantidad, 10, MidpointRounding.AwayFromZero) : 0m);
            var igv = decimal.Round(importe * (porcentajeIgv / 100m), 2, MidpointRounding.AwayFromZero);
            var precioConImpuesto = cantidad > 0
                ? decimal.Round((importe + igv) / cantidad, 10, MidpointRounding.AwayFromZero)
                : 0m;

            detalleFactura.Add(new EnviarFacturaDetalleRequest
            {
                item = indice,
                unidadMedida = NormalizarUnidadMedidaSunat(detalle.DetalleUm, producto?.ProductoUM),
                cantidad = cantidad,
                precio = precioConImpuesto,
                importe = importe,
                precioSinImpuesto = precioSinImpuesto,
                igv = igv,
                codTipoOperacion = "10",
                codigo = string.IsNullOrWhiteSpace(producto?.ProductoCodigo)
                    ? $"PROD{detalle.IdProducto.Value}"
                    : producto.ProductoCodigo.Trim(),
                codigoSunat = codigoSunat,
                descripcion = string.IsNullOrWhiteSpace(detalle.DetalleDescripcion)
                    ? producto?.ProductoNombre?.Trim()
                    : detalle.DetalleDescripcion.Trim(),
                descuento = 0m,
                subTotal = importe
            });

            indice++;
        }

        request.detalle = detalleFactura;
        return request;
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
            "CAJA" => "BX",
            _ => valor
        };
    }

    private static decimal DecimalMax(decimal left, decimal right)
    {
        return left >= right ? left : right;
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
        return valor.ToString("0.00", CultureInfo.InvariantCulture);
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

    private sealed record UbigeoInfo(string Codigo, string Departamento, string Provincia, string Distrito);
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
