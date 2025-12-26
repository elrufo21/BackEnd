using System.Net;
using Ecommerce.Application.Contracts.Feriados;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class FeriadosController : ControllerBase
{
    private readonly IFeriado _mediator;

    public FeriadosController(IFeriado mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost("register", Name = "RegisterFeriado")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult RegisterFeriado([FromBody] Feriado feriado)
    {
        return Ok(_mediator.Insertar(feriado));
    }

    [AllowAnonymous]
    [HttpDelete("{id:int}", Name = "EliminarFeriado")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult EliminarFeriado(int id)
    {
        return Ok(_mediator.Eliminar(id));
    }

    [AllowAnonymous]
    [HttpGet("list", Name = "GetFeriadoList")]
    [ProducesResponseType(typeof(IReadOnlyList<Feriado>), (int)HttpStatusCode.OK)]
    public ActionResult<IReadOnlyList<Feriado>> GetFeriadoList()
    {
        return Ok(_mediator.Listar());
    }

    [AllowAnonymous]
    [HttpGet("{id:int}", Name = "GetFeriadoById")]
    [ProducesResponseType(typeof(Feriado), (int)HttpStatusCode.OK)]
    public ActionResult<Feriado?> GetFeriadoById(int id)
    {
        var feriado = _mediator.ObtenerPorId(id);
        if (feriado is null) return NotFound();
        return Ok(feriado);
    }
}
