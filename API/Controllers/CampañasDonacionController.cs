using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CampanasDonacionController : ControllerBase
    {
        private readonly ICampanaDonacionRepositorio _repo;

        public CampanasDonacionController(ICampanaDonacionRepositorio repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CampanaDonacionDto>>> Listar()
        {
            var campanas = await _repo.ListarCampanasAsync();
            return Ok(campanas);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Crear([FromBody] CrearCampanaDonacionDto dto)
        {
            if (dto == null) return BadRequest("Datos inválidos");

            var id = await _repo.CrearCampanaAsync(dto);
            return CreatedAtAction(nameof(Listar), new { id }, id);
        }
    }

}
