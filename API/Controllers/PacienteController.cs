using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PacienteController : ControllerBase
    {
        private readonly IPacienteRepositorio _repositorio;

        public PacienteController(IPacienteRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        [HttpGet]

        public async Task<ActionResult<IEnumerable<ListarPacienteDTO>>> Listar(CancellationToken ct)
        {
            var donantes = await _repositorio.ListarPacientePacientesAsync(ct);
            return Ok(donantes);
        }

        [HttpGet ("Id/{id}")]
        public async Task<ActionResult<ListarPacienteDTO>> ObtenerPorId(int id, CancellationToken ct)
        {
            var donante = await _repositorio.ObtenerPacientePorIdAsync(id, ct);
            if (donante == null)
                return NotFound();
            return Ok(donante);
        }

        [HttpPost]
        public async Task<ActionResult<int>> RegistrarPaciente([FromBody] RegistrarPacienteDTO dto, CancellationToken ct)
        {
            var nuevoId = await _repositorio.RegistrarPacienteAsync(new PacienteDTO
            {
                Nombre = dto.Nombre,
                CedulaID = dto.CedulaID,
                FechaNacimiento = dto.FechaNacimiento,
                Telefono = dto.Telefono,
                Email = dto.Email,
                Direccion = dto.Direccion,
                TipoSangreID = dto.TipoSangreID
            }, ct);

            return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoId }, nuevoId);
        }

        [HttpPut("Id/{id}")]
        public async Task<ActionResult<ListarPacienteDTO>> ActualizarPaciente(int id, [FromBody] PacienteDTO dto, CancellationToken ct)
        {
            if (id != dto.PacienteId)
                return BadRequest("El ID del paciente no coincide con el ID en el cuerpo de la solicitud.");
            var actualizado = await _repositorio.ActualizarPacienteAsync(dto, ct);
            if (actualizado == null)
                return NotFound();
            return Ok(actualizado);
        }

        [HttpGet("Tipo/{tipo}")]
        public async Task<ActionResult<IEnumerable<ListarPacienteDTO>>> ListarPorTipoSangre(string tipo, CancellationToken ct)
        {
            var donantes = await _repositorio.ListarPacientePacientesPorTipoDeSangreAsync(tipo, ct);
            return Ok(donantes);
        }

        [HttpDelete("Id/{id}")]
        public async Task<ActionResult> EliminarPaciente(int id, CancellationToken ct)
        {
            var eliminado = await _repositorio.EliminarPacienteAsync(id, ct);
            if (!eliminado)
                return NotFound();
            return NoContent();
        }
    }
}
