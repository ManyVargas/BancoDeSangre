using Backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultadosPruebaController : ControllerBase
    {
        private readonly IResultadosPrueba _repositorio;

        public ResultadosPruebaController(IResultadosPrueba repositorio)
        {
            _repositorio = repositorio;
        }

        [HttpGet("ObtenerResultadosPrueba")]
        public async Task<IActionResult> ObtenerResultadosPrueba(CancellationToken ct)
        {
            var resultados = await _repositorio.ListarResultadosPruebas(ct);
            return Ok(resultados);
        }
    }
}
