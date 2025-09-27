using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Backend.DTOs;

namespace Backend.Interfaces
{
    public interface INotificacionRepositorio
    {
        Task<IEnumerable<ListarNotificacionDTO>> ListarPorDonanteAsync(int DonanteID, CancellationToken ct);
        Task<ListarNotificacionDTO?> ListarPorIdAsync(int id, CancellationToken ct);
        Task<int> CrearAsync(CrearNotificacionDTO dto, CancellationToken ct);
        Task<ListarNotificacionDTO?> ActualizarAsync(int id, ActualizarNotificacionDTO dto, CancellationToken ct);
        Task<bool> MarcarLeidaAsync(int id, bool leido, CancellationToken ct);
        Task<bool> EliminarAsync(int id, CancellationToken ct);
    }
}
