using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DTOs
{
    public class EnfermedadDTO
    {
        public int EnfermedadID { get; set; }
        public string NombreEnfermedad { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }

    public class CrearEnfermedadDTO
    {
        public string NombreEnfermedad { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }

    public class ActualizarEnfermedadDTO
    {
        public string NombreEnfermedad { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }
}