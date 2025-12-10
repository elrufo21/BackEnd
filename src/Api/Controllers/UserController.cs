using System.Net;
using Ecommerce.Application.Contracts.Usuarios;
using Ecommerce.Application.Features.Auths.Users.Vms;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUsuario _mediator;

    public UserController(IUsuario mediador)
    {
        _mediator = mediador;
    }

    [AllowAnonymous]
    [HttpPost("acceso", Name = "Acceso")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public ActionResult<AuthResponseA> Acceso([FromBody] EUser request)
    {
        return Ok(_mediator.Login(request));
    }
}
