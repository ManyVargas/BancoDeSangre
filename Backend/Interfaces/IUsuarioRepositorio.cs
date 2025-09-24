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
        Task<IEnumerable<UsuarioDto>> ListarAsync(CancellationToken ct);
        Task<UsuarioDto?> ObtenerPorIdAsync(int id, CancellationToken ct); // (ver nota más abajo)
        Task<int> CrearAsync(UsuarioCrearDto dto, CancellationToken ct);
        Task<UsuarioDto?> ActualizarAsync(int id, UsuarioActualizarDto dto, CancellationToken ct);
        Task<bool> EliminarAsync(int id, CancellationToken ct); // ver nota de SP
        Task<LoginResultDto?> AutenticarAsync(LoginDto dto, CancellationToken ct);
    }
}
