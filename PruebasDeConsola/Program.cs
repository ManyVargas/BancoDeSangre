using Backend.Data;
using Backend.Interfaces;
using Backend.DTOs;
using Backend.Servicios;
using Microsoft.Extensions.Configuration;

// 1. Configuración para leer appsettings.json
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

// 2. Crear fábrica de conexión
var cf = new SqlConnectionFactory(config);

// 3. Crear repositorio
IUsuarioRepositorio repo = new UsuarioRepositorio(cf);

// 4. Insertar un usuario
var nuevoUsuario = new UsuarioCrearDto
{
    Email = "test@example.com",
    Contrasena = "MiClaveSegura123", // plano, se hashea en el repositorio
    NombreCompleto = "Usuario de Prueba",
    Telefono = "809-555-1234",
    RolID = 1 // debe existir un rol con ID=1
};

int id = await repo.CrearAsync(nuevoUsuario, CancellationToken.None);
Console.WriteLine($"Usuario insertado con ID: {id}");

// 5. Listar todos los usuarios
var usuarios = await repo.ListarAsync(CancellationToken.None);
foreach (var u in usuarios)
{
    Console.WriteLine($"{u.UsuarioID} - {u.Email} - {u.NombreCompleto} - Estado: {u.Estado}");
}

Console.ReadLine();