using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DTOs
{
    public class DonantesDTO
    {
        public int DonanteID { get; set; }
        public string Nombre { get; set; }
        public string CedulaID { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public int TipoSangreID { get; set; }
        public DateTime UltimaDonacion { get; set; }
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public bool Disponibilidad { get; set; }
    }

    public class RegistrarDonanteDTO 
    {
    public string Nombre { get; set; }
    public string CedulaID { get; set; }
	public DateTime FechaNacimiento { get; set; }
    public string Telefono { get; set; }
    public string Email { get; set; }
	public int  TipoSangreID { get; set; }
	public DateTime UltimaDonacion { get; set; }
    public decimal Latitud { get; set; }
	public decimal Longitud { get; set; }
    public bool    Disponibilidad { get; set; }

    }

    public class ActualizarDonanteDTO
    {
        public int DonanteID { get; set; }            
        public string Nombre { get; set; } = "";
        public string CedulaID { get; set; } = "";
        public DateTime FechaNacimiento { get; set; }
        public string? Telefono { get; set; }        
        public string Email { get; set; } = "";
        public int TipoSangreID { get; set; }
        public DateTime? UltimaDonacion { get; set; } 
        public decimal? Latitud { get; set; }         
        public decimal? Longitud { get; set; }        
        public bool Disponibilidad { get; set; }
    }
    public class EliminarDonanteDTO
    {
        public int DonanteID { get; set; }
    }
}
