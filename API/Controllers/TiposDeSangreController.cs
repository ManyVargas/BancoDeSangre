using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TiposDeSangreController : ControllerBase
    {

        private readonly ITiposSangreRepositorio _repositorio;

        public TiposDeSangreController(ITiposSangreRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TiposSangreDTO>>> Listar(CancellationToken ct)
        {
            var tipos = await _repositorio.ListarTiposSangreAsync(ct);
            return Ok(tipos);
        }
    }
}
