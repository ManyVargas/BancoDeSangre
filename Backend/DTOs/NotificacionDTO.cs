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
        public int solicitudID { get; set; }
        public int DonanteID { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public DateTime FechaEnvio { get; set; }
        public bool Leido { get; set; }
    }

    public class CrearNotificacionDTO
    {
        public int DonanteID { get; set; }
        public int solicitudID { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }

    public class ActualizarNotificacionDTO
    {
        public string Mensaje { get; set; } = string.Empty;
    }

    public class ListarNotificacionDTO {
        public int NotificacionID { get; set; }
        public string NombreDonante { get; set; }
        public int SolicitudID { get; set; }
        public string Mensaje { get; set; }
        public DateTime FechaEnvio {get;set;}
        public bool Leida { get; set;}

    }
}

