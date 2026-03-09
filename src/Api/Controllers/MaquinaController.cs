using System.Net;
using Ecommerce.Application.Contracts.Maquinas;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class MaquinaController : ControllerBase
{
    private readonly IMaquina _mediator;

    public MaquinaController(IMaquina mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpPost("registermaquina", Name = "RegisterMaquina")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> RegisterMaquina([FromBody] Maquina maquina, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.InsertarAsync(maquina, cancellationToken));
    }
    
    [Authorize]
    [HttpDelete("{id}", Name = "EliminarMaquina")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> EliminarMaquina(int id, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.EliminarAsync(id, cancellationToken));
    }

    [AllowAnonymous]
    [HttpGet("list", Name = "GetMaquinaList")]
    [ProducesResponseType(typeof(IReadOnlyList<Maquina>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<Maquina>>> GetMaquinaList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.ListarAsync(page, pageSize, cancellationToken));
    }
}
