using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DTOs
{
    public class RegistrarBancoDeSangreDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Direccion {  get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public decimal Latitud {  get; set; }
        public decimal Longitud { get; set; }
        public string SitioWeb { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public string RNC { get; set; } = string.Empty;

    }
}
