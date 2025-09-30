using System.Threading;
using System.Threading.Tasks;
using Backend.DTOs;

namespace Backend.Interfaces
{
    public interface IParticipacionCampanaRepositorio
    {
       
        // CRUD mínimos según lo que necesitas
        Task<int> CrearAsync(CrearParticipacionCampanaDTO dto, CancellationToken ct);
        Task<ListarParticipantesCampana?> ObtenerPorPaticipantesPorIdAsync(int id, CancellationToken ct);
    }
}
