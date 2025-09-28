using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DTOs
{
    public class HistorialMedicoDto
    {
        public int HistorialID {  get; set; }
        public string Medicamentos { get; set; }
        public bool AptoParaDonar {  get; set; }
        public DateTime FehaRevision { get; set; }
        public int? DonanteID { get; set; }
        public int? PacienteID { get; set; }

    }

    public class RegistrarHistorialMedicoDto
    {
        public string Medicamentos { get; set; } = string.Empty;
        public bool AptoParaDonar { get; set; }
        public DateTime FechaRevision { get; set; }
        public int DonanteID {  get; set; }
        public int PacienteID { get; set; }
    }

    public class HistorialMedicoActualizarDto
    {
        // --- Valores NUEVOS que quieres guardar ---
        public int HistorialID { get; set; }                
        public string? Medicamentos { get; set; }           
        public bool? AptoParaDonar { get; set; }             
        public DateTime? FechaRevision { get; set; }         
        public int? DonanteID { get; set; }                 
        public int? PacienteID { get; set; }                
    }
}
