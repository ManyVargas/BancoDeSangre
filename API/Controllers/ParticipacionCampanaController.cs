using System;
using System.Threading;
using System.Threading.Tasks;
using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParticipacionCampanaController : ControllerBase
    {
        private readonly IParticipacionCampanaRepositorio _repo;

        public ParticipacionCampanaController(IParticipacionCampanaRepositorio repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

  
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearParticipacionCampanaDTO dto, CancellationToken ct)
        {
            if (dto == null)
                return BadRequest("El objeto participación no puede ser nulo.");

            try
            {
                var id = await _repo.CrearAsync(dto, ct);
                return Ok(new { Message = "Participación registrada exitosamente.", ParticipacionID = id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorCampana(int id, CancellationToken ct)
        {
            if (id <= 0)
                return BadRequest("El ID de la campaña debe ser mayor que cero.");

          
                var participante = await _repo.ObtenerPorPaticipantesPorIdAsync(id, ct);

                if (participante == null)
                    return NotFound(new { Message = "No se encontraron participantes para esta campaña." });

                return Ok(participante);
   
        }
    }
}
