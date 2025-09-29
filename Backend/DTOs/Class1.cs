using System;

namespace Backend.DTOs
{
    /// <summary>
    /// Estructura que devuelven el SP y los SELECT (coincide con las columnas del SELECT del SP).
    /// </summary>
    public class ParticipacionCampanaDetalleDTO
    {
        public int ParticipacionID { get; set; }
        public int CampanaID { get; set; }
        public string NombreCampana { get; set; } = string.Empty;
        public int DonanteID { get; set; }
        public string NombreDonante { get; set; } = string.Empty;
        public DateTime FechaDonacion { get; set; }
        public int CantidadUnidades { get; set; }
    }

    /// <summary>
    /// Body del endpoint PUT {id}/cantidad (SP sp_ActualizarCantidadUnidades).
    /// </summary>
    public class ActualizarCantidadUnidadesDTO
    {
        public int CantidadUnidades { get; set; }
    }

    /// <summary>
    /// Body para POST (crear participación de campaña).
    /// </summary>
    public class CrearParticipacionCampanaDTO
    {
        public int CampanaID { get; set; }
        public int DonanteID { get; set; }
        public DateTime FechaDonacion { get; set; }
        public int CantidadUnidades { get; set; }
    }
}
