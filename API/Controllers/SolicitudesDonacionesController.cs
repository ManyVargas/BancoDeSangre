using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudesDonacionesController : ControllerBase
    {
        private readonly ISolicitudesDonacionRepositorio _repo;

        public SolicitudesDonacionesController(ISolicitudesDonacionRepositorio repo)
        {
            _repo = repo;
        }

        // GET: api/SolicitudesDonacion
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SolicitudesDonacionDto>>> Listar()
        {
            try
            {
                var items = await _repo.ListarSolicitudes();
                if (items == null || !items.Any()) return NoContent();
                return Ok(items);
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"Error de base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al listar solicitudes: {ex.Message}");
            }
        }

        // GET: api/SolicitudesDonacion/estado/{estado}
        [HttpGet("estado/{estado}")]
        public async Task<ActionResult<IEnumerable<SolicitudesDonacionDto>>> ListarPorEstado(string estado)
        {
            if (string.IsNullOrWhiteSpace(estado)) return BadRequest("Estado requerido.");

            try
            {
                var items = await _repo.ListarSolicitudesPorEstado(estado);
                if (items == null || !items.Any()) return NoContent();
                return Ok(items);
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"Error de base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al listar por estado: {ex.Message}");
            }
        }

        // POST: api/SolicitudesDonacion
        [HttpPost]
        public async Task<ActionResult> Crear([FromBody] CrearSolicitudesDonacionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Validaciones básicas (ajusta según tus reglas)
            if (dto.PacienteId <= 0) return BadRequest("PacienteId inválido.");
            if (dto.TipoSangreId <= 0) return BadRequest("TipoSangreId inválido.");
            if (dto.CantidadRequerida <= 0) return BadRequest("CantidadRequerida debe ser > 0.");
            if (dto.HospitalId <= 0) return BadRequest("HospitalId inválido.");
            if (dto.BancoId <= 0) return BadRequest("BancoId inválido.");

            try
            {
                var nuevoId = await _repo.CrearSolicitudesDonacion(dto);
                // No tenemos GET by id, retornamos 201 con el ID nuevo
                return StatusCode(StatusCodes.Status201Created, new { solicitudID = nuevoId });
            }
            catch (SqlException ex) when (ex.Number == 547) // FK
            {
                return BadRequest("Alguna clave foránea no existe (Paciente/TipoSangre/Hospital/Banco).");
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"Error de base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear la solicitud: {ex.Message}");
            }
        }

        // PUT: api/SolicitudesDonacion/{id}/estado
        // Body: { "estado": "Aprobada" }
        [HttpPut("{id:int}/estado")]
        public async Task<ActionResult<SolicitudesDonacionDto>> ActualizarEstado(int id, [FromBody] ActualizarEstadoSolicitudDto body)
        {
            if (id <= 0) return BadRequest("SolicitudId inválido.");
            if (body is null || string.IsNullOrWhiteSpace(body.Estado)) return BadRequest("Estado requerido.");

            try
            {
                var actualizado = await _repo.ActualizarEstadoSolicitud(id, body.Estado);
                return Ok(actualizado);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Solicitud no encontrada.");
            }
            // Si tu SP usa THROW con códigos propios, podrías mapearlos aquí:
            // catch (SqlException ex) when (ex.Number == 53001) { return NotFound("Solicitud no existe."); }
            catch (SqlException ex)
            {
                return StatusCode(500, $"Error de base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar el estado: {ex.Message}");
            }
        }
    }
}
