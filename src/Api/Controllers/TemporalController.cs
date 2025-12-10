using System.Net;
using Ecommerce.Application.Contracts.TemporalVenta;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TemporalController : ControllerBase
{
    private readonly ITemporalVenta _mediator;

    public TemporalController(ITemporalVenta mediador)
    {
        _mediator = mediador;
    }
    [AllowAnonymous]
    [HttpPost("registerTemporal", Name = "RegisterTemporal")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult RegisterTemporal([FromBody] ETemporalVenta eTemporal)
    {
        return Ok(_mediator.Insertar(eTemporal));
    }

    [AllowAnonymous]
    [HttpPut("editarTemporal", Name = "editarTemporal")]
    [ProducesResponseType(typeof(IReadOnlyList<EListaTemporal>), (int)HttpStatusCode.OK)]
    public ActionResult<IReadOnlyList<EListaTemporal>> EditarTemporal([FromBody] ETemporalVenta eTemporal)
    {
        return Ok(_mediator.Editar(eTemporal));
    }

    [AllowAnonymous]
    [HttpGet("{id}", Name = "GetTemList")]
    [ProducesResponseType(typeof(IReadOnlyList<EListaTemporal>), (int)HttpStatusCode.OK)]
    public ActionResult<IReadOnlyList<EListaTemporal>> GetTempList(int id)
    {
        return Ok(_mediator.Listar(id));
    }
    [AllowAnonymous]
    [HttpDelete("item/{id}", Name = "eliminarTemporal")]
    [ProducesResponseType(typeof(IReadOnlyList<EListaTemporal>), (int)HttpStatusCode.OK)]
    public ActionResult<IReadOnlyList<EListaTemporal>> EliminarTemporal(int id)
    {
        return Ok(_mediator.Eliminar(id));
    }
    [AllowAnonymous]
    [HttpDelete("user/{id}", Name = "eliminarTodo")]
    [ProducesResponseType(typeof(IReadOnlyList<EListaTemporal>), (int)HttpStatusCode.OK)]
    public ActionResult<IReadOnlyList<EListaTemporal>> EliminarTodo(int id)
    {
        return Ok(_mediator.EliminarTodo(id));
    }
}
