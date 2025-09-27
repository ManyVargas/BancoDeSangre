using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Backend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistorialEnfermedadController : ControllerBase
    {
        private readonly IHistorialEnfermedadRepositorio _repo;
        private readonly ILogger<HistorialEnfermedadController> _logger;

        public HistorialEnfermedadController(
            IHistorialEnfermedadRepositorio repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        // GET: api/HistorialEnfermedad/5   -> enfermedades de un historial
        [HttpGet("{historialId:int}")]
        [ProducesResponseType(typeof(IEnumerable<HistorialEnfermedadDetalleDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListarPorHistorial(int historialId, CancellationToken ct)
        {
            var data = await _repo.ListarPorHistorialAsync(historialId, ct);
            return Ok(data);
        }

        // POST: api/HistorialEnfermedad     -> agrega relación HistorialID-EnfermedadID
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Agregar([FromBody] HistorialEnfermedadDTO dto, CancellationToken ct)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            try
            {
                await _repo.AgregarAsync(dto, ct);
                return NoContent();
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error al registrar enfermedad en el historial");
                return Problem($"Error de base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al registrar enfermedad en el historial");
                return Problem("Error inesperado al registrar enfermedad en el historial.");
            }
        }
    }
}
