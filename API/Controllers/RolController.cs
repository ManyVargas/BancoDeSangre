using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Backend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRolRepositorio _repo;

        public RolesController(IRolRepositorio repo, ILogger<RolesController> logger)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RolDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Listar(CancellationToken ct)
        {
            try { return Ok(await _repo.ListarAsync(ct)); }
            catch (Exception ex) {return Problem("Error al listar roles.", ex.Message); }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(RolDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Obtener(int id, CancellationToken ct)
        {
            if (id <= 0) return BadRequest("Id inválido.");
            try
            {
                var r = await _repo.ObtenerPorIdAsync(id, ct);
                return r is null ? NotFound() : Ok(r);
            }
            catch (Exception ex) { return Problem("Error al obtener rol.",ex.Message); }
        }

        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        public async Task<IActionResult> Crear([FromBody] CrearRolDTO dto, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            try
            {
                var id = await _repo.CrearAsync(dto, ct);
                return CreatedAtAction(nameof(Obtener), new { id }, new { RolID = id });
            }
            catch (SqlException ex) { return Problem($"BD: {ex.Message}"); }
            catch (Exception ex) { return Problem("Error al crear rol.", ex.Message); }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(RolDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarRolDTO dto, CancellationToken ct)
        {
            if (id <= 0) return BadRequest("Id inválido.");
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            try
            {
                var r = await _repo.ActualizarAsync(id, dto, ct);
                return r is null ? NotFound() : Ok(r);
            }
            catch (SqlException ex) { return Problem($"BD: {ex.Message}"); }
            catch (Exception ex) { return Problem("Error al actualizar rol.", ex.Message); }
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Eliminar(int id, CancellationToken ct)
        {
            if (id <= 0) return BadRequest("Id inválido.");
            try
            {
                var ok = await _repo.EliminarAsync(id, ct);
                return ok ? NoContent() : NotFound();
            }
            catch (SqlException ex) { return Problem($"BD: {ex.Message}"); }
            catch (Exception ex) { return Problem("Error al eliminar rol.", ex.Message); }
        }
    }
}
