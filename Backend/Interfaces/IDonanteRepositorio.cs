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
        Task<IEnumerable<ListarDonantesDTO>> ListarAsync(CancellationToken ct);
        Task<IEnumerable<ListarDonantesDTO>> ListarPorTipoSangreAsync(string nombre, CancellationToken ct);
        Task<ListarDonantesDTO?> ObtenerPorIdAsync(int id, CancellationToken ct); 
        Task<int> CrearAsync(RegistrarDonanteDTO dto, CancellationToken ct);
        Task<ListarDonantesDTO?> ActualizarAsync(int id, ActualizarDonanteDTO dto, CancellationToken ct);
        Task<bool> EliminarAsync(int id, CancellationToken ct);
    }
}
