using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DTOs
{
    public class PruebasLaboratorioDTO
    {
        public int PruebaID { get; set; }
        public int DonanteID { get; set; }
        public int ResultadoID { get; set; }
        public string TipoPrueba { get; set; }
        public DateTime FechaPrueba { get; set; }
        public string Laboratorio { get; set; }
        public DateTime ValidezHasta { get; set; }
        public string Observaciones { get; set; }
    }
    public class RegistrarPruebaLaboratorioDTO
    {
        public int DonanteID { get; set; }
        public int ResultadoID { get; set; }
        public string TipoPrueba { get; set; }
        public DateTime FechaPrueba { get; set; }
        public string Laboratorio { get; set; }
        public DateTime ValidezHasta { get; set; }
        public string Observaciones { get; set; }
    }

    public class ListarPruebasLaboratorioDTO
    {
        public int PruebaID { get; set; }
        public string NombreDonante { get; set; }
        public string Resultado { get; set; }
        public string TipoPrueba { get; set; }
        public DateTime FechaPrueba { get; set; }
        public string Laboratorio { get; set; }
        public DateTime ValidezHasta { get; set; }
        public string Observaciones { get; set; }
    }

    public class ActualizarResultadoPrueba
    {

        public int PruebaID { get; set; }
        public int Resultado { get; set; }
        public string Observaciones { get; set; }

 
    }
}
