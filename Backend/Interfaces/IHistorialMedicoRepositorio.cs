using Backend.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Interfaces
{
    public interface IHistorialMedicoRepositorio
    {
        Task<IEnumerable<HistorialMedicoDto>> ListarHistorialMedico(int? DonanteID, int? PacienteID);
        Task<int> RegistrarHistorialMedico(RegistrarHistorialMedicoDto registrarHistorialMedico);
        Task<HistorialMedicoDto?> ActualizarHistorialMedico(HistorialMedicoActualizarDto actualizarHistorialMedico);
    }
}
