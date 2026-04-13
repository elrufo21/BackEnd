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
        [FromForm(Name = "imagen")] IFormFile? imagen,
        [FromForm(Name = "imageFile")] IFormFile? imageFile,
        [FromForm(Name = "imagenUnidad")] IFormFile? imagenUnidad,
        [FromForm] bool eliminarImagen = false,
        CancellationToken cancellationToken = default)
    {
        var imagenRequest = imagen ?? imageFile;

        Producto? existente = null;
        if (producto.IdProducto > 0)
        {
            existente = await _mediator.ObtenerPorIdAsync(producto.IdProducto, cancellationToken);
        }

        if (imagenRequest is not null && imagenRequest.Length > 0)
        {
            if (!IsValidImage(imagenRequest, out var error))
            {
                return BadRequest(error);
            }

            if (existente is not null && !string.IsNullOrWhiteSpace(existente.ProductoImagen))
            {
                await _imageService.DeleteImage(existente.ProductoImagen);
            }

            await using var stream = imagenRequest.OpenReadStream();
            var imageData = new ImageData
            {
                ImageStream = stream,
                Nombre = imagenRequest.FileName
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
            producto.ProductoImagen = string.Empty;
        }
        else if (producto.IdProducto > 0 && string.IsNullOrWhiteSpace(producto.ProductoImagen))
        {
            // Mantener la imagen existente en una actualización cuando no se envía nueva.
            if (existente is not null)
            {
                producto.ProductoImagen = existente.ProductoImagen;
            }
        }

        var unidadImagenFiles = GetUnidadMedidaImageFiles(imagenUnidad);
        var unidadImagenError = await ReplaceUnidadMedidaImagesFromFilesAsync(producto, unidadImagenFiles, cancellationToken);
        if (!string.IsNullOrWhiteSpace(unidadImagenError))
        {
            return BadRequest(unidadImagenError);
        }

        return Ok(await _mediator.InsertarAsync(producto, cancellationToken));
    }

    [Authorize]
    [RequestSizeLimit(MaxImageSizeBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxImageSizeBytes)]
    [HttpPost("register-with-image", Name = "RegisterProductoConImagen")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> RegisterProductoConImagen(
        [FromForm] Producto producto,
        [FromForm(Name = "imagen")] IFormFile? imagen,
        [FromForm(Name = "imageFile")] IFormFile? imageFile,
        [FromForm(Name = "imagenUnidad")] IFormFile? imagenUnidad,
        CancellationToken cancellationToken)
    {
        var imagenRequest = imagen ?? imageFile;
        if (imagenRequest is not null && imagenRequest.Length > 0)
        {
            if (!IsValidImage(imagenRequest, out var error))
            {
                return BadRequest(error);
            }

            await using var stream = imagenRequest.OpenReadStream();
            var imageData = new ImageData
            {
                ImageStream = stream,
                Nombre = imagenRequest.FileName
            };

            var uploadResult = await _imageService.UploadImage(imageData);
            producto.ProductoImagen = uploadResult.Url;
        }

        var unidadImagenFiles = GetUnidadMedidaImageFiles(imagenUnidad);
        var unidadImagenError = await ReplaceUnidadMedidaImagesFromFilesAsync(producto, unidadImagenFiles, cancellationToken);
        if (!string.IsNullOrWhiteSpace(unidadImagenError))
        {
            return BadRequest(unidadImagenError);
        }

        return Ok(await _mediator.InsertarAsync(producto, cancellationToken));
    }

    [Authorize]
    [RequestSizeLimit(MaxImageSizeBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxImageSizeBytes)]
    [HttpPost("unidad-medida", Name = "GuardarUnidadMedidaProducto")]
    [ProducesResponseType(typeof(GuardarUnidadMedidaProductoResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<GuardarUnidadMedidaProductoResponse>> GuardarUnidadMedidaProducto(
        [FromForm] GuardarUnidadMedidaProductoRequest request,
        [FromForm(Name = "imagen")] IFormFile? imagen,
        CancellationToken cancellationToken = default)
    {
        if (request.IdProducto <= 0)
        {
            return BadRequest("IdProducto debe ser mayor a 0.");
        }

        if (string.IsNullOrWhiteSpace(request.UMDescripcion))
        {
            return BadRequest("UMDescripcion es requerido.");
        }

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
            request.UnidadImagen = uploadResult.Url;
        }

        var idUm = await _mediator.GuardarUnidadMedidaProductoAsync(request, cancellationToken);
        return Ok(new GuardarUnidadMedidaProductoResponse
        {
            IdUm = idUm
        });
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
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetProductoList(
        [FromQuery] string? estado = "ACTIVO",
        CancellationToken cancellationToken = default)
    {
        var raw = await _mediator.ListarCrudRawAsync(estado, cancellationToken);
        return Content(raw, "text/plain");
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

    private IReadOnlyList<IFormFile> GetUnidadMedidaImageFiles(IFormFile? imagenUnidad)
    {
        if (imagenUnidad is not null)
        {
            return new[] { imagenUnidad };
        }

        if (!Request.HasFormContentType || Request.Form.Files.Count == 0)
        {
            return Array.Empty<IFormFile>();
        }

        return Request.Form.Files
            .Where(file => IsUnidadMedidaFileField(file.Name))
            .ToList();
    }

    private async Task<string?> ReplaceUnidadMedidaImagesFromFilesAsync(
        Producto producto,
        IReadOnlyList<IFormFile> files,
        CancellationToken cancellationToken)
    {
        if (files.Count == 0)
        {
            return null;
        }

        var rawData = producto.Data;
        if (string.IsNullOrWhiteSpace(rawData))
        {
            return "Se enviaron imágenes de unidad de medida, pero falta el campo Data con el detalle.";
        }

        var openIndex = rawData.IndexOf('[');
        var closeIndex = rawData.LastIndexOf(']');
        if (openIndex < 0 || closeIndex <= openIndex)
        {
            return "Formato de Data inválido. Debe incluir detalle de unidad de medida entre [ y ].";
        }

        var detalle = rawData[(openIndex + 1)..closeIndex];
        if (string.IsNullOrWhiteSpace(detalle))
        {
            return "Se enviaron imágenes de unidad de medida, pero el detalle de Data está vacío.";
        }

        var items = detalle.Split(';');
        if (items.Length == 0)
        {
            return null;
        }

        var indexedFiles = new Dictionary<int, IFormFile>();
        var sequentialFiles = new Queue<IFormFile>();
        foreach (var file in files)
        {
            if (TryGetUnidadMedidaIndex(file.Name, out var index))
            {
                indexedFiles[index] = file;
                continue;
            }

            sequentialFiles.Enqueue(file);
        }

        var changed = false;
        for (var i = 0; i < items.Length; i++)
        {
            var item = items[i];
            if (string.IsNullOrWhiteSpace(item))
            {
                continue;
            }

            var file = indexedFiles.TryGetValue(i, out var indexedFile)
                ? indexedFile
                : sequentialFiles.Count > 0 ? sequentialFiles.Dequeue() : null;

            if (file is null || file.Length == 0)
            {
                continue;
            }

            if (!IsValidImage(file, out var error))
            {
                return $"Imagen de unidad de medida inválida ({file.Name}): {error}";
            }

            var campos = item.Split('|').ToList();
            while (campos.Count < 6)
            {
                campos.Add(string.Empty);
            }

            await using var stream = file.OpenReadStream();
            var uploadResult = await _imageService.UploadImage(new ImageData
            {
                ImageStream = stream,
                Nombre = string.IsNullOrWhiteSpace(file.FileName) ? $"producto-um-{Guid.NewGuid():N}" : file.FileName
            });

            campos[5] = uploadResult.Url ?? string.Empty;
            items[i] = string.Join("|", campos);
            changed = true;
        }

        if (!changed)
        {
            return null;
        }

        var detalleActualizado = string.Join(";", items);
        producto.Data = $"{rawData[..(openIndex + 1)]}{detalleActualizado}{rawData[closeIndex..]}";
        return null;
    }

    private static bool IsUnidadMedidaFileField(string? fieldName)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            return false;
        }

        var normalized = fieldName.Trim().ToLowerInvariant();
        if (normalized == "imagenunidad")
        {
            return true;
        }

        return normalized == "unidadimagen"
            || normalized == "unidadimagenes"
            || normalized.StartsWith("unidadimagen[", StringComparison.Ordinal)
            || normalized.StartsWith("unidadimagenes[", StringComparison.Ordinal)
            || normalized.StartsWith("unidadimagen_", StringComparison.Ordinal)
            || normalized.StartsWith("unidadimagenes_", StringComparison.Ordinal)
            || normalized.StartsWith("imagenunidad[", StringComparison.Ordinal)
            || normalized.StartsWith("imagenunidad_", StringComparison.Ordinal);
    }

    private static bool TryGetUnidadMedidaIndex(string fieldName, out int index)
    {
        index = -1;
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            return false;
        }

        var openBracket = fieldName.IndexOf('[');
        if (openBracket >= 0)
        {
            var closeBracket = fieldName.IndexOf(']', openBracket + 1);
            if (closeBracket > openBracket + 1)
            {
                var value = fieldName[(openBracket + 1)..closeBracket];
                return int.TryParse(value, out index) && index >= 0;
            }
        }

        var lastUnderscore = fieldName.LastIndexOf('_');
        if (lastUnderscore >= 0 && lastUnderscore < fieldName.Length - 1)
        {
            var value = fieldName[(lastUnderscore + 1)..];
            return int.TryParse(value, out index) && index >= 0;
        }

        return false;
    }
}
