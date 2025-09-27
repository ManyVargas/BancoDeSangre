using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DTOs
{
    // Para registrar la relación
    public class HistorialEnfermedadDTO
    {
        public int HistorialID { get; set; }
        public int EnfermedadID { get; set; }
    }

    // Para listar enfermedades de un historial
    public class HistorialEnfermedadDetalleDTO
    {
        public int HistorialID { get; set; }
        public int EnfermedadID { get; set; }
        public string NombreEnfermedad { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }
}

