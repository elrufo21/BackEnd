using System.Net;
using Ecommerce.Application.Contracts.Lineas;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LineaController : ControllerBase
{
    private readonly ILinea _mediator;

    public LineaController(ILinea mediador)
    {
        _mediator = mediador;
    }

    [AllowAnonymous]
    [HttpPost("registerlinea", Name = "Registerlinea")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult Registerlinea([FromBody] Linea linea)
    {
        return Ok(_mediator.Insertar(linea));
    }

    [AllowAnonymous]
    [HttpPut("{id}", Name = "EditarLinea")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult EditarLinea(int id, [FromBody] Linea linea)
    {
        return Ok(_mediator.Editar(id, linea));
    }

    [AllowAnonymous]
    [HttpDelete("{id}", Name = "EliminarLinea")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult Eliminarlinea(int id)
    {
        return Ok(_mediator.Eliminar(id));
    }
    [AllowAnonymous]
    [HttpGet("list", Name = "GetLineaList")]
    [ProducesResponseType(typeof(IReadOnlyList<EGeneral>), (int)HttpStatusCode.OK)]
    public ActionResult<IReadOnlyList<EGeneral>> GetLineaList()
    {
        return Ok(_mediator.Listar());
    }
}
