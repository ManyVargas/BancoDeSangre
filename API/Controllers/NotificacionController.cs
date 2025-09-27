using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificacionController : ControllerBase
    {
        private readonly INotificacionRepositorio _repo;

        public NotificacionController(INotificacionRepositorio repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        [HttpGet("donante/{donanteId:int}")]
        public async Task<ActionResult<IEnumerable<ListarNotificacionDTO>>> ListarPorDonante(int donanteId, CancellationToken ct)
        {
            var notificaciones = await _repo.ListarPorDonanteAsync(donanteId, ct);
            return Ok(notificaciones);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ListarNotificacionDTO>> ObtenerPorId(int id, CancellationToken ct)
        {
            var notificacion = await _repo.ListarPorIdAsync(id, ct);
            if (notificacion == null)
                return NotFound(new { error = $"No se encontró notificación con ID {id}" });

            return Ok(notificacion);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Crear(CrearNotificacionDTO dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var id = await _repo.CrearAsync(dto, ct);
                return CreatedAtAction(nameof(ObtenerPorId), new { id }, id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al crear la notificación.", details = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ListarNotificacionDTO>> Actualizar(int id, ActualizarNotificacionDTO dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var actualizado = await _repo.ActualizarAsync(id, dto, ct);
            if (actualizado == null)
                return NotFound(new { error = $"No se pudo actualizar, no existe notificación con ID {id}" });

            return Ok(actualizado);
        }

        [HttpPatch("{id:int}/leida")]
        public async Task<ActionResult> MarcarLeida(int id, [FromBody] bool leido, CancellationToken ct)
        {
            var exito = await _repo.MarcarLeidaAsync(id, leido, ct);
            if (!exito)
                return NotFound(new { error = $"No se pudo marcar como leída, no existe notificación con ID {id}" });

            return Ok(new { message = $"Notificación con ID {id} {(leido ? "marcada como leída" : "marcada como no leída")}" });
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Eliminar(int id, CancellationToken ct)
        {
            var eliminado = await _repo.EliminarAsync(id, ct);
            if (!eliminado)
                return NotFound(new { error = $"No se pudo eliminar, no existe notificación con ID {id}" });

            return Ok(new { message = $"Notificación con ID {id} eliminada correctamente" });
        }
    }
}