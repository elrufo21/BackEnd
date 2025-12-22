using System.Net;
using Ecommerce.Application.Contracts.Proveedores;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProveedorController : ControllerBase
{
    private readonly IProveedor _mediator;

    public ProveedorController(IProveedor mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost("register", Name = "RegisterProveedor")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult RegisterProveedor([FromBody] Proveedor proveedor)
    {
        return Ok(_mediator.Insertar(proveedor));
    }
    
    [AllowAnonymous]
    [HttpDelete("{id:long}", Name = "EliminarProveedor")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult EliminarProveedor(long id)
    {
        return Ok(_mediator.Eliminar(id));
    }

    [AllowAnonymous]
    [HttpGet("list", Name = "GetProveedorList")]
    [ProducesResponseType(typeof(IReadOnlyList<Proveedor>), (int)HttpStatusCode.OK)]
    public ActionResult<IReadOnlyList<Proveedor>> GetProveedorList()
    {
        return Ok(_mediator.Listar());
    }

    [AllowAnonymous]
    [HttpGet("{id:long}", Name = "GetProveedorById")]
    [ProducesResponseType(typeof(Proveedor), (int)HttpStatusCode.OK)]
    public ActionResult<Proveedor?> GetProveedorById(long id)
    {
        var proveedor = _mediator.ObtenerPorId(id);
        if (proveedor is null) return NotFound();
        return Ok(proveedor);
    }
}
