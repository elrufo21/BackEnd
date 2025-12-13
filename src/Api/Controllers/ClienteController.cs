using System.Net;
using Ecommerce.Application.Contracts.Clientes;
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
    [HttpPost("register", Name = "RegisterCliente")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult RegisterCliente([FromBody] Cliente cliente)
    {
        return Ok(_mediator.Insertar(cliente));
    }

    [AllowAnonymous]
    [HttpPut("{id}", Name = "EditarCliente")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult EditarCliente(long id, [FromBody] Cliente cliente)
    {
        return Ok(_mediator.Editar(id, cliente));
    }

    [AllowAnonymous]
    [HttpDelete("{id}", Name = "EliminarCliente")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult EliminarCliente(long id)
    {
        return Ok(_mediator.Eliminar(id));
    }

    [AllowAnonymous]
    [HttpGet("list", Name = "GetClienteList")]
    [ProducesResponseType(typeof(IReadOnlyList<Cliente>), (int)HttpStatusCode.OK)]
    public ActionResult<IReadOnlyList<Cliente>> GetClienteList()
    {
        return Ok(_mediator.Listar());
    }

    [AllowAnonymous]
    [HttpGet(Name = "GetListCombo")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    public ActionResult<string> ListarCombo()
    {
        return Ok(_mediator.ListarCombo());
    }
}
