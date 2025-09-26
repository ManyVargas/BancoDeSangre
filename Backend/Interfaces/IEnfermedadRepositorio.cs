using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Backend.DTOs;

namespace Backend.Interfaces
{
    public interface IEnfermedadRepositorio
    {
        Task<int> CrearAsync(CrearEnfermedadDTO dto, CancellationToken ct);
        Task<IEnumerable<EnfermedadDTO>> ListarAsync(CancellationToken ct);
        Task<EnfermedadDTO?> ObtenerPorIdAsync(int id, CancellationToken ct);
        Task<EnfermedadDTO?> ActualizarAsync(int id, ActualizarEnfermedadDTO dto, CancellationToken ct);
        Task<bool> EliminarAsync(int id, CancellationToken ct);
    }
}