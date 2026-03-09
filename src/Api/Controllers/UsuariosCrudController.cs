using System.Net;
using Ecommerce.Application.Contracts.Usuarios;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UsuariosCrudController : ControllerBase
{
    private readonly IUsuariosCrud _usuariosCrud;

    public UsuariosCrudController(IUsuariosCrud usuariosCrud)
    {
        _usuariosCrud = usuariosCrud;
    }

    [Authorize]
    [HttpPost("register", Name = "RegisterUsuarioCrud")]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> RegisterUsuario([FromBody] UsuarioBd usuario, CancellationToken cancellationToken)
    {
        var id = await _usuariosCrud.InsertarAsync(usuario, cancellationToken);
        if (id == -1) return Conflict("El alias de usuario ya existe.");
        if (id == 0) return BadRequest("No se pudo crear el usuario.");
        return Ok(id);
    }

    [Authorize]
    [HttpPut("{id:int}", Name = "ActualizarUsuarioCrud")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> ActualizarUsuario(int id, [FromBody] UsuarioBd usuario, CancellationToken cancellationToken)
    {
        var actualizado = await _usuariosCrud.EditarAsync(id, usuario, cancellationToken);
        if (!actualizado) return NotFound();
        return Ok(actualizado);
    }

    [Authorize]
    [HttpDelete("{id:int}", Name = "EliminarUsuarioCrud")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> EliminarUsuario(int id, CancellationToken cancellationToken)
    {
        var eliminado = await _usuariosCrud.EliminarAsync(id, cancellationToken);
        if (!eliminado) return NotFound();
        return Ok(eliminado);
    }

    [AllowAnonymous]
    [HttpGet("list", Name = "GetUsuariosCrudList")]
    [ProducesResponseType(typeof(IReadOnlyList<UsuarioBd>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<UsuarioBd>>> GetUsuariosList(
        [FromQuery] string? estado = "ACTIVO",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _usuariosCrud.ListarAsync(estado, page, pageSize, cancellationToken));
    }

    [AllowAnonymous]
    [HttpGet("list-with-personal", Name = "GetUsuariosCrudListWithPersonal")]
    [ProducesResponseType(typeof(IReadOnlyList<UsuarioConPersonal>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<UsuarioConPersonal>>> GetUsuariosListWithPersonal(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _usuariosCrud.ListarConPersonalAsync(page, pageSize, cancellationToken));
    }

    [AllowAnonymous]
    [HttpGet("{id:int}", Name = "GetUsuarioCrudById")]
    [ProducesResponseType(typeof(UsuarioBd), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<UsuarioBd?>> GetUsuarioById(int id, CancellationToken cancellationToken)
    {
        var usuario = await _usuariosCrud.ObtenerPorIdAsync(id, cancellationToken);
        if (usuario is null) return NotFound();
        return Ok(usuario);
    }

    [AllowAnonymous]
    [HttpGet("{id:int}/with-personal", Name = "GetUsuarioCrudByIdWithPersonal")]
    [ProducesResponseType(typeof(UsuarioConPersonal), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<UsuarioConPersonal?>> GetUsuarioByIdWithPersonal(int id, CancellationToken cancellationToken)
    {
        var usuario = await _usuariosCrud.ObtenerPorIdConPersonalAsync(id, cancellationToken);
        if (usuario is null) return NotFound();
        return Ok(usuario);
    }
}
