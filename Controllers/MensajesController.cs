using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RestApiDating.Data;
using RestApiDating.Dtos;
using RestApiDating.Helpers;
using RestApiDating.Models;

namespace RestApiDating.Controllers
{
    [ApiController]
    [Route("api/users/{userId}/[controller]")]
    [ServiceFilter(typeof(LogUserActivity))]
    public class MensajesController : ControllerBase
    {
        private IDatingRepository _repository;
        private IMapper _mapper;

        public MensajesController(IDatingRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet("{id}", Name = "GetMensaje")]
        public async Task<IActionResult> GetMensaje(int userId, int id)
        {
            if (!IsValidUser(userId)) return Unauthorized();

            var mensaje = await _repository.GetMensaje(id);

            if (mensaje == null) return NotFound();

            var dto = _mapper.Map<MensajeDetailDto>(mensaje);

            return Ok(dto);
        }

        [HttpGet]
        public async Task<IActionResult> GetMensajes(int userId, [FromQuery] MensajesParams mensajesParams)
        {
            if (!IsValidUser(userId)) return Unauthorized();

            mensajesParams.UserId = userId;

            var mensajes = await _repository.GetMensajesForUser(mensajesParams);

            var dto = _mapper.Map<List<MensajeDetailDto>>(mensajes);

            Response.AddPagination(mensajes.CurrentPage, mensajes.PageSize, mensajes.TotalCount, mensajes.TotalPages);

            return Ok(dto);
        }

        [HttpGet("conversacion/{receptorId}")]
        public async Task<IActionResult> GetConversacion(int userId, int receptorId)
        {
            if (!IsValidUser(userId)) return Unauthorized();

            var mensajes = await _repository.GetConversacion(userId, receptorId);

            var dto = _mapper.Map<IEnumerable<MensajeDetailDto>>(mensajes);

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int userId, MensajeCreateDto dto)
        {
            if (!IsValidUser(userId)) return Unauthorized();

            var emisor = await _repository.GetUser(userId);
            var receptor = await _repository.GetUser(dto.ReceptorId);
            if (receptor == null) return BadRequest("No existe el usuario receptor");

            var mensaje = _mapper.Map<Mensaje>(dto);
            mensaje.EmisorId = userId;

            _repository.Add<Mensaje>(mensaje);

            if (await _repository.SaveAll())
            {
                var dtoDetail = _mapper.Map<MensajeDetailDto>(mensaje);
                return CreatedAtRoute("GetMensaje", new { Id = mensaje.Id }, dtoDetail);
            }

            throw new Exception("Ocurrio un error al intentar enviar el mensaje");
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Delete(int id, int userId)
        {
            if (!IsValidUser(userId)) return Unauthorized();

            var mensaje = await _repository.GetMensaje(id);

            if (mensaje.EmisorId == userId)
                mensaje.HaSidoEliminadoPorEmisor = true;

            if (mensaje.ReceptorId == userId)
                mensaje.HaSidoEliminadoPorReceptor = true;

            if (mensaje.HaSidoEliminadoPorEmisor && mensaje.HaSidoEliminadoPorReceptor)
                _repository.Delete<Mensaje>(mensaje);

            if(await _repository.SaveAll())
                return NoContent();

            throw new Exception("Error al intentar eliminar el mensaje");

        }

        /// <summary>
        /// Verifica si userId es igual a userId del usuario logeado 
        /// /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private bool IsValidUser(int userId)
        {
            var userLoggedId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return userLoggedId == userId;
        }
    }
}