using System.Net;
using Ecommerce.Application.Contracts.Productos;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;


[ApiController]
[Route("api/v1/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly IProducto _mediator;

    public ProductosController(IProducto mediador)
    {
        _mediator = mediador;
    }
    [AllowAnonymous]
    [HttpGet("listaPro", Name = "GetListPro")]
    [ProducesResponseType(typeof(IReadOnlyList<EListaProducto>), (int)HttpStatusCode.OK)]
    public ActionResult<IReadOnlyList<EListaProducto>> GetListPro()
    {
        return Ok(_mediator.Listar());
    }
    [AllowAnonymous]
    [HttpGet("buscaPro", Name = "GetBusPro")]
    [ProducesResponseType(typeof(IReadOnlyList<EListaProducto>), (int)HttpStatusCode.OK)]
    public ActionResult<IReadOnlyList<EListaProducto>> GetBusPro(string nombre)
    {
        return Ok(_mediator.BuscarProducto(nombre));
    }
}
