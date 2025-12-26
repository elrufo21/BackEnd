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

    [AllowAnonymous]
    [HttpPost("register", Name = "RegisterUsuarioCrud")]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    public IActionResult RegisterUsuario([FromBody] UsuarioBd usuario)
    {
        var id = _usuariosCrud.Insertar(usuario);
        if (id == -1) return Conflict("El alias de usuario ya existe.");
        if (id == 0) return BadRequest("No se pudo crear el usuario.");
        return Ok(id);
    }

    [AllowAnonymous]
    [HttpPut("{id:int}", Name = "ActualizarUsuarioCrud")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult ActualizarUsuario(int id, [FromBody] UsuarioBd usuario)
    {
        var actualizado = _usuariosCrud.Editar(id, usuario);
        if (!actualizado) return NotFound();
        return Ok(actualizado);
    }

    [AllowAnonymous]
    [HttpDelete("{id:int}", Name = "EliminarUsuarioCrud")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult EliminarUsuario(int id)
    {
        var eliminado = _usuariosCrud.Eliminar(id);
        if (!eliminado) return NotFound();
        return Ok(eliminado);
    }

    [AllowAnonymous]
    [HttpGet("list", Name = "GetUsuariosCrudList")]
    [ProducesResponseType(typeof(IReadOnlyList<UsuarioBd>), (int)HttpStatusCode.OK)]
    public ActionResult<IReadOnlyList<UsuarioBd>> GetUsuariosList([FromQuery] string? estado = "ACTIVO")
    {
        return Ok(_usuariosCrud.Listar(estado));
    }

    [AllowAnonymous]
    [HttpGet("list-with-personal", Name = "GetUsuariosCrudListWithPersonal")]
    [ProducesResponseType(typeof(IReadOnlyList<UsuarioConPersonal>), (int)HttpStatusCode.OK)]
    public ActionResult<IReadOnlyList<UsuarioConPersonal>> GetUsuariosListWithPersonal()
    {
        return Ok(_usuariosCrud.ListarConPersonal());
    }

    [AllowAnonymous]
    [HttpGet("{id:int}", Name = "GetUsuarioCrudById")]
    [ProducesResponseType(typeof(UsuarioBd), (int)HttpStatusCode.OK)]
    public ActionResult<UsuarioBd?> GetUsuarioById(int id)
    {
        var usuario = _usuariosCrud.ObtenerPorId(id);
        if (usuario is null) return NotFound();
        return Ok(usuario);
    }

    [AllowAnonymous]
    [HttpGet("{id:int}/with-personal", Name = "GetUsuarioCrudByIdWithPersonal")]
    [ProducesResponseType(typeof(UsuarioConPersonal), (int)HttpStatusCode.OK)]
    public ActionResult<UsuarioConPersonal?> GetUsuarioByIdWithPersonal(int id)
    {
        var usuario = _usuariosCrud.ObtenerPorIdConPersonal(id);
        if (usuario is null) return NotFound();
        return Ok(usuario);
    }
}
