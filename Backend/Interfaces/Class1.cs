using System.Threading;
using System.Threading.Tasks;
using Backend.DTOs;

namespace Backend.Interfaces
{
    public interface IParticipacionCampanaRepositorio
    {
        // SP: actualizar cantidad y devolver detalle con joins
        Task<ParticipacionCampanaDetalleDTO?> ActualizarCantidadUnidadesAsync(
            int participacionId, int cantidadUnidades, CancellationToken ct);

        // CRUD mínimos según lo que necesitas
        Task<int> CrearAsync(CrearParticipacionCampanaDTO dto, CancellationToken ct);
        Task<ParticipacionCampanaDetalleDTO?> ObtenerPorIdAsync(int id, CancellationToken ct);
        Task<bool> EliminarAsync(int id, CancellationToken ct);
    }
}
