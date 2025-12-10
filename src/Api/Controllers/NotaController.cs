using System.Net;
using Ecommerce.Application.Contracts.NotaPedido;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Newtonsoft.Json.Linq;

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
    [HttpPost("crearOrden", Name = "CrearOrden")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult RegistrarOrden(JsonElement body)
    {
        string json = JsonSerializer.Serialize(body);
        dynamic res = JObject.Parse(json);
        dynamic producto = (JObject)res["requestDetalle"];

        //Console.WriteLine("aramirez:" + res);
        Console.WriteLine("aramirez Total:" + res["Total"].ToString());
        Console.WriteLine("aramirez Items:" + res["Items"].ToString());
        Console.WriteLine("aramirez Detalle:" + producto);

        int count = 0;
        count = res["Items"];
        decimal xtotal = 0;
        xtotal = Convert.ToDecimal(res["Total"]);
        decimal xsubtotal = 0;
        xsubtotal = Convert.ToDecimal(res["SubTotal"]);
        decimal xigv = 0;
        xigv = Convert.ToDecimal(res["IGV"]);
        string xletra = string.Empty;
        xletra = Letras.enletras(xtotal.ToString("N2")) + "  SOLES";

        /*Cabezera*/

        string xDocumento, xserie = string.Empty;
        xDocumento = res["Documento"].ToString();
        switch (xDocumento)
        {
            case "BOLETA":
                xserie = "BA01";
                break;
            case "FACTURA":
                xserie = "FA01";
                break;
            default:
                xserie = "0001";
                break;
        }
        string fila = string.Empty;
        string vdata = string.Empty;
        vdata = xDocumento + "|" +
        res["ClienteId"].ToString() + "|" + res["Usuario"].ToString() +
        "|" + res["FormaPago"].ToString() + "|ALCONTADO|||" +
        xtotal + "|0|0|" + xtotal + "|0|" + xtotal + "|0.00|0.00|" +
        xtotal + "|PENDIENTE|1|INMEDIATA|MERCADERIA|" + xserie + "|00000001|" +
        Convert.ToDecimal(res["Ganancia"].ToString("N2")) + "|" + xletra + "|0.00||PENDIENTE|" +
        xsubtotal + "|" + xigv + "|" + res["UsuarioId"].ToString() + "|0.00|" + xsubtotal + "|0[";

        for (int i = 0; i < count; i++)
        {
            fila = i.ToString();
            vdata += Convert.ToInt32(producto[fila].productId);
            vdata += "|";
            vdata += Convert.ToDecimal(producto[fila].cantidad);
            vdata += "|";
            vdata += Convert.ToString(producto[fila].unidad);
            vdata += "|";
            vdata += Convert.ToString(producto[fila].producto);
            vdata += "|";
            vdata += Convert.ToDecimal(producto[fila].costo);
            vdata += "|";
            vdata += Convert.ToDecimal(producto[fila].precio);
            vdata += "|";
            vdata += Convert.ToDecimal(producto[fila].importe);
            vdata += "|PENDIENTE|";
            vdata += Convert.ToDecimal(producto[fila].valorUM);
            vdata += "|E";
            if (i == count - 1) break;
            else vdata += ";";
        }
        vdata += "[";
        Console.WriteLine("aramirez DES: " + vdata);
        return Ok(_mediator.Insertar(vdata));
    }
}
