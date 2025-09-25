using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Servicios;

namespace Backend.Data
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly IConnectionFactory _connectionFactory;
        public UsuarioRepositorio(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<UsuarioDto?> ActualizarAsync(int id, UsuarioActualizarDto dto)
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("dbo.sp_ActualizarUsuario", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            var hash = PasswordHasher.Sha512Hex(dto.Contrasena);
            cmd.Parameters.Add(new SqlParameter("@UsuarioID", SqlDbType.Int) { Value = id });
            cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = dto.Email });
            cmd.Parameters.Add(new SqlParameter("@ContrasenaHash", SqlDbType.NVarChar, 255) { Value = hash }); // SP hashea
            cmd.Parameters.Add(new SqlParameter("@NombreCompleto", SqlDbType.NVarChar, 100) { Value = dto.NombreCompleto });
            cmd.Parameters.Add(new SqlParameter("@Telefono", SqlDbType.VarChar, 20) { Value = (object?)dto.Telefono ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@RolID", SqlDbType.Int) { Value = dto.RolID });
            cmd.Parameters.Add(new SqlParameter("@Estado", SqlDbType.Bit) { Value = dto.Estado });

            await con.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();

            if (!reader.HasRows) return null;

            await reader.ReadAsync();

            return new UsuarioDto
            {
                UsuarioID = reader.GetInt32(reader.GetOrdinal("UsuarioID")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                NombreCompleto = reader.GetString(reader.GetOrdinal("NombreCompleto")),
                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString(reader.GetOrdinal("Telefono")),
                RolID = reader.GetInt32(reader.GetOrdinal("RolID")),
                FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro")),
                UltimaActividad = reader.IsDBNull(reader.GetOrdinal("UltimaActividad")) ? null : reader.GetDateTime(reader.GetOrdinal("UltimaActividad")),
                Estado = reader.GetBoolean(reader.GetOrdinal("Estado"))
            };
        }

        public async Task<LoginResultDto?> AutenticarAsync(LoginDto dto)
        {
            using var con = _connectionFactory.Create();

            using var cmd = new SqlCommand("dbo.sp_AutenticarUsuario", con)
            { 
                CommandType = CommandType.StoredProcedure 
            };

            var hash = PasswordHasher.Sha512Hex(dto.Contrasena);
            cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = dto.Email });
            cmd.Parameters.Add(new SqlParameter("@ContrasenaHash", SqlDbType.NVarChar, 255) { Value = hash });

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (!reader.HasRows) return null;
            await reader.ReadAsync();

            return new LoginResultDto
            {
                UsuarioID = reader.GetInt32(reader.GetOrdinal("UsuarioID")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                NombreCompleto = reader.GetString(reader.GetOrdinal("NombreCompleto")),
                RolID = reader.GetInt32(reader.GetOrdinal("RolID")),
                FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro")),
                Estado = reader.GetBoolean(reader.GetOrdinal("Estado"))
            };
        }

        public async Task<int> CrearAsync(UsuarioCrearDto dto)
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("dbo.sp_RegistrarUsuario", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            var hash = PasswordHasher.Sha512Hex(dto.Contrasena);

            cmd.Parameters.Add(new SqlParameter("@ContrasenaHash", SqlDbType.NVarChar, 255) { Value = hash });
            cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = dto.Email });
            cmd.Parameters.Add(new SqlParameter("@NombreCompleto", SqlDbType.NVarChar, 100) { Value = dto.NombreCompleto });
            cmd.Parameters.Add(new SqlParameter("@Telefono", SqlDbType.VarChar, 20) { Value = (object?)dto.Telefono ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@RolID", SqlDbType.Int) { Value = dto.RolID });
            cmd.Parameters.Add(new SqlParameter("@FechaRegistro", SqlDbType.DateTime2) { Value = DateTime.UtcNow });
            cmd.Parameters.Add(new SqlParameter("@UltimoAcceso", SqlDbType.DateTime2) { Value = DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@UltimaActividad", SqlDbType.DateTime2) { Value = DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Estado", SqlDbType.Bit) { Value = true });

            await con.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();
            // el SP retorna la fila insertada; toma el ID
            if (!rd.HasRows) throw new Exception("No se pudo registrar el usuario.");
            await rd.ReadAsync();
            return rd.GetInt32(rd.GetOrdinal("UsuarioID"));

        }

        public Task<bool> EliminarAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<UsuarioDto>> ListarAsync()
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("dbo.sp_ListarUsuarios", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<UsuarioDto>();
            while(await reader.ReadAsync())
            {
                list.Add(new UsuarioDto
                {
                    UsuarioID = reader.GetInt32(reader.GetOrdinal("UsuarioID")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    NombreCompleto = reader.GetString(reader.GetOrdinal("NombreCompleto")),
                    Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString(reader.GetOrdinal("Telefono")),
                    RolID = reader.GetInt32(reader.GetOrdinal("RolID")),
                    FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro")),
                    UltimoAcceso = reader.IsDBNull(reader.GetOrdinal("UltimoAcceso")) ? null : reader.GetDateTime(reader.GetOrdinal("UltimoAcceso")),
                    UltimaActividad = reader.IsDBNull(reader.GetOrdinal("UltimaActividad")) ? null : reader.GetDateTime(reader.GetOrdinal("UltimaActividad")),
                    Estado = reader.GetBoolean(reader.GetOrdinal("Estado"))
                });
            }
            return list;
        }

        public Task<UsuarioDto?> ObtenerPorIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
