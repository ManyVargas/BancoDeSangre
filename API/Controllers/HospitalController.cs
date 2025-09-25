using Backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HospitalController : ControllerBase
    {
        private readonly IHospitalRepositorio _hospitalRepositorio;

        public HospitalController(IHospitalRepositorio hospitalRepositorio)
        {
            _hospitalRepositorio = hospitalRepositorio;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodosLosHospitales(CancellationToken ct)
        {
            var hospitales = await _hospitalRepositorio.ObtenerTodosLosHospitales(ct);
            return Ok(hospitales);
        }

        [HttpGet("{nombre}")]
        public async Task<IActionResult> ObtenerHospitalPorNombre(string nombre, CancellationToken ct)
        {
            var hospital = await _hospitalRepositorio.ObtenerHospitalPorNombre(nombre, ct);
            if (hospital == null)
                return NotFound();
            return Ok(hospital);
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarHospital([FromBody] Backend.DTOs.RegistrarHospitalDTO nuevoHospital, CancellationToken ct)
        {
            var nuevoId = await _hospitalRepositorio.RegistrarHospital(nuevoHospital, ct);
            return CreatedAtAction(nameof(ObtenerHospitalPorNombre), new { nombre = nuevoHospital.Nombre }, nuevoId);
        }

        [HttpPut("{hospitalId}")]
        public async Task<IActionResult> ActualizarHospital(int hospitalId, [FromBody] Backend.DTOs.RegistrarHospitalDTO hospitalActualizado, CancellationToken ct)
        {
            var hospital = await _hospitalRepositorio.ActualizarHospital(hospitalId, hospitalActualizado, ct);
            if (hospital == null)
                return NotFound();
            return Ok(hospital);
        }

        [HttpDelete("{hospitalId}")]
        public async Task<IActionResult> EliminarHospital(int hospitalId, CancellationToken ct)
        {
            var eliminado = await _hospitalRepositorio.EliminarHospital(hospitalId, ct);
            if (!eliminado)
                return NotFound();
            return NoContent();
        }

    }
}
