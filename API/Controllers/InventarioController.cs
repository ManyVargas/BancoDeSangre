using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventarioController : ControllerBase
    {
        private readonly IInventarioRepositorio _repo;

        public InventarioController(IInventarioRepositorio repo)
        {
            _repo = repo;
        }

        // GET: api/Inventario/{bancoId}
        // Lista TODO el inventario de un banco.
        [HttpGet("{bancoId:int}")]
        public async Task<ActionResult<IEnumerable<InventarioDto>>> ListarPorBanco(int bancoId)
        {
            if (bancoId <= 0) return BadRequest("BancoID inválido.");

            try
            {
                // Si tu interfaz no tiene este método, usa el que retorna uno y adáptalo.
                var items = await _repo.ListarInventario(bancoId);
                if (items == null || !items.Any()) return NoContent();
                return Ok(items);
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"Error de base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al listar inventario: {ex.Message}");
            }
        }

        // POST: api/Inventario
        // Crea un registro de inventario (FechaActualizacion = ahora en el SP).
        [HttpPost]
        public async Task<ActionResult<InventarioDto>> Registrar([FromBody] RegistrarInventarioDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto.BancoID <= 0) return BadRequest("BancoID inválido.");
            if (dto.TipoDeSangreID <= 0) return BadRequest("TipoDeSangreID inválido.");
            if (dto.CantidadesUnidades <= 0) return BadRequest("CantidadesUnidades debe ser > 0.");

            try
            {
                var creado = await _repo.RegistrarInventario(dto);
                // No tenemos GET por InventarioID, devolvemos 201 con el recurso creado.
                return StatusCode(StatusCodes.Status201Created, creado);
            }
            //Códigos THROW del SP de registrar
            catch (SqlException ex) when (ex.Number is 52001 or 52002)
            {
                return BadRequest(ex.Message); // validaciones de cantidad/fecha
            }
            catch (SqlException ex) when (ex.Number is 52003 or 52004 or 52005 or 547)
            {
                return BadRequest("Alguna clave foránea no existe (Banco/Tipo de sangre/Estatus).");
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"Error de base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al registrar inventario: {ex.Message}");
            }
        }

        // PUT: api/Inventario
        // Actualiza la cantidad de un tipo de sangre en un banco.
        [HttpPut]
        public async Task<ActionResult<InventarioDto>> Actualizar([FromBody] ActualizarInventarioDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto.BancoID <= 0) return BadRequest("BancoID inválido.");
            if (dto.TipoDeSangreID <= 0) return BadRequest("TipoDeSangreID inválido.");
            if (dto.CantidadesUnidades < 0) return BadRequest("CantidadesUnidades no puede ser negativa.");

            try
            {
                var actualizado = await _repo.ActualizarInventario(dto.BancoID, dto.TipoDeSangreID, dto.CantidadesUnidades);
                return Ok(actualizado);
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                return BadRequest("BancoID o TipoDeSangreID no existe (violación de clave foránea).");
            }
            catch (Exception ex)
            {
                // Tu SP hace UPDATE y luego SELECT. Si no hay match, el repo lanza Exception.
                if (ex.Message.Contains("No se encontró inventario", StringComparison.OrdinalIgnoreCase))
                    return NotFound("No existe un registro de inventario para ese banco y tipo de sangre.");
                return StatusCode(500, $"Error al actualizar inventario: {ex.Message}");
            }
        }
    }
}
