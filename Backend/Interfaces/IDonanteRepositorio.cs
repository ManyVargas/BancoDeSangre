using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.DTOs;

namespace Backend.Interfaces
{
    public interface IDonanteRepositorio
    {
        Task<IEnumerable<DonantesDTO>> ListarAsync(CancellationToken ct);
        Task<IEnumerable<DonantesDTO>> ListarPorTipoSangreAsync(string nombre, CancellationToken ct);
        Task<DonantesDTO?> ObtenerPorIdAsync(int id, CancellationToken ct); 
        Task<int> CrearAsync(RegistrarDonanteDTO dto, CancellationToken ct);
        Task<DonantesDTO?> ActualizarAsync(int id, ActualizarDonanteDTO dto, CancellationToken ct);
        Task<bool> EliminarAsync(int id, CancellationToken ct);
    }
}
