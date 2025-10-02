using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BancoDeSangreController : ControllerBase
    {
        private readonly IBancoDeSangreRepositorio _repo;

        public BancoDeSangreController(IBancoDeSangreRepositorio repo)
        {
            _repo = repo;
        }

        // GET: api/BancosDeSangre/todos
        [HttpGet("todos")]
        public async Task<ActionResult<IEnumerable<BancoDeSangreDto>>> Listar()
        {
            try
            {
                var bancos = await _repo.ListarBancosDeSangre();
                if (bancos == null || !bancos.Any())
                    return NoContent();

                return Ok(bancos);
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"Error de base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al listar bancos de sangre: {ex.Message}");
            }
        }

        // POST: api/BancosDeSangre/crear
        [HttpPost("crear")]
        public async Task<ActionResult> Crear([FromBody] RegistrarBancoDeSangreDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var nuevoId = await _repo.RegistrarBancoDeSangre(dto);

                // 201 Created con el ID creado
                return StatusCode(StatusCodes.Status201Created,
                    new { mensaje = "Banco de sangre creado correctamente.", bancoID = nuevoId });
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                // Violación de índice único (por ejemplo, RNC o correo duplicado)
                return Conflict("Ya existe un banco de sangre con datos únicos duplicados (RNC/correo/etc.).");
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"Error de base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear el banco de sangre: {ex.Message}");
            }
        }
    }
}
