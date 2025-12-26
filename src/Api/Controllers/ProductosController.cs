using System.Net;
using Ecommerce.Application.Contracts.Productos;
using Ecommerce.Application.Contracts.Infrastructure;
using Ecommerce.Application.Models.ImageManagement;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;


[ApiController]
[Route("api/v1/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly IProducto _mediator;
    private readonly IManageImageService _imageService;

    public ProductosController(IProducto mediador, IManageImageService imageService)
    {
        _mediator = mediador;
        _imageService = imageService;
    }

    [AllowAnonymous]
    [HttpPost("register", Name = "RegisterProducto")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> RegisterProducto([FromForm] Producto producto, IFormFile? imagen, [FromForm] bool eliminarImagen = false)
    {
        Producto? existente = null;
        if (producto.IdProducto > 0)
        {
            existente = _mediator.ObtenerPorId(producto.IdProducto);
        }

        if (imagen is not null && imagen.Length > 0)
        {
            if (existente is not null && !string.IsNullOrWhiteSpace(existente.ProductoImagen))
            {
                await _imageService.DeleteImage(existente.ProductoImagen);
            }

            await using var stream = imagen.OpenReadStream();
            var imageData = new ImageData
            {
                ImageStream = stream,
                Nombre = imagen.FileName
            };

            var uploadResult = await _imageService.UploadImage(imageData);
            producto.ProductoImagen = uploadResult.Url;
        }
        else if (eliminarImagen)
        {
            if (existente is not null && !string.IsNullOrWhiteSpace(existente.ProductoImagen))
            {
                await _imageService.DeleteImage(existente.ProductoImagen);
            }
            producto.ProductoImagen = null;
        }
        else if (producto.IdProducto > 0 && string.IsNullOrWhiteSpace(producto.ProductoImagen))
        {
            // Mantener la imagen existente en una actualización cuando no se envía nueva.
            if (existente is not null)
            {
                producto.ProductoImagen = existente.ProductoImagen;
            }
        }

        return Ok(_mediator.Insertar(producto));
    }

    [AllowAnonymous]
    [HttpPost("register-with-image", Name = "RegisterProductoConImagen")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> RegisterProductoConImagen([FromForm] Producto producto, IFormFile? imagen)
    {
        if (imagen is not null && imagen.Length > 0)
        {
            await using var stream = imagen.OpenReadStream();
            var imageData = new ImageData
            {
                ImageStream = stream,
                Nombre = imagen.FileName
            };

            var uploadResult = await _imageService.UploadImage(imageData);
            producto.ProductoImagen = uploadResult.Url;
        }

        return Ok(_mediator.Insertar(producto));
    }

    [AllowAnonymous]
    [HttpDelete("{id:long}", Name = "EliminarProducto")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> EliminarProducto(long id)
    {
        var existente = _mediator.ObtenerPorId(id);
        if (existente is not null && !string.IsNullOrWhiteSpace(existente.ProductoImagen))
        {
            await _imageService.DeleteImage(existente.ProductoImagen);
        }

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
