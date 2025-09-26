using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PruebasLaboratorioController : ControllerBase
    {
        private readonly IPruebasLaboratorio _pruebasLaboratorio;

        public PruebasLaboratorioController(IPruebasLaboratorio pruebasLaboratorio)
        {
            _pruebasLaboratorio = pruebasLaboratorio;
        }

        // GET: api/pruebaslaboratorio
        [HttpGet]
        public async Task<IActionResult> GetPruebasLaboratorio(CancellationToken ct)
        {
            var pruebas = await _pruebasLaboratorio.ListarPruebasLaboratorio(ct);
            return Ok(pruebas);
        }

        // GET: api/pruebaslaboratorio/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPruebaLaboratorioPorId(int id, CancellationToken ct)
        {
            var prueba = await _pruebasLaboratorio.ListarPruebaPorID(id, ct);
            if (prueba == null)
                return NotFound($"No se encontró la prueba con ID {id}");
            return Ok(prueba);
        }

        // GET: api/pruebaslaboratorio/donante/{donanteId}
        [HttpGet("donante/{donanteId:int}")]
        public async Task<IActionResult> GetPruebasPorDonante(int donanteId, CancellationToken ct)
        {
            var pruebas = await _pruebasLaboratorio.ListarPruebasLaboratorioPorDonante(donanteId, ct);
            return Ok(pruebas);
        }

        // GET: api/pruebaslaboratorio/tipo/{tipo}
        [HttpGet("tipo/{tipo}")]
        public async Task<IActionResult> GetPruebasPorTipo(string tipo, CancellationToken ct)
        {
            var pruebas = await _pruebasLaboratorio.ListarPruebaPorTipo(tipo, ct);
            return Ok(pruebas);
        }

        // GET: api/pruebaslaboratorio/rango-fechas
        [HttpGet("rango-fechas")]
        public async Task<IActionResult> GetPruebasPorRangoDeFechas(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin,
            CancellationToken ct)
        {
            var pruebas = await _pruebasLaboratorio.ListarPruebasPorRangoDeFechas(fechaInicio, fechaFin, ct);
            return Ok(pruebas);
        }

        // POST: api/pruebaslaboratorio
        [HttpPost]
        public async Task<IActionResult> RegistrarPruebaLaboratorio([FromBody] RegistrarPruebaLaboratorioDTO dto, CancellationToken ct)
        {
            var nuevoId = await _pruebasLaboratorio.RegistrarPruebaLaboratorio(dto, ct);
            return CreatedAtAction(nameof(GetPruebaLaboratorioPorId), new { id = nuevoId }, nuevoId);
        }

        // PUT: api/pruebaslaboratorio/{id}
        [HttpPut]
        public async Task<IActionResult> ActualizarResultadoPrueba(int PruebaID, int ResultadoID, string Observaciones, [FromBody] ActualizarResultadoPrueba dto, CancellationToken ct)
        {
            var pruebaActualizada = await _pruebasLaboratorio.ActualizarResultadoPrueba(PruebaID,ResultadoID,Observaciones, dto, ct);
            if (pruebaActualizada == null)
                return NotFound($"No se encontró la prueba con ID {PruebaID}");
            return Ok(pruebaActualizada);
        }

        // DELETE: api/pruebaslaboratorio/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarPruebaLaboratorio(int id, CancellationToken ct)
        {
            var eliminado = await _pruebasLaboratorio.EliminarPruebaLaboratorio(id, ct);
            if (!eliminado)
                return NotFound($"No se encontró la prueba con ID {id}");
            return NoContent();
        }
    }
}