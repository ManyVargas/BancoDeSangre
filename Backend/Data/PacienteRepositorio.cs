using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.PortableExecutable;
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
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<ListarPacienteDTO> ActualizarPacienteAsync(PacienteDTO paciente, CancellationToken ct)
        {
            if (paciente == null)
                throw new ArgumentNullException(nameof(paciente));

            try
            {
                using var con = _connectionFactory.Create();
                using var cmd = new SqlCommand("dbo.sp_ActualizarPaciente", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@PacienteID", SqlDbType.Int) { Value = paciente.PacienteId });
                cmd.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 100) { Value = paciente.Nombre ?? throw new ArgumentException("Nombre no puede ser nulo", nameof(paciente.Nombre)) });
                cmd.Parameters.Add(new SqlParameter("@CedulaID", SqlDbType.VarChar, 20) { Value = paciente.CedulaID ?? throw new ArgumentException("CedulaID no puede ser nulo", nameof(paciente.CedulaID)) });
                cmd.Parameters.Add(new SqlParameter("@FechaNacimiento", SqlDbType.Date) { Value = paciente.FechaNacimiento });
                cmd.Parameters.Add(new SqlParameter("@Telefono", SqlDbType.VarChar, 20) { Value = (object?)paciente.Telefono ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = (object?)paciente.Email ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Direccion", SqlDbType.NVarChar, 200) { Value = (object?)paciente.Direccion ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@TipoSangreID", SqlDbType.Int) { Value = paciente.TipoSangreID });

                await con.OpenAsync(ct);

                using var rd = await cmd.ExecuteReaderAsync(ct);

                if (!rd.HasRows)
                    throw new InvalidOperationException($"No se encontró el paciente con ID {paciente.PacienteId} para actualizar o no se pudo actualizar.");

                await rd.ReadAsync(ct);

                var pacienteActualizado = new ListarPacienteDTO
                {
                    PacienteId = rd.GetInt32("PacienteID"),
                    Nombre = rd.GetString("Nombre"),
                    CedulaID = rd.GetString("CedulaID"),
                    FechaNacimiento = rd.GetDateTime("FechaNacimiento"),
                    Telefono = rd.IsDBNull("Telefono") ? null : rd.GetString("Telefono"),
                    Email = rd.IsDBNull("Email") ? null : rd.GetString("Email"),
                    Direccion = rd.IsDBNull("Direccion") ? null : rd.GetString("Direccion"),
                    TipoSangre = rd.GetString("TipoSangre")
                };

                return pacienteActualizado;
            }
            catch (SqlException sqlEx)
            {
                // Manejo específico de errores de SQL Server
                if (sqlEx.Number == 2627 || sqlEx.Number == 2601) // Error de clave duplicada
                {
                    throw new InvalidOperationException($"Ya existe un paciente con la cédula {paciente.CedulaID}.", sqlEx);
                }
                else if (sqlEx.Number == 547) // Error de clave foránea
                {
                    throw new InvalidOperationException($"El TipoSangreID {paciente.TipoSangreID} no existe.", sqlEx);
                }
                else
                {
                    throw new InvalidOperationException($"Error al actualizar paciente: {sqlEx.Message}", sqlEx);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error inesperado al actualizar paciente: {ex.Message}", ex);
            }
        }

        public async Task<bool> EliminarPacienteAsync(int id, CancellationToken ct)
        {
            if (id <= 0)
                throw new ArgumentException("ID debe ser mayor a 0", nameof(id));

            try
            {
                using var con = _connectionFactory.Create();
                using var cmd = new SqlCommand("dbo.sp_EliminarPaciente", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@PacienteID", SqlDbType.Int) { Value = id });

                await con.OpenAsync(ct);

                using var reader = await cmd.ExecuteReaderAsync(ct);

                if (await reader.ReadAsync(ct))
                {
                    var filasAfectadas = reader.GetInt32("FilasAfectadas");
                    return filasAfectadas > 0;
                }

                return false;
            }
            catch (SqlException sqlEx)
            {
                throw new InvalidOperationException($"Error al eliminar paciente: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error inesperado al eliminar paciente: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<ListarPacienteDTO>> ListarPacientePacientesAsync(CancellationToken ct)
        {
            try
            {
                using var con = _connectionFactory.Create();
                using var cmd = new SqlCommand("dbo.sp_ListarPacientes", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                await con.OpenAsync(ct);

                using var reader = await cmd.ExecuteReaderAsync(ct);

                var list = new List<ListarPacienteDTO>();

                while (await reader.ReadAsync(ct))
                {
                    var paciente = new ListarPacienteDTO
                    {
                        PacienteId = reader.GetInt32("PacienteID"),
                        Nombre = reader.GetString("Nombre"),
                        CedulaID = reader.GetString("CedulaID"),
                        FechaNacimiento = reader.GetDateTime("FechaNacimiento"),
                        Telefono = reader.IsDBNull("Telefono") ? null : reader.GetString("Telefono"),
                        Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                        Direccion = reader.IsDBNull("Direccion") ? null : reader.GetString("Direccion"),
                        TipoSangre = reader.GetString("TipoSangre")
                    };
                    list.Add(paciente);
                }

                return list;
            }
            catch (SqlException sqlEx)
            {
                throw new InvalidOperationException($"Error al listar pacientes: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error inesperado al listar pacientes: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<ListarPacienteDTO>> ListarPacientePacientesPorTipoDeSangreAsync(string tipoSangre, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(tipoSangre))
                throw new ArgumentException("Tipo de sangre no puede ser nulo o vacío", nameof(tipoSangre));

            try
            {
                using var con = _connectionFactory.Create();
                using var cmd = new SqlCommand("dbo.sp_ListarPacientesPorTipoSangre", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@TipoSangre", SqlDbType.NVarChar, 50) { Value = tipoSangre });

                await con.OpenAsync(ct);

                using var reader = await cmd.ExecuteReaderAsync(ct);

                var list = new List<ListarPacienteDTO>();

                while (await reader.ReadAsync(ct))
                {
                    var paciente = new ListarPacienteDTO
                    {
                        PacienteId = reader.GetInt32("PacienteID"),
                        Nombre = reader.GetString("Nombre"),
                        CedulaID = reader.GetString("CedulaID"),
                        FechaNacimiento = reader.GetDateTime("FechaNacimiento"),
                        Telefono = reader.IsDBNull("Telefono") ? null : reader.GetString("Telefono"),
                        Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                        Direccion = reader.IsDBNull("Direccion") ? null : reader.GetString("Direccion"),
                        TipoSangre = reader.GetString("TipoSangre")
                    };
                    list.Add(paciente);
                }

                return list;
            }
            catch (SqlException sqlEx)
            {
                throw new InvalidOperationException($"Error al listar pacientes por tipo de sangre '{tipoSangre}': {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error inesperado al listar pacientes por tipo de sangre: {ex.Message}", ex);
            }
        }

        public async Task<ListarPacienteDTO?> ObtenerPacientePorIdAsync(int id, CancellationToken ct)
        {
            if (id <= 0)
                throw new ArgumentException("ID debe ser mayor a 0", nameof(id));

            try
            {
                using var con = _connectionFactory.Create();
                using var cmd = new SqlCommand("dbo.sp_ListarPacientePorId", con) // Corregí el nombre del SP
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@PacienteID", SqlDbType.Int) { Value = id });

                await con.OpenAsync(ct);

                using var reader = await cmd.ExecuteReaderAsync(ct);

                if (await reader.ReadAsync(ct))
                {
                    var paciente = new ListarPacienteDTO
                    {
                        PacienteId = reader.GetInt32("PacienteID"),
                        Nombre = reader.GetString("Nombre"),
                        CedulaID = reader.GetString("CedulaID"),
                        FechaNacimiento = reader.GetDateTime("FechaNacimiento"),
                        Telefono = reader.IsDBNull("Telefono") ? null : reader.GetString("Telefono"),
                        Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                        Direccion = reader.IsDBNull("Direccion") ? null : reader.GetString("Direccion"),
                        TipoSangre = reader.GetString("TipoSangre")
                    };
                    return paciente;
                }

                return null;
            }
            catch (SqlException sqlEx)
            {
                throw new InvalidOperationException($"Error al obtener paciente con ID {id}: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error inesperado al obtener paciente: {ex.Message}", ex);
            }
        }

        public async Task<int> RegistrarPacienteAsync(PacienteDTO paciente, CancellationToken ct)
        {
            if (paciente == null)
                throw new ArgumentNullException(nameof(paciente));

            try
            {
                using var con = _connectionFactory.Create();
                using var cmd = new SqlCommand("dbo.sp_RegistrarPaciente", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 100) { Value = paciente.Nombre ?? throw new ArgumentException("Nombre no puede ser nulo", nameof(paciente.Nombre)) });
                cmd.Parameters.Add(new SqlParameter("@CedulaID", SqlDbType.VarChar, 20) { Value = paciente.CedulaID ?? throw new ArgumentException("CedulaID no puede ser nulo", nameof(paciente.CedulaID)) });
                cmd.Parameters.Add(new SqlParameter("@FechaNacimiento", SqlDbType.Date) { Value = paciente.FechaNacimiento });
                cmd.Parameters.Add(new SqlParameter("@Telefono", SqlDbType.VarChar, 20) { Value = (object?)paciente.Telefono ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = (object?)paciente.Email ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Direccion", SqlDbType.NVarChar, 200) { Value = (object?)paciente.Direccion ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@TipoSangreID", SqlDbType.Int) { Value = paciente.TipoSangreID });

                await con.OpenAsync(ct);

                using var rd = await cmd.ExecuteReaderAsync(ct);

                if (await rd.ReadAsync(ct))
                {
                    return Convert.ToInt32(rd["NuevoPacienteID"]);
                }

                throw new InvalidOperationException("No se pudo obtener el ID del paciente recién registrado.");
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 2627 || sqlEx.Number == 2601) 
                {
                    throw new InvalidOperationException($"Ya existe un paciente con la cédula {paciente.CedulaID}.", sqlEx);
                }
                else if (sqlEx.Number == 547) 
                {
                    throw new InvalidOperationException($"El TipoSangreID {paciente.TipoSangreID} no existe.", sqlEx);
                }
                else
                {
                    throw new InvalidOperationException($"Error al registrar paciente: {sqlEx.Message}", sqlEx);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error inesperado al registrar paciente: {ex.Message}", ex);
            }
        }
    }
}