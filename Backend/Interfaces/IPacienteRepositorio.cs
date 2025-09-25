using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.DTOs;

namespace Backend.Interfaces
{
    internal interface IPacienteRepositorio
    {
        Task<IEnumerable<ListarPacienteDTO>> ListarPacientePacientesAsync(CancellationToken ct);
        Task<IEnumerable<ListarPacienteDTO>> ListarPacientePacientesPorTipoDeSangreAsync(string TipoSangre, CancellationToken ct);
        Task<ListarPacienteDTO> ObtenerPacientePorIdAsync(int id, CancellationToken ct);
        Task<int> RegistrarPacienteAsync(PacienteDTO paciente, CancellationToken ct);
        Task<ListarPacienteDTO?> ActualizarPacienteAsync(PacienteDTO paciente, CancellationToken ct);
        Task<bool> EliminarPacienteAsync(int id, CancellationToken ct);
    }
}
