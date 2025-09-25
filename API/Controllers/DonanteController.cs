using Backend.Data;
using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DonantesController : ControllerBase
    {
        private readonly IDonanteRepositorio _repositorio;

        public DonantesController(IDonanteRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        // GET: api/donantes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DonantesDTO>>> Listar(CancellationToken ct)
        {
            var donantes = await _repositorio.ListarAsync(ct);
            return Ok(donantes);
        }

        // GET: api/donantes/tipo/{tipoSangre}
        [HttpGet("tipo/{tipoSangre}")]
        public async Task<ActionResult<IEnumerable<DonantesDTO>>> ListarPorTipoSangre(string tipoSangre, CancellationToken ct)
        {
            var donantes = await _repositorio.ListarPorTipoSangreAsync(tipoSangre, ct);
            return Ok(donantes);
        }

        // GET: api/donantes/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DonantesDTO>> ObtenerPorId(int id, CancellationToken ct)
        {
            var donante = await _repositorio.ObtenerPorIdAsync(id, ct);
            if (donante == null)
                return NotFound();

            return Ok(donante);
        }

        // POST: api/donantes
        [HttpPost]
        public async Task<ActionResult<int>> Crear([FromBody] RegistrarDonanteDTO dto, CancellationToken ct)
        {
            var nuevoId = await _repositorio.CrearAsync(dto, ct);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoId }, nuevoId);
        }

        // PUT: api/donantes/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ListarDonantesDTO>> Actualizar(int id, [FromBody] ActualizarDonanteDTO dto, CancellationToken ct)
        {
            var actualizado = await _repositorio.ActualizarAsync(id, dto, ct);
            if (actualizado == null)
                return NotFound();

            return Ok(actualizado);
        }

        // DELETE: api/donantes/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(int id, CancellationToken ct)
        {
            var eliminado = await _repositorio.EliminarAsync(id, ct);
            if (!eliminado)
                return NotFound();

            return NoContent();
        }
    }
}
