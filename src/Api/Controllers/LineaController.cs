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

    [Authorize]
    [HttpPost("registerlinea", Name = "Registerlinea")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Registerlinea([FromBody] Linea linea, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.InsertarAsync(linea, cancellationToken));
    }
    
    [Authorize]
    [HttpDelete("{id}", Name = "EliminarLinea")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Eliminarlinea(int id, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.EliminarAsync(id, cancellationToken));
    }
    [AllowAnonymous]
    [HttpGet("list", Name = "GetLineaList")]
    [ProducesResponseType(typeof(IReadOnlyList<EGeneral>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<EGeneral>>> GetLineaList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.ListarAsync(page, pageSize, cancellationToken));
    }
}
