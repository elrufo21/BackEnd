using System.Net;
using Ecommerce.Application.Contracts.Companias;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CompaniaController : ControllerBase
{
    private readonly ICompania _mediator;

    public CompaniaController(ICompania mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpPost("register", Name = "RegisterCompania")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> RegisterCompania([FromBody] Compania compania, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.InsertarAsync(compania, cancellationToken));
    }

    [Authorize]
    [HttpPut("{id}", Name = "EditarCompania")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> EditarCompania(int id, [FromBody] Compania compania, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.EditarAsync(id, compania, cancellationToken));
    }

    [Authorize]
    [HttpDelete("{id}", Name = "EliminarCompania")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> EliminarCompania(int id, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.EliminarAsync(id, cancellationToken));
    }

    [AllowAnonymous]
    [HttpGet("list", Name = "GetCompaniaList")]
    [ProducesResponseType(typeof(IReadOnlyList<Compania>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<Compania>>> GetCompaniaList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.ListarAsync(page, pageSize, cancellationToken));
    }

    [AllowAnonymous]
    [HttpGet("combo", Name = "GetCompaniaCombo")]
    [ProducesResponseType(typeof(IReadOnlyList<EGeneral>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<EGeneral>>> GetCompaniaCombo(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.ListarComboAsync(page, pageSize, cancellationToken));
    }
}
