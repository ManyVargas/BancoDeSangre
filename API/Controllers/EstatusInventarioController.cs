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

        public EstatusInventarioController(IEstatusInventarioRepositorio repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EstatusInventarioDTO>>> Listar(CancellationToken ct)
        {
            //try
            //{
                var data = await _repo.ListarAsync(ct);
                return Ok(data);
            //}
            //catch (Exception ex)
            //{
            //    return Problem(ex.Message);
            //}
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EstatusInventarioDTO>> Obtener(int id, CancellationToken ct)
        {
            var item = await _repo.ObtenerPorIdAsync(id, ct);
            if (item == null) return NotFound();
            return Ok(item);
        }
    }
}
