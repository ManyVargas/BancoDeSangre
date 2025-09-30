using Backend.DTOs;
using Backend.Interfaces;
using Backend.Servicios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IJwtService _jwt;

        public UsuarioController(IUsuarioRepositorio usuarioRepositorio, IJwtService jwt)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _jwt = jwt;
        }

        [HttpGet("todosUsuarios")]
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> ListarUsuarios()
        {
            try
            {

                var usuarios = await _usuarioRepositorio.ListarAsync();

                if (usuarios == null || !usuarios.Any())
                {
                    return NoContent();
                }

                return Ok(usuarios);
                
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                              $"Error al obtener los usuarios: {ex.Message}");
            }
        }

        [HttpPost("crearUsuario")]
        public async Task<ActionResult> CrearUsuario([FromBody]UsuarioCrearDto crearDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var nuevoUsuario = await _usuarioRepositorio.CrearAsync(crearDto);

                return StatusCode(StatusCodes.Status201Created, new {Mensaje = "Usuario creado correctamente.",usuarioID = nuevoUsuario});
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                // 2627/2601 = violación de UNIQUE (email duplicado, por ejemplo)
                return Conflict("Ya existe un usuario con ese correo electrónico.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                              $"Error al crear el usuario: {ex.Message}");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> LoginUsuario([FromBody] LoginDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var user = await _usuarioRepositorio.AutenticarAsync(dto);
                if (user is null) return Unauthorized("Credenciales inválidas.");
                if (!user.Estado) return Forbid("Usuario inactivo.");

                var token = _jwt.Generar(user);
                return Ok(new { token, user });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"Error de base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al autenticar: {ex.Message}");
            }
        }

        // PUT: api/Usuarios/5
        [HttpPut("{id:int}")]
        //[Authorize(Roles = "Admin")] // <-- opcional si manejas roles
        public async Task<ActionResult<UsuarioDto>> ActualizarUsuario(int id, [FromBody] UsuarioActualizarDto dto)
        {
            try
            {
                // Con [ApiController], si dto es inválido devuelve 400 automático.
                if (id <= 0) return BadRequest("Id inválido.");

                // (Opcional) Si tu DTO incluye Id, puedes validar que coincidan:
                // if (dto.UsuarioID != 0 && dto.UsuarioID != id) return BadRequest("Id de ruta y cuerpo no coinciden.");

                var actualizado = await _usuarioRepositorio.ActualizarAsync(id, dto);

                if (actualizado is null)
                    return NotFound("Usuario no encontrado.");

                // 200 OK con el recurso actualizado (PUT suele devolver 200/204)
                return Ok(actualizado);
                // Si prefieres 204 sin cuerpo:
                // return NoContent();
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                // violación de UNIQUE (por ejemplo, Email duplicado)
                return Conflict("Ya existe un usuario con ese correo electrónico.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  $"Error al actualizar el usuario: {ex.Message}");
            }
        }
    }
}
