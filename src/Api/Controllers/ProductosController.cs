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
    private const long MaxImageSizeBytes = 5 * 1024 * 1024; // 5 MB
    private static readonly HashSet<string> AllowedImageContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    private readonly IProducto _mediator;
    private readonly IManageImageService _imageService;

    public ProductosController(IProducto mediador, IManageImageService imageService)
    {
        _mediator = mediador;
        _imageService = imageService;
    }

    [Authorize]
    [RequestSizeLimit(MaxImageSizeBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxImageSizeBytes)]
    [HttpPost("register", Name = "RegisterProducto")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> RegisterProducto(
        [FromForm] Producto producto,
        IFormFile? imagen,
        [FromForm] bool eliminarImagen = false,
        CancellationToken cancellationToken = default)
    {
        Producto? existente = null;
        if (producto.IdProducto > 0)
        {
            existente = await _mediator.ObtenerPorIdAsync(producto.IdProducto, cancellationToken);
        }

        if (imagen is not null && imagen.Length > 0)
        {
            if (!IsValidImage(imagen, out var error))
            {
                return BadRequest(error);
            }

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

        return Ok(await _mediator.InsertarAsync(producto, cancellationToken));
    }

    [Authorize]
    [RequestSizeLimit(MaxImageSizeBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxImageSizeBytes)]
    [HttpPost("register-with-image", Name = "RegisterProductoConImagen")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> RegisterProductoConImagen([FromForm] Producto producto, IFormFile? imagen, CancellationToken cancellationToken)
    {
        if (imagen is not null && imagen.Length > 0)
        {
            if (!IsValidImage(imagen, out var error))
            {
                return BadRequest(error);
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

        return Ok(await _mediator.InsertarAsync(producto, cancellationToken));
    }

    [Authorize]
    [HttpDelete("{id:long}", Name = "EliminarProducto")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> EliminarProducto(long id, CancellationToken cancellationToken)
    {
        var existente = await _mediator.ObtenerPorIdAsync(id, cancellationToken);
        if (existente is not null && !string.IsNullOrWhiteSpace(existente.ProductoImagen))
        {
            await _imageService.DeleteImage(existente.ProductoImagen);
        }

        return Ok(await _mediator.EliminarAsync(id, cancellationToken));
    }

    [AllowAnonymous]
    [HttpGet("list", Name = "GetProductoList")]
    [ProducesResponseType(typeof(IReadOnlyList<Producto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<Producto>>> GetProductoList(
        [FromQuery] string? estado = "ACTIVO",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.ListarCrudAsync(estado, page, pageSize, cancellationToken));
    }

    [AllowAnonymous]
    [HttpGet("{id:long}", Name = "GetProductoById")]
    [ProducesResponseType(typeof(Producto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Producto?>> GetProductoById(long id, CancellationToken cancellationToken)
    {
        var producto = await _mediator.ObtenerPorIdAsync(id, cancellationToken);
        if (producto is null) return NotFound();
        return Ok(producto);
    }

    [AllowAnonymous]
    [HttpGet("listaPro", Name = "GetListPro")]
    [ProducesResponseType(typeof(IReadOnlyList<EListaProducto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<EListaProducto>>> GetListPro(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.ListarAsync(page, pageSize, cancellationToken));
    }
    [AllowAnonymous]
    [HttpGet("buscaPro", Name = "GetBusPro")]
    [ProducesResponseType(typeof(IReadOnlyList<EListaProducto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<EListaProducto>>> GetBusPro(
        [FromQuery] string nombre,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.BuscarProductoAsync(nombre, page, pageSize, cancellationToken));
    }

    private static bool IsValidImage(IFormFile file, out string error)
    {
        if (file.Length > MaxImageSizeBytes)
        {
            error = $"La imagen excede el límite de {MaxImageSizeBytes / (1024 * 1024)} MB.";
            return false;
        }

        if (!AllowedImageContentTypes.Contains(file.ContentType))
        {
            error = "Tipo de archivo no permitido. Use JPG, PNG o WEBP.";
            return false;
        }

        error = string.Empty;
        return true;
    }
}
