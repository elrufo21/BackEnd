using System.Net;
using Ecommerce.Application.Contracts.Feriados;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class FeriadosController : ControllerBase
{
    private readonly IFeriado _mediator;

    public FeriadosController(IFeriado mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpPost("register", Name = "RegisterFeriado")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> RegisterFeriado([FromBody] Feriado feriado, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.InsertarAsync(feriado, cancellationToken));
    }

    [Authorize]
    [HttpDelete("{id:int}", Name = "EliminarFeriado")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> EliminarFeriado(int id, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.EliminarAsync(id, cancellationToken));
    }

    [AllowAnonymous]
    [HttpGet("list", Name = "GetFeriadoList")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<string>> GetFeriadoList(CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.ListarAsync(cancellationToken));
    }

    [AllowAnonymous]
    [HttpGet("{id:int}", Name = "GetFeriadoById")]
    [ProducesResponseType(typeof(Feriado), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Feriado?>> GetFeriadoById(int id, CancellationToken cancellationToken)
    {
        var feriado = await _mediator.ObtenerPorIdAsync(id, cancellationToken);
        if (feriado is null) return NotFound();
        return Ok(feriado);
    }
}
