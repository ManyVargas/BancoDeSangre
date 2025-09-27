using Backend.DTOs;

namespace Backend.Interfaces
{
    public interface IRolRepositorio
    {
        Task<IEnumerable<RolDTO>> ListarAsync(CancellationToken ct);
        Task<RolDTO?> ObtenerPorIdAsync(int id, CancellationToken ct); // (opcional si tu SP lo soporta)
        Task<int> CrearAsync(CrearRolDTO dto, CancellationToken ct);
        Task<RolDTO?> ActualizarAsync(int id, ActualizarRolDTO dto, CancellationToken ct);
        Task<bool> EliminarAsync(int id, CancellationToken ct);
    }
}
