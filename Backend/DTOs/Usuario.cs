using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DTOs
{
    // Salida (lo que devuelves al cliente)
    public class UsuarioDto
    {
        public int UsuarioID { get; set; }
        public string Email { get; set; } = "";
        public string NombreCompleto { get; set; } = "";
        public string? Telefono { get; set; }
        public int RolID { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? UltimoAcceso { get; set; }
        public DateTime? UltimaActividad { get; set; }
        public bool Estado { get; set; }
    }

    // Crear (lo que recibes al crear)
    public class UsuarioCrearDto
    {
        public string Email { get; set; } = "";
        public string Contrasena { get; set; } = "";
        public string NombreCompleto { get; set; } = "";
        public string? Telefono { get; set; }
        public int RolID { get; set; }
    }

    // Actualizar
    public class UsuarioActualizarDto
    {
        public string Email { get; set; } = "";
        public string Contrasena { get; set; } = ""; // plano (SP la hashea)
        public string NombreCompleto { get; set; } = "";
        public string? Telefono { get; set; }
        public int RolID { get; set; }
        public bool Estado { get; set; }
    }

    // Login
    public class LoginDto
    {
        public string Email { get; set; } = "";
        public string Contrasena { get; set; } = "";
    }

    public class LoginResultDto
    {
        public int UsuarioID { get; set; }
        public string Email { get; set; } = "";
        public string NombreCompleto { get; set; } = "";
        public int RolID { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Estado { get; set; }
    }

}
