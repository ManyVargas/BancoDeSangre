using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]      // => api/ParticipacionCampana
    [Tags("Participación Campaña")]
    public class ParticipacionCampanaController : ControllerBase
    {
        private readonly IParticipacionCampanaRepositorio _repo;

        public ParticipacionCampanaController(IParticipacionCampanaRepositorio repo)
        {
            _repo = repo;
        }

        // POST: api/ParticipacionCampana
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        public async Task<IActionResult> Crear([FromBody] CrearParticipacionCampanaDTO dto, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var id = await _repo.CrearAsync(dto, ct);
            return CreatedAtAction(nameof(Obtener), new { id }, new { ParticipacionID = id });
        }

        // GET: api/ParticipacionCampana/{id}
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ParticipacionCampanaDetalleDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Obtener(int id, CancellationToken ct)
        {
            if (id <= 0) return BadRequest("Id inválido.");

            var r = await _repo.ObtenerPorIdAsync(id, ct);
            return r is null ? NotFound() : Ok(r);
        }

        // PUT: api/ParticipacionCampana/{id}/cantidad  (SP)
        [HttpPut("{id:int}/cantidad")]
        [ProducesResponseType(typeof(ParticipacionCampanaDetalleDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ActualizarCantidad(
            int id, [FromBody] ActualizarCantidadUnidadesDTO body, CancellationToken ct)
        {
            if (id <= 0) return BadRequest("Id inválido.");
            if (body is null) return BadRequest("Body requerido.");
            if (body.CantidadUnidades <= 0) return BadRequest("La cantidad de unidades debe ser mayor a 0.");

            try
            {
                var r = await _repo.ActualizarCantidadUnidadesAsync(id, body.CantidadUnidades, ct);
                return r is null ? NotFound() : Ok(r);
            }
            catch (InvalidOperationException ex)
            {
                // Mensajes del RAISERROR del SP (participación no existe / cantidad <= 0)
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/ParticipacionCampana/{id}
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Eliminar(int id, CancellationToken ct)
        {
            if (id <= 0) return BadRequest("Id inválido.");

            var ok = await _repo.EliminarAsync(id, ct);
            return ok ? NoContent() : NotFound();
        }
    }
}
