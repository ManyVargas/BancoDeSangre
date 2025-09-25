using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DTOs
{
    public class PacienteDTO
    {
        public int PacienteId { get; set; }
        public string Nombre { get; set; }
        public string CedulaID { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Direccion { get; set; }
        public int TipoSangreID { get; set; }
    }

    public class RegistrarPacienteDTO
    {
        public string Nombre { get; set; }
        public string CedulaID { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Direccion { get; set; }
        public int TipoSangreID { get; set; }
    }

    public class ListarPacienteDTO
    {
        public int PacienteId { get; set; }
        public string Nombre { get; set; }
        public string CedulaID { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Direccion { get; set; }
        public string TipoSangre { get; set; }
    }
}
