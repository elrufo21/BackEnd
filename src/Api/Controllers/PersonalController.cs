using System.Net;
using Ecommerce.Application.Contracts.Personales;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PersonalController : ControllerBase
{
    private readonly IPersonal _mediator;

    public PersonalController(IPersonal mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost("registerpersonal", Name = "RegisterPersonal")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult RegisterPersonal([FromBody] Personal personal)
    {
        return Ok(_mediator.Insertar(personal));
    }

    [AllowAnonymous]
    [HttpPut("{id}", Name = "EditarPersonal")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult EditarPersonal(long id, [FromBody] Personal personal)
    {
        return Ok(_mediator.Editar(id, personal));
    }

    [AllowAnonymous]
    [HttpDelete("{id}", Name = "EliminarPersonal")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult EliminarPersonal(long id)
    {
        return Ok(_mediator.Eliminar(id));
    }

    [AllowAnonymous]
    [HttpGet("list", Name = "GetPersonalList")]
    [ProducesResponseType(typeof(IReadOnlyList<Personal>), (int)HttpStatusCode.OK)]
    public ActionResult<IReadOnlyList<Personal>> GetPersonalList()
    {
        return Ok(_mediator.Listar());
    }
}
