using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.Data.SqlClient;

namespace Backend.Data
{
    public class PacienteRepositorio : IPacienteRepositorio
    {

        private readonly IConnectionFactory _connectionFactory;
        public PacienteRepositorio(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }


        public async Task<ListarPacienteDTO> ActualizarPacienteAsync(PacienteDTO paciente, CancellationToken ct)
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("dbo.sp_EliminarPaciente", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@PacienteId", SqlDbType.Int) { Value = paciente.PacienteId });
            cmd.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 100) { Value = paciente.Nombre });
            cmd.Parameters.Add(new SqlParameter("@CedulaID", SqlDbType.NVarChar, 20) { Value = paciente.CedulaID });
            cmd.Parameters.Add(new SqlParameter("@FechaNacimiento", SqlDbType.Date) { Value = paciente.FechaNacimiento });
            cmd.Parameters.Add(new SqlParameter("@Telefono", SqlDbType.NVarChar, 15) { Value = paciente.Telefono });
            cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = paciente.Email });
            cmd.Parameters.Add(new SqlParameter("@Direccion", SqlDbType.NVarChar, 200) { Value = paciente.Direccion });
            cmd.Parameters.Add(new SqlParameter("@TipoSangreID", SqlDbType.Int) { Value = paciente.TipoSangreID });

            await con.OpenAsync(ct);

            using var rd = await cmd.ExecuteReaderAsync(ct);

            if (!rd.HasRows) throw new Exception("No se pudo actualizar el paciente.");

            await rd.ReadAsync(ct);

            var pacienteActualizado = new ListarPacienteDTO
            {
                PacienteId = rd.GetInt32(rd.GetOrdinal("PacienteId")),
                Nombre = rd.GetString(rd.GetOrdinal("Nombre")),
                CedulaID = rd.GetString(rd.GetOrdinal("CedulaID")),
                FechaNacimiento = rd.GetDateTime(rd.GetOrdinal("FechaNacimiento")),
                Telefono = rd.GetString(rd.GetOrdinal("Telefono")),
                Email = rd.GetString(rd.GetOrdinal("Email")),
                Direccion = rd.GetString(rd.GetOrdinal("Direccion")),
                TipoSangre = rd.GetString(rd.GetOrdinal("TipoSangre"))
            };

            throw new NotImplementedException();
        }

        public async Task<bool> EliminarPacienteAsync(int id, CancellationToken ct)

        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("dbo.sp_EliminarPaciente", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            await con.OpenAsync();

            cmd.Parameters.Add(new SqlParameter("@PacienteId", SqlDbType.Int) { Value = id });
            await con.OpenAsync(ct);

            var filasAfectadas = await cmd.ExecuteNonQueryAsync(ct);

            return filasAfectadas > 0;


            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ListarPacienteDTO>> ListarPacientePacientesAsync(CancellationToken ct)
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("dbo.sp_ListarPacientes", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            await con.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<ListarPacienteDTO>();

            while (reader.Read()) {
                var paciente = new ListarPacienteDTO
                {
                    PacienteId = reader.GetInt32(reader.GetOrdinal("PacienteId")),
                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                    CedulaID = reader.GetString(reader.GetOrdinal("CedulaID")),
                    FechaNacimiento = reader.GetDateTime(reader.GetOrdinal("FechaNacimiento")),
                    Telefono = reader.GetString(reader.GetOrdinal("Telefono")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    Direccion = reader.GetString(reader.GetOrdinal("Direccion")),
                    TipoSangre = reader.GetString(reader.GetOrdinal("TipoSangre"))
                };
                list.Add(paciente);
            }

            return list;
        }

        public async Task<IEnumerable<ListarPacienteDTO>> ListarPacientePacientesPorTipoDeSangreAsync(string tipoSangre,CancellationToken ct)
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("dbo.sp_ListarPacientePorId", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@TipoSangre", SqlDbType.NVarChar, 10) { Value = tipoSangre });

            await con.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<ListarPacienteDTO>();

            while (reader.Read())
            {
                var paciente = new ListarPacienteDTO
                {
                    PacienteId = reader.GetInt32(reader.GetOrdinal("PacienteId")),
                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                    CedulaID = reader.GetString(reader.GetOrdinal("CedulaID")),
                    FechaNacimiento = reader.GetDateTime(reader.GetOrdinal("FechaNacimiento")),
                    Telefono = reader.GetString(reader.GetOrdinal("Telefono")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    Direccion = reader.GetString(reader.GetOrdinal("Direccion")),
                    TipoSangre = reader.GetString(reader.GetOrdinal("TipoSangre"))
                };
                list.Add(paciente);
            }

            return list;
        }

        public async Task<ListarPacienteDTO> ObtenerPacientePorIdAsync(int id, CancellationToken ct)
        {

            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("dbo.sp_ListarPacientePorId", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            
            cmd.Parameters.Add(new SqlParameter("@PacienteId", SqlDbType.Int) { Value = id });

            await con.OpenAsync(ct);

            using var reader = await cmd.ExecuteReaderAsync(ct);

            if (await reader.ReadAsync(ct))
            {
                var paciente = new ListarPacienteDTO
                {
                    PacienteId = reader.GetInt32(reader.GetOrdinal("PacienteId")),
                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                    CedulaID = reader.GetString(reader.GetOrdinal("CedulaID")),
                    FechaNacimiento = reader.GetDateTime(reader.GetOrdinal("FechaNacimiento")),
                    Telefono = reader.GetString(reader.GetOrdinal("Telefono")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    Direccion = reader.GetString(reader.GetOrdinal("Direccion")),
                    TipoSangre = reader.GetString(reader.GetOrdinal("TipoSangre"))
                };
                return paciente;
            }
            return null!;
        }

        public async Task<int> RegistrarPacienteAsync(PacienteDTO paciente, CancellationToken ct)
        {

            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("dbo.sp_RegistrarPaciente", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 100) { Value = paciente.Nombre });
            cmd.Parameters.Add(new SqlParameter("@CedulaID", SqlDbType.NVarChar, 20) { Value = paciente.CedulaID });
            cmd.Parameters.Add(new SqlParameter("@FechaNacimiento", SqlDbType.Date) { Value = paciente.FechaNacimiento });
            cmd.Parameters.Add(new SqlParameter("@Telefono", SqlDbType.NVarChar, 15) { Value = paciente.Telefono });
            cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = paciente.Email });
            cmd.Parameters.Add(new SqlParameter("@Direccion", SqlDbType.NVarChar, 200) { Value = paciente.Direccion });
            cmd.Parameters.Add(new SqlParameter("@TipoSangreID", SqlDbType.Int) { Value = paciente.TipoSangreID });

            await con.OpenAsync(ct);

            using var rd = await cmd.ExecuteReaderAsync(ct);
            // el SP retorna la fila insertada; toma el ID
            if (!rd.HasRows) throw new Exception("No se pudo registrar el donante.");
            await rd.ReadAsync(ct);
            return rd.GetInt32(rd.GetOrdinal("DonanteID"));

            throw new NotImplementedException();
        }
    }
}
