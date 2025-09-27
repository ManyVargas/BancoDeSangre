using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Backend.DTOs;

namespace Backend.Interfaces
{
    public interface IEstatusInventarioRepositorio
    {
        Task<IEnumerable<EstatusInventarioDTO>> ListarAsync(CancellationToken ct);
        Task<EstatusInventarioDTO?> ObtenerPorIdAsync(int id, CancellationToken ct);
        Task<int> CrearAsync(CrearEstatusInventarioDTO dto, CancellationToken ct);
        Task<EstatusInventarioDTO?> ActualizarAsync(int id, ActualizarEstatusInventarioDTO dto, CancellationToken ct);
        Task<bool> EliminarAsync(int id, CancellationToken ct);
    }
}
