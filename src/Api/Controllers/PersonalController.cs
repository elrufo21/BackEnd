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
    private readonly IPersonal _mediator;
    private readonly IManageImageService _imageService;

    public PersonalController(IPersonal mediator, IManageImageService imageService)
    {
        _mediator = mediator;
        _imageService = imageService;
    }

    [AllowAnonymous]
    [HttpPost("registerpersonal", Name = "RegisterPersonal")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> RegisterPersonal([FromForm] Personal personal, IFormFile? imagen, [FromForm] bool eliminarImagen = false)
    {
        Personal? existente = null;
        if (personal.PersonalId > 0)
        {
            existente = _mediator.Listar().FirstOrDefault(x => x.PersonalId == personal.PersonalId);
        }

        if (imagen is not null && imagen.Length > 0)
        {
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

        return Ok(_mediator.Insertar(personal));
    }

    [AllowAnonymous]
    [HttpDelete("{id}", Name = "EliminarPersonal")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> EliminarPersonal(long id)
    {
        var existente = _mediator.Listar().FirstOrDefault(x => x.PersonalId == id);
        if (existente is not null && !string.IsNullOrWhiteSpace(existente.PersonalImagen))
        {
            await _imageService.DeleteImage(existente.PersonalImagen);
        }

        return Ok(_mediator.Eliminar(id));
    }

    [AllowAnonymous]
    [HttpGet("list", Name = "GetPersonalList")]
    [ProducesResponseType(typeof(IReadOnlyList<Personal>), (int)HttpStatusCode.OK)]
    public ActionResult<IReadOnlyList<Personal>> GetPersonalList([FromQuery] string? estado = "ACTIVO")
    {
        return Ok(_mediator.Listar(estado));
    }
}
