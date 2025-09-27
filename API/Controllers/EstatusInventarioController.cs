using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Backend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstatusInventarioController : ControllerBase
    {
        private readonly IEstatusInventarioRepositorio _repo;
        private readonly ILogger<EstatusInventarioController> _logger;

        public EstatusInventarioController(IEstatusInventarioRepositorio repo, ILogger<EstatusInventarioController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EstatusInventarioDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Listar(CancellationToken ct)
        {
            try
            {
                var data = await _repo.ListarAsync(ct);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar estatus de inventario");
                return Problem("Error al listar estatus de inventario.");
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(EstatusInventarioDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Obtener(int id, CancellationToken ct)
        {
            var item = await _repo.ObtenerPorIdAsync(id, ct);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        public async Task<IActionResult> Crear([FromBody] CrearEstatusInventarioDTO dto, CancellationToken ct)
        {
            var id = await _repo.CrearAsync(dto, ct);
            return CreatedAtAction(nameof(Obtener), new { id }, new { EstatusID = id });
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(EstatusInventarioDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarEstatusInventarioDTO dto, CancellationToken ct)
        {
            var actualizado = await _repo.ActualizarAsync(id, dto, ct);
            if (actualizado == null) return NotFound();
            return Ok(actualizado);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Eliminar(int id, CancellationToken ct)
        {
            var ok = await _repo.EliminarAsync(id, ct);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
