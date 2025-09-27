using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Backend.DTOs;

namespace Backend.Interfaces
{
    public interface IHistorialEnfermedadRepositorio
    {
        Task<bool> AgregarAsync(HistorialEnfermedadDTO dto, CancellationToken ct);
        Task<IEnumerable<HistorialEnfermedadDetalleDTO>> ListarPorHistorialAsync(int historialId, CancellationToken ct);
    }
}
