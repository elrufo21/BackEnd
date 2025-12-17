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

    [AllowAnonymous]
    [HttpPost("registermaquina", Name = "RegisterMaquina")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult RegisterMaquina([FromBody] Maquina maquina)
    {
        return Ok(_mediator.Insertar(maquina));
    }
    
    [AllowAnonymous]
    [HttpDelete("{id}", Name = "EliminarMaquina")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult EliminarMaquina(int id)
    {
        return Ok(_mediator.Eliminar(id));
    }

    [AllowAnonymous]
    [HttpGet("list", Name = "GetMaquinaList")]
    [ProducesResponseType(typeof(IReadOnlyList<Maquina>), (int)HttpStatusCode.OK)]
    public ActionResult<IReadOnlyList<Maquina>> GetMaquinaList()
    {
        return Ok(_mediator.Listar());
    }
}
