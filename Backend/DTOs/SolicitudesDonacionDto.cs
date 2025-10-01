using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DTOs
{
    public class SolicitudesDonacionDto
    {
        public int SolicituId { get; set; }
        public string NombreSolicitante { get; set; }
        public string TipoSangre { get; set; }
        public int CantidadRequerida { get; set; }
        public DateTime FechaLimite { get; set; }
        public string NombreHospital { get; set; }
        public string Estado { get; set; }
        public string DireccionPaciente { get; set; }
        public bool Anonimmo { get; set; }
        public DateTime FechaInicio { get; set; }
        public string NombreBanco { get; set; }
    }

    public class CrearSolicitudesDonacionDto
    {
        public int PacienteId { get; set; }
        public int TipoSangreId { get; set; }
        public int CantidadRequerida { get; set; }
        public DateTime FechaLimite{ get; set; }
        public int HospitalId {  get; set; }
        public string Estado { get; set; }
        public string DireccionPaciente { get; set; }
        public bool Anonimmo { get; set; }
        public DateTime FechaInicio { get; set; }
        public int BancoId { get; set; }
    }

    public class ActualizarEstadoSolicitudDto
    {
        public string Estado { get; set; }
    }
}
