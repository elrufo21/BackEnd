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
    public ActionResult<IReadOnlyList<Compra>> ListarCompraCrud([FromQuery] string? estado = null)
    {
        return Ok(_mediator.ListarCrud(estado));
    }

    [AllowAnonymous]
    [HttpGet("list", Name = "GetCompraList")]
    [ProducesResponseType(typeof(IReadOnlyList<Compra>), (int)HttpStatusCode.OK)]
    public ActionResult<IReadOnlyList<Compra>> ListarCompras()
    {
        return Ok(_mediator.ListarCrud());
    }

    [AllowAnonymous]
    [HttpGet("{id:long}", Name = "GetCompraById")]
    [ProducesResponseType(typeof(Compra), (int)HttpStatusCode.OK)]
    public ActionResult<Compra?> ObtenerCompra(long id)
    {
        var compra = _mediator.ObtenerPorId(id);
        if (compra is null) return NotFound();
        return Ok(compra);
    }

    [AllowAnonymous]
    [HttpGet("{id:long}/detalles", Name = "GetCompraDetalles")]
    [ProducesResponseType(typeof(IReadOnlyList<DetalleCompra>), (int)HttpStatusCode.OK)]
    public ActionResult<IReadOnlyList<DetalleCompra>> ObtenerDetalles(long id)
    {
        return Ok(_mediator.ListarDetalle(id));
    }

    [AllowAnonymous]
    [HttpPost("register", Name = "RegisterCompra")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult RegistrarCompra([FromBody] Compra compra)
    {
        return Ok(_mediator.Insertar(compra));
    }

    [AllowAnonymous]
    [HttpPost("register-with-detail", Name = "RegisterCompraConDetalle")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult RegistrarCompraConDetalle([FromBody] CompraConDetalleRequest request)
    {
        if (request is null || request.Compra is null)
        {
            return BadRequest("Compra requerida.");
        }
        return Ok(_mediator.InsertarConDetalle(request.Compra, request.Detalles ?? new List<DetalleCompra>()));
    }

    [AllowAnonymous]
    [HttpDelete("{id:long}", Name = "EliminarCompra")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult EliminarCompra(long id)
    {
        var ok = _mediator.Eliminar(id);
        if (!ok) return NotFound();
        return Ok(ok);
    }
}

public class CompraConDetalleRequest
{
    public Compra? Compra { get; set; }
    public List<DetalleCompra>? Detalles { get; set; }
}
