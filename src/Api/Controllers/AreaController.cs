using System.Net;
using Ecommerce.Application.Contracts.Areas;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AreaController : ControllerBase
{
    private readonly IArea _mediator;

    public AreaController(IArea mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpPost("registerarea", Name = "RegisterArea")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> RegisterArea([FromBody] Area area, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.InsertarAsync(area, cancellationToken));
    }

    [Authorize]
    [HttpDelete("{id}", Name = "EliminarArea")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> EliminarArea(int id, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.EliminarAsync(id, cancellationToken));
    }

    [AllowAnonymous]
    [HttpGet("list", Name = "GetAreaList")]
    [ProducesResponseType(typeof(IReadOnlyList<EGeneral>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<EGeneral>>> GetAreaList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.ListarAsync(page, pageSize, cancellationToken));
    }
}
