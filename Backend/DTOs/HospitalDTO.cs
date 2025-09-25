using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DTOs
{
    public class HospitalDTO
    {
        public int HospitalID { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }
        public string ContactoPrincipal { get; set; }
        public string EmailContacto { get; set; }
    }

    public class RegistrarHospitalDTO
    {
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }
        public string ContactoPrincipal { get; set; }
        public string EmailContacto { get; set; }
    }
}
