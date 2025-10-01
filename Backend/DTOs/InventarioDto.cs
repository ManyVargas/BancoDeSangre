using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DTOs
{
    public class InventarioDto
    {
        public int InventarioID { get; set; }
        public string NombreDeBanco { get; set; }
        public string TipoDeSangre { get; set; }
        public int CantidadesUnidades { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public int EstatusID { get; set; }
    }

    public class RegistrarInventarioDto
    {
        public int BancoID {  get; set; }
        public int TipoDeSangreID {  get; set; }
        public int CantidadesUnidades {  get; set; }
        public DateTime FechaVencimiento { get; set; }
        public int EstatusID { get; set; }
    }

    public class ActualizarInventarioDto
    {
        public int BancoID { get; set; }
        public int TipoDeSangreID { get; set; }
        public int CantidadesUnidades { get; set; }
    }
}
