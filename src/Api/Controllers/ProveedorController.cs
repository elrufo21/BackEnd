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

    [Authorize]
    [HttpPost("register", Name = "RegisterProveedor")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> RegisterProveedor([FromBody] Proveedor proveedor, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.InsertarAsync(proveedor, cancellationToken));
    }
        
    [Authorize]
    [HttpDelete("{id:long}", Name = "EliminarProveedor")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> EliminarProveedor(long id, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.EliminarAsync(id, cancellationToken));
    }

    [AllowAnonymous]
    [HttpGet("list", Name = "GetProveedorList")]
    [ProducesResponseType(typeof(IReadOnlyList<Proveedor>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<Proveedor>>> GetProveedorList(
        [FromQuery] string? estado = "ACTIVO",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.ListarAsync(estado, page, pageSize, cancellationToken));
    }

    [AllowAnonymous]
    [HttpGet("{id:long}", Name = "GetProveedorById")]
    [ProducesResponseType(typeof(Proveedor), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Proveedor?>> GetProveedorById(long id, CancellationToken cancellationToken)
    {
        var proveedor = await _mediator.ObtenerPorIdAsync(id, cancellationToken);
        if (proveedor is null) return NotFound();
        return Ok(proveedor);
    }

    [Authorize]
    [HttpPost("registerCuenta", Name = "CrearCuentaProveedor")]
    [ProducesResponseType(typeof(long), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<long>> CrearCuentaProveedor([FromBody] CuentaProveedor cuenta, CancellationToken cancellationToken)
    {
       return Ok(await _cuentaProveedor.InsertarAsync(cuenta, cancellationToken));
    }

    [AllowAnonymous]
    [HttpGet("{proveedorId:long}/cuentas", Name = "ListarCuentasProveedor")]
    [ProducesResponseType(typeof(IReadOnlyList<CuentaProveedor>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<CuentaProveedor>>> ListarCuentasProveedor(
        long proveedorId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _cuentaProveedor.ListarPorProveedorAsync(proveedorId, page, pageSize, cancellationToken));
    }

    [Authorize]
    [HttpDelete("cuentas/{cuentaId:long}", Name = "EliminarCuentaProveedor")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> EliminarCuentaProveedor(long cuentaId, CancellationToken cancellationToken)
    {
        var ok = await _cuentaProveedor.EliminarAsync(cuentaId, cancellationToken);
        if (!ok) return NotFound();
        return Ok(ok);
    }
}
