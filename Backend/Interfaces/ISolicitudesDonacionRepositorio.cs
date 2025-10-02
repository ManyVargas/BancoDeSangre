using Backend.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Interfaces
{
    public interface ISolicitudesDonacionRepositorio
    {
        Task<IEnumerable<SolicitudesDonacionDto>> ListarSolicitudes();
        Task<IEnumerable<SolicitudesDonacionDto>> ListarSolicitudesPorEstado(string estado);
        Task<SolicitudesDonacionDto> ActualizarEstadoSolicitud(int solicitudId, string estado);
        Task<int> CrearSolicitudesDonacion(CrearSolicitudesDonacionDto solicitudesDonacionDto);
    }
}
