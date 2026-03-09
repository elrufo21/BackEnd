using System.Net;
using Ecommerce.Application.Contracts.Personales;
using Ecommerce.Application.Contracts.Infrastructure;
using Ecommerce.Application.Models.ImageManagement;
using Ecommerce.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PersonalController : ControllerBase
{
    private const long MaxImageSizeBytes = 5 * 1024 * 1024; // 5 MB
    private static readonly HashSet<string> AllowedImageContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    private readonly IPersonal _mediator;
    private readonly IManageImageService _imageService;

    public PersonalController(IPersonal mediator, IManageImageService imageService)
    {
        _mediator = mediator;
        _imageService = imageService;
    }

    [Authorize]
    [RequestSizeLimit(MaxImageSizeBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxImageSizeBytes)]
    [HttpPost("registerpersonal", Name = "RegisterPersonal")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> RegisterPersonal([FromForm] Personal personal, IFormFile? imagen, [FromForm] bool eliminarImagen = false, CancellationToken cancellationToken = default)
    {
        Personal? existente = null;
        if (personal.PersonalId > 0)
        {
            existente = await _mediator.ObtenerPorIdAsync(personal.PersonalId, cancellationToken);
        }

        if (imagen is not null && imagen.Length > 0)
        {
            if (!IsValidImage(imagen, out var error))
            {
                return BadRequest(error);
            }

            if (existente is not null && !string.IsNullOrWhiteSpace(existente.PersonalImagen))
            {
                await _imageService.DeleteImage(existente.PersonalImagen);
            }

            await using var stream = imagen.OpenReadStream();
            var imageData = new ImageData
            {
                ImageStream = stream,
                Nombre = imagen.FileName
            };

            var uploadResult = await _imageService.UploadImage(imageData);
            personal.PersonalImagen = uploadResult.Url;
        }
        else if (eliminarImagen)
        {
            if (existente is not null && !string.IsNullOrWhiteSpace(existente.PersonalImagen))
            {
                await _imageService.DeleteImage(existente.PersonalImagen);
            }
            personal.PersonalImagen = null;
        }
        else if (personal.PersonalId > 0 && string.IsNullOrWhiteSpace(personal.PersonalImagen))
        {
            // Mantener la imagen existente cuando no se envía una nueva y no se marca para eliminar.
            if (existente is not null)
            {
                personal.PersonalImagen = existente.PersonalImagen;
            }
        }

        return Ok(await _mediator.InsertarAsync(personal, cancellationToken));
    }

    [Authorize]
    [HttpDelete("{id}", Name = "EliminarPersonal")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> EliminarPersonal(long id, CancellationToken cancellationToken)
    {
        var existente = await _mediator.ObtenerPorIdAsync(id, cancellationToken);
        if (existente is not null && !string.IsNullOrWhiteSpace(existente.PersonalImagen))
        {
            await _imageService.DeleteImage(existente.PersonalImagen);
        }

        return Ok(await _mediator.EliminarAsync(id, cancellationToken));
    }

    [AllowAnonymous]
    [HttpGet("list", Name = "GetPersonalList")]
    [ProducesResponseType(typeof(IReadOnlyList<Personal>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyList<Personal>>> GetPersonalList(
        [FromQuery] string? estado = "ACTIVO",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.ListarAsync(estado, page, pageSize, cancellationToken));
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
