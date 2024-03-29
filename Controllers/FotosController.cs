using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RestApiDating.Data;
using RestApiDating.Dtos;
using RestApiDating.Helpers;
using RestApiDating.Models;

namespace RestApiDating.Controllers
{
    [ApiController]
    [Route("api/users/{userId}/fotos")]
    public class FotosController : ControllerBase
    {
        private readonly IDatingRepository _repository;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public FotosController(IDatingRepository repository, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _repository = repository;
            _mapper = mapper;
            _cloudinaryConfig = cloudinaryConfig;

            Account account = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
        }

        [HttpGet("{id}", Name = "GetFoto")]
        public async Task<IActionResult> GetFoto(int id)
        {
            var foto = await _repository.GetFoto(id);
            var dto = _mapper.Map<FotoDto>(foto);

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> AddFoto(int userId, [FromForm] FotoCreateDto dto)
        {
            // solo el usuario propietario puede editar su perfil
            if (!IsOwnerUser(userId))
                return Unauthorized();

            var user = await _repository.GetUser(userId);

            var file = dto.File;
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            { // preparamos la imagen para subir
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        // obtenemos la imagen
                        File = new FileDescription(file.Name, stream),
                        // transformamos para que no sea muy grande
                        Transformation = new Transformation().Width(500).Height(500)
                        .Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            var foto = _mapper.Map<Foto>(dto);
            foto.Url = uploadResult.Uri.ToString();
            foto.IdPublico = uploadResult.PublicId;
            // si es la primera foto, sera la principal
            foto.EsPrincipal = !user.Fotos.Any(f => f.EsPrincipal);
            // foto.User = user;
            // _repository.Add(foto);

            user.Fotos.Add(foto);

            if (await _repository.SaveAll())
            {
                // dto a retornar
                var fotoDto = _mapper.Map<FotoDto>(foto);
                return CreatedAtRoute("GetFoto", new { Id = foto.Id }, fotoDto);
            }

            return BadRequest("No se pudo subir la imagen");
        }

        [HttpPost("{id}/principal")]
        public async Task<IActionResult> SetPrincipal(int userId, int id)
        {
            // solo el usuario propietario puede establecer su foto principal
            if (!IsOwnerUser(userId))
                return Unauthorized();

            var user = await _repository.GetUser(userId);

            // solo puede establecer una foto que le corresponte
            if (!user.Fotos.Any(f => f.Id == id))
                return Unauthorized();

            var foto = await _repository.GetFoto(id);
            if (foto.EsPrincipal)
                return BadRequest("La foto ya es principal");

            var fotoPrincipalActual = await _repository.GetFotoPrincipal(userId);
            fotoPrincipalActual.EsPrincipal = false;

            // nueva foto principal
            foto.EsPrincipal = true;

            if (await _repository.SaveAll())
                return NoContent();

            return BadRequest("Ocurrió un error al intentar establecer la foto de perfil");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int userId, int id)
        {
            // el usuario logeado solo puede eliminar sus fotos
            if (!IsOwnerUser(userId))
                return Unauthorized();

            var user = await _repository.GetUser(userId);

            // solo puede eliminar si la foto es suya
            if (!user.Fotos.Any(f => f.Id == id))
                return Unauthorized();

            var foto = await _repository.GetFoto(id);
            if (foto.EsPrincipal)
                return BadRequest("No se puede eliminar la foto principal");

            if (foto.IdPublico != null)
            {
                // eliminamos la foto del cloudinary
                var deleteParams = new DeletionParams(foto.IdPublico);
                var deleteResult = _cloudinary.Destroy(deleteParams);

                if (deleteResult.Result.Equals("ok"))
                {
                    _repository.Delete(foto);
                }
            }
            else
            {
                _repository.Delete(foto);
            }

            if (await _repository.SaveAll())
            {
                return Ok();
            }

            return BadRequest("Ocurrio un error al intentar eliminar la foto");
        }

        private bool IsOwnerUser(int userId)
        {
            // User obtiene los claims asociados con la acción que se está ejecutando.
            return (userId == int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value));
        }
    }
}