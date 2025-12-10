using System.Net;
using Ecommerce.Application.Contracts.Cliente;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ClienteController: ControllerBase
{
    private readonly ICliente _mediator;
    public ClienteController(ICliente mediador)
    {
        _mediator = mediador;
    }

    [AllowAnonymous]
    [HttpGet(Name = "GetListCombo")]
    [ProducesResponseType(typeof(String), (int)HttpStatusCode.OK)]
    public ActionResult<String> ListarCombo()
    {
        return Ok(_mediator.ListarCombo());
    }
}
