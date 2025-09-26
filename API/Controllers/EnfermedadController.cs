using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnfermedadController : ControllerBase
    {
        private readonly IEnfermedadRepositorio _repo;

        public EnfermedadController(IEnfermedadRepositorio repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        // GET: api/enfermedad
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnfermedadDTO>>> Listar(CancellationToken ct)
        {
            var enfermedades = await _repo.ListarAsync(ct);
            return Ok(enfermedades);
        }

        // GET: api/enfermedad/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<EnfermedadDTO>> ObtenerPorId(int id, CancellationToken ct)
        {
            var enfermedad = await _repo.ObtenerPorIdAsync(id, ct);
            if (enfermedad == null)
                return NotFound($"No se encontró enfermedad con ID {id}");
            return Ok(enfermedad);
        }

        // POST: api/enfermedad
        [HttpPost]
        public async Task<ActionResult<int>> Crear(CrearEnfermedadDTO dto, CancellationToken ct)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var id = await _repo.CrearAsync(dto, ct);
            return CreatedAtAction(nameof(ObtenerPorId), new { id }, id);
        }

        // PUT: api/enfermedad/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<EnfermedadDTO>> Actualizar(int id, ActualizarEnfermedadDTO dto, CancellationToken ct)
        {
            var actualizado = await _repo.ActualizarAsync(id, dto, ct);
            if (actualizado == null)
                return NotFound($"No se pudo actualizar, no existe enfermedad con ID {id}");
            return Ok(actualizado);
        }

        // DELETE: api/enfermedad/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Eliminar(int id, CancellationToken ct)
        {
            var eliminado = await _repo.EliminarAsync(id, ct);
            if (!eliminado)
                return NotFound($"No se pudo eliminar, no existe enfermedad con ID {id}");
            return Ok(new { message = $"Enfermedad con ID {id} eliminada correctamente" });
        }
    }
}