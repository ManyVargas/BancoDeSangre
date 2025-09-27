using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Backend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificacionesController : ControllerBase
    {
        private readonly INotificacionRepositorio _repo;
        private readonly ILogger<NotificacionesController> _logger;

        public NotificacionesController(INotificacionRepositorio repo, ILogger<NotificacionesController> logger)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<NotificacionDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Listar(CancellationToken ct)
        {
            try { return Ok(await _repo.ListarAsync(ct)); }
            catch (Exception ex) { _logger.LogError(ex, "Error al listar notificaciones"); return Problem("Error al listar notificaciones."); }
        }

        [HttpGet("usuario/{usuarioId:int}")]
        [ProducesResponseType(typeof(IEnumerable<NotificacionDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListarPorUsuario(int usuarioId, CancellationToken ct)
        {
            try { return Ok(await _repo.ListarPorUsuarioAsync(usuarioId, ct)); }
            catch (Exception ex) { _logger.LogError(ex, "Error al listar notificaciones de usuario {UsuarioId}", usuarioId); return Problem("Error al listar notificaciones del usuario."); }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(NotificacionDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Obtener(int id, CancellationToken ct)
        {
            var item = await _repo.ObtenerPorIdAsync(id, ct);
            return item is null ? NotFound() : Ok(item);
        }

        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        public async Task<IActionResult> Crear([FromBody] CrearNotificacionDTO dto, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            try
            {
                var id = await _repo.CrearAsync(dto, ct);
                return CreatedAtAction(nameof(Obtener), new { id }, new { NotificacionID = id });
            }
            catch (SqlException ex) { _logger.LogError(ex, "SQL al crear notificación"); return Problem($"BD: {ex.Message}"); }
            catch (Exception ex) { _logger.LogError(ex, "Error al crear notificación"); return Problem("Error al crear notificación."); }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(NotificacionDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarNotificacionDTO dto, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            try
            {
                var upd = await _repo.ActualizarAsync(id, dto, ct);
                return upd is null ? NotFound() : Ok(upd);
            }
            catch (SqlException ex) { _logger.LogError(ex, "SQL al actualizar notificación {Id}", id); return Problem($"BD: {ex.Message}"); }
            catch (Exception ex) { _logger.LogError(ex, "Error al actualizar notificación {Id}", id); return Problem("Error al actualizar notificación."); }
        }

        [HttpPatch("{id:int}/leido/{leido:bool}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> MarcarLeida(int id, bool leido, CancellationToken ct)
        {
            var ok = await _repo.MarcarLeidaAsync(id, leido, ct);
            return ok ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Eliminar(int id, CancellationToken ct)
        {
            var ok = await _repo.EliminarAsync(id, ct);
            return ok ? NoContent() : NotFound();
        }
    }
}
