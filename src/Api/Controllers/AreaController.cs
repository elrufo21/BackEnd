using System.Net;
using Ecommerce.Application.Contracts.Areas;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AreaController : ControllerBase
{
    private readonly IArea _mediator;

    public AreaController(IArea mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost("registerarea", Name = "RegisterArea")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult RegisterArea([FromBody] Area area)
    {
        return Ok(_mediator.Insertar(area));
    }

    [AllowAnonymous]
    [HttpPut("{id}", Name = "EditarArea")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult EditarArea(int id, [FromBody] Area area)
    {
        return Ok(_mediator.Editar(id, area));
    }

    [AllowAnonymous]
    [HttpDelete("{id}", Name = "EliminarArea")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult EliminarArea(int id)
    {
        return Ok(_mediator.Eliminar(id));
    }

    [AllowAnonymous]
    [HttpGet("list", Name = "GetAreaList")]
    [ProducesResponseType(typeof(IReadOnlyList<EGeneral>), (int)HttpStatusCode.OK)]
    public ActionResult<IReadOnlyList<EGeneral>> GetAreaList()
    {
        return Ok(_mediator.Listar());
    }
}
