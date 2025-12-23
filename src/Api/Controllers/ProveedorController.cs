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
    private readonly ICuentaProveedor _cuentaProveedor;

    public ProveedorController(IProveedor mediator, ICuentaProveedor cuentaProveedor)
    {
        _mediator = mediator;
        _cuentaProveedor = cuentaProveedor;
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

    [AllowAnonymous]
    [HttpPost("{proveedorId:long}/cuentas", Name = "CrearCuentaProveedor")]
    [ProducesResponseType(typeof(long), (int)HttpStatusCode.OK)]
    public ActionResult<long> CrearCuentaProveedor(long proveedorId, [FromBody] CuentaProveedor cuenta)
    {
        cuenta.ProveedorId = proveedorId;
        var id = _cuentaProveedor.Insertar(cuenta);
        if (id == 0) return BadRequest("No se pudo crear la cuenta.");
        return Ok(id);
    }

    [AllowAnonymous]
    [HttpGet("{proveedorId:long}/cuentas", Name = "ListarCuentasProveedor")]
    [ProducesResponseType(typeof(IReadOnlyList<CuentaProveedor>), (int)HttpStatusCode.OK)]
    public ActionResult<IReadOnlyList<CuentaProveedor>> ListarCuentasProveedor(long proveedorId)
    {
        return Ok(_cuentaProveedor.ListarPorProveedor(proveedorId));
    }

    [AllowAnonymous]
    [HttpPut("{proveedorId:long}/cuentas/{cuentaId:long}", Name = "ActualizarCuentaProveedor")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult ActualizarCuentaProveedor(long proveedorId, long cuentaId, [FromBody] CuentaProveedor cuenta)
    {
        cuenta.ProveedorId = proveedorId;
        var ok = _cuentaProveedor.Actualizar(proveedorId, cuentaId, cuenta);
        if (!ok) return NotFound();
        return Ok(ok);
    }

    [AllowAnonymous]
    [HttpDelete("{proveedorId:long}/cuentas/{cuentaId:long}", Name = "EliminarCuentaProveedor")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult EliminarCuentaProveedor(long proveedorId, long cuentaId)
    {
        var ok = _cuentaProveedor.Eliminar(proveedorId, cuentaId);
        if (!ok) return NotFound();
        return Ok(ok);
    }
}
