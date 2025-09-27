using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DTOs
{
    public class NotificacionDTO
    {
        public int NotificacionID { get; set; }
        public int UsuarioID { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public bool Leido { get; set; }
    }

    public class CrearNotificacionDTO
    {
        public int UsuarioID { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }

    public class ActualizarNotificacionDTO
    {
        public string Mensaje { get; set; } = string.Empty;
        public bool Leido { get; set; }
    }
}

