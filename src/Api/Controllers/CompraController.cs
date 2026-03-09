using System.Collections.Generic;
using System.Net;
using Ecommerce.Application.Contracts.Compras;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CompraController : ControllerBase
{
    private readonly ICompra _mediator;

    public CompraController(ICompra mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpGet("crud", Name = "GetCompraCrud")]
    [ProducesResponseType(typeof(IReadOnlyList<Compra>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<Compra>>> ListarCompraCrud(
        [FromQuery] string? estado = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.ListarCrudAsync(estado, page, pageSize, cancellationToken));
    }

    [AllowAnonymous]
    [HttpGet("list", Name = "GetCompraList")]
    [ProducesResponseType(typeof(IReadOnlyList<Compra>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<Compra>>> ListarCompras(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.ListarCrudAsync(page: page, pageSize: pageSize, cancellationToken: cancellationToken));
    }

    [AllowAnonymous]
    [HttpGet("{id:long}", Name = "GetCompraById")]
    [ProducesResponseType(typeof(Compra), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Compra?>> ObtenerCompra(long id, CancellationToken cancellationToken)
    {
        var compra = await _mediator.ObtenerPorIdAsync(id, cancellationToken);
        if (compra is null) return NotFound();
        return Ok(compra);
    }

    [AllowAnonymous]
    [HttpGet("{id:long}/detalles", Name = "GetCompraDetalles")]
    [ProducesResponseType(typeof(IReadOnlyList<DetalleCompra>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<DetalleCompra>>> ObtenerDetalles(
        long id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.ListarDetalleAsync(id, page, pageSize, cancellationToken));
    }

    [Authorize]
    [HttpPost("register", Name = "RegisterCompra")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> RegistrarCompra([FromBody] Compra compra, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.InsertarAsync(compra, cancellationToken));
    }

    [Authorize]
    [HttpPost("register-with-detail", Name = "RegisterCompraConDetalle")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> RegistrarCompraConDetalle([FromBody] CompraConDetalleRequest request, CancellationToken cancellationToken)
    {
        if (request is null || request.Compra is null)
        {
            return BadRequest("Compra requerida.");
        }
        return Ok(await _mediator.InsertarConDetalleAsync(request.Compra, request.Detalles ?? new List<DetalleCompra>(), cancellationToken));
    }

    [Authorize]
    [HttpDelete("{id:long}", Name = "EliminarCompra")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> EliminarCompra(long id, CancellationToken cancellationToken)
    {
        var ok = await _mediator.EliminarAsync(id, cancellationToken);
        if (!ok) return NotFound();
        return Ok(ok);
    }
}

public class CompraConDetalleRequest
{
    public Compra? Compra { get; set; }
    public List<DetalleCompra>? Detalles { get; set; }
}
