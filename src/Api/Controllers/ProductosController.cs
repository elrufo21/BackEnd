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
    [HttpPost("register", Name = "RegisterProducto")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult RegisterProducto([FromBody] Producto producto)
    {
        return Ok(_mediator.Insertar(producto));
    }

    [AllowAnonymous]
    [HttpPut("{id:long}", Name = "EditarProducto")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult EditarProducto(long id, [FromBody] Producto producto)
    {
        return Ok(_mediator.Editar(id, producto));
    }

    [AllowAnonymous]
    [HttpDelete("{id:long}", Name = "EliminarProducto")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult EliminarProducto(long id)
    {
        return Ok(_mediator.Eliminar(id));
    }

    [AllowAnonymous]
    [HttpGet("list", Name = "GetProductoList")]
    [ProducesResponseType(typeof(IReadOnlyList<Producto>), (int)HttpStatusCode.OK)]
    public ActionResult<IReadOnlyList<Producto>> GetProductoList()
    {
        return Ok(_mediator.ListarCrud());
    }

    [AllowAnonymous]
    [HttpGet("{id:long}", Name = "GetProductoById")]
    [ProducesResponseType(typeof(Producto), (int)HttpStatusCode.OK)]
    public ActionResult<Producto?> GetProductoById(long id)
    {
        var producto = _mediator.ObtenerPorId(id);
        if (producto is null) return NotFound();
        return Ok(producto);
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
