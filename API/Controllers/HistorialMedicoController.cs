using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistorialMedicoController : ControllerBase
    {
        private readonly IHistorialMedicoRepositorio _repo;

        public HistorialMedicoController(IHistorialMedicoRepositorio repo)
        {
            _repo = repo;
        }

        // GET: api/HistorialMedico?donanteId=10   o   api/HistorialMedico?pacienteId=7
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HistorialMedicoDto>>> Listar([FromQuery] int? donanteId, [FromQuery] int? pacienteId)
        {
            // Debe venir exactamente uno
            if ((donanteId.HasValue && pacienteId.HasValue) || (!donanteId.HasValue && !pacienteId.HasValue))
                return BadRequest("Debe enviar DonanteID o PacienteID (solo uno).");

            
                var items = await _repo.ListarHistorialMedico(donanteId, pacienteId);
                if (items == null || !items.Any()) return NoContent();
                return Ok(items);
            
            
        }

        // POST: api/HistorialMedico
        [HttpPost]
        public async Task<ActionResult> Registrar([FromBody] RegistrarHistorialMedicoDto dto)
        {
            // En tu DTO DonanteID y PacienteID son int (no-null).
            // Enforzamos XOR usando > 0 como “enviado”.
            bool tieneDonante = dto.DonanteID > 0;
            bool tienePaciente = dto.PacienteID > 0;

            if (tieneDonante == tienePaciente) // ambos true o ambos false
                return BadRequest("Debe enviar DonanteID o PacienteID (solo uno).");

            try
            {
                var id = await _repo.RegistrarHistorialMedico(dto);
                return StatusCode(StatusCodes.Status201Created, new { historialID = id });
            }
            catch (SqlException ex) when (ex.Number == 547) // FK violada
            {
                return BadRequest("DonanteID o PacienteID no existe (violación de clave foránea).");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al registrar historial médico: {ex.Message}");
            }
        }

        // PUT: api/HistorialMedico/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<HistorialMedicoDto>> Actualizar(int id, [FromBody] HistorialMedicoActualizarDto dto)
        {
            if (id <= 0) return BadRequest("Id inválido.");
            if (dto.HistorialID == 0) dto.HistorialID = id;
            else if (dto.HistorialID != id) return BadRequest("El HistorialID del cuerpo no coincide con la ruta.");

            
                var actualizado = await _repo.ActualizarHistorialMedico(dto);
                if (actualizado is null)
                    return NotFound("No existe el historial médico indicado.");
                return Ok(actualizado);
            
            
        }
    }
}
