using System.Net;
using Ecommerce.Application.Contracts.Companias;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CompaniaController : ControllerBase
{
    private readonly ICompania _mediator;

    public CompaniaController(ICompania mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost("register", Name = "RegisterCompania")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult RegisterCompania([FromBody] Compania compania)
    {
        return Ok(_mediator.Insertar(compania));
    }

    [AllowAnonymous]
    [HttpPut("{id}", Name = "EditarCompania")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult EditarCompania(int id, [FromBody] Compania compania)
    {
        return Ok(_mediator.Editar(id, compania));
    }

    [AllowAnonymous]
    [HttpDelete("{id}", Name = "EliminarCompania")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult EliminarCompania(int id)
    {
        return Ok(_mediator.Eliminar(id));
    }

    [AllowAnonymous]
    [HttpGet("list", Name = "GetCompaniaList")]
    [ProducesResponseType(typeof(IReadOnlyList<Compania>), (int)HttpStatusCode.OK)]
    public ActionResult<IReadOnlyList<Compania>> GetCompaniaList()
    {
        return Ok(_mediator.Listar());
    }

    [AllowAnonymous]
    [HttpGet("combo", Name = "GetCompaniaCombo")]
    [ProducesResponseType(typeof(IReadOnlyList<EGeneral>), (int)HttpStatusCode.OK)]
    public ActionResult<IReadOnlyList<EGeneral>> GetCompaniaCombo()
    {
        return Ok(_mediator.ListarCombo());
    }
}
