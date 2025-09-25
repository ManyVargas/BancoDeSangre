using Backend.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Interfaces
{
    public interface IUsuarioRepositorio
    {
        Task<IEnumerable<UsuarioDto>> ListarAsync();
        Task<UsuarioDto?> ObtenerPorIdAsync(int id); // (ver nota más abajo)
        Task<int> CrearAsync(UsuarioCrearDto dto);
        Task<UsuarioDto?> ActualizarAsync(int id, UsuarioActualizarDto dto);
        Task<bool> EliminarAsync(int id); // ver nota de SP
        Task<LoginResultDto?> AutenticarAsync(LoginDto dto);
    }
}
