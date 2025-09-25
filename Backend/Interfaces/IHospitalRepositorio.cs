using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.DTOs;

namespace Backend.Interfaces
{
    public interface IHospitalRepositorio
    {
        Task<IEnumerable<HospitalDTO>> ObtenerTodosLosHospitales(CancellationToken ct);
        Task<HospitalDTO> ObtenerHospitalPorNombre(string NombreHospital, CancellationToken ct);
        Task<int> RegistrarHospital(RegistrarHospitalDTO nuevoHospital, CancellationToken ct);
        Task<HospitalDTO> ActualizarHospital(int hospitalId,RegistrarHospitalDTO hospitalActualizado, CancellationToken ct);
        Task<bool> EliminarHospital(int hospitalId, CancellationToken ct);

    }
}
