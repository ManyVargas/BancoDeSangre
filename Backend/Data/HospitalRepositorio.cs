using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection.PortableExecutable;
using System.Threading;
using System.Threading.Tasks;
using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.Data.SqlClient;

namespace Backend.Data
{
    public class HospitalRepositorio : IHospitalRepositorio
    {
        private readonly IConnectionFactory _connectionFactory;

        public HospitalRepositorio(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<HospitalDTO> ActualizarHospital(int hospitalId, RegistrarHospitalDTO dto, CancellationToken ct)
        {
            try
            {
                using var con = _connectionFactory.Create();
                using var cmd = new SqlCommand("sp_ActualizarHospital", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@HospitalID", SqlDbType.Int) { Value = hospitalId });
                cmd.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 100) { Value = dto?.Nombre ?? (object)DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Direccion", SqlDbType.NVarChar, 100) { Value = dto?.Direccion ?? (object)DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Telefono", SqlDbType.NVarChar, 15) { Value = dto?.Telefono ?? (object)DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Latitud", SqlDbType.Decimal) { Value = dto?.Latitud ?? (object)DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Longitud", SqlDbType.Decimal) { Value = dto?.Longitud ?? (object)DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@ContactoPrincipal", SqlDbType.NVarChar, 15) { Value = dto?.ContactoPrincipal ?? (object)DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@EmailContacto", SqlDbType.NVarChar, 50) { Value = dto?.EmailContacto ?? (object)DBNull.Value });

                await con.OpenAsync(ct);
                using var rd = await cmd.ExecuteReaderAsync(ct);

                if (!rd.HasRows)
                    throw new KeyNotFoundException($"No se encontró el hospital con ID {hospitalId}");

                await rd.ReadAsync(ct);

                return new HospitalDTO
                {
                    HospitalID = rd.GetInt32(rd.GetOrdinal("HospitalID")),
                    Nombre = rd.GetString(rd.GetOrdinal("Nombre")),
                    Direccion = rd.GetString(rd.GetOrdinal("Direccion")),
                    Telefono = rd.GetString(rd.GetOrdinal("Telefono")),
                    Latitud = rd.IsDBNull("Latitud") ? (decimal?)null : rd.GetDecimal("Latitud"),
                    Longitud = rd.IsDBNull("Longitud") ? (decimal?)null : rd.GetDecimal("Longitud"),
                    ContactoPrincipal = rd.GetString(rd.GetOrdinal("ContactoPrincipal")),
                    EmailContacto = rd.GetString(rd.GetOrdinal("EmailContacto"))
                };
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Error en la base de datos al actualizar hospital", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error inesperado al actualizar hospital", ex);
            }
        }

        public async Task<bool> EliminarHospital(int hospitalId, CancellationToken ct)
        {
            try
            {
                using var con = _connectionFactory.Create();
                using var cmd = new SqlCommand("sp_EliminarHospital", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@HospitalID", SqlDbType.Int) { Value = hospitalId });

                await con.OpenAsync(ct);
                using var reader = await cmd.ExecuteReaderAsync(ct);

                if (await reader.ReadAsync(ct))
                {
                    var filasAfectadas = reader.GetInt32(reader.GetOrdinal("FilasAfectadas"));
                    return filasAfectadas > 0;
                }

                return false;
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Error en la base de datos al eliminar hospital", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error inesperado al eliminar hospital", ex);
            }
        }

        public async Task<HospitalDTO> ObtenerHospitalPorNombre(string nombreHospital, CancellationToken ct)
        {
            try
            {
                using var con = _connectionFactory.Create();
                using var cmd = new SqlCommand("sp_ListarHospitalesPorNombre", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 50) { Value = nombreHospital });

                await con.OpenAsync(ct);
                using var rd = await cmd.ExecuteReaderAsync(ct);

                if (await rd.ReadAsync(ct))
                {
                    return new HospitalDTO
                    {
                        HospitalID = rd.GetInt32(rd.GetOrdinal("HospitalID")),
                        Nombre = rd.GetString(rd.GetOrdinal("Nombre")),
                        Direccion = rd.GetString(rd.GetOrdinal("Direccion")),
                        Telefono = rd.GetString(rd.GetOrdinal("Telefono")),
                        Latitud = rd.IsDBNull("Latitud") ? (decimal?)null : rd.GetDecimal("Latitud"),
                        Longitud = rd.IsDBNull("Longitud") ? (decimal?)null : rd.GetDecimal("Longitud"),
                        ContactoPrincipal = rd.GetString(rd.GetOrdinal("ContactoPrincipal")),
                        EmailContacto = rd.GetString(rd.GetOrdinal("EmailContacto"))
                    };
                }

                return null;
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Error en la base de datos al obtener hospital por nombre", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error inesperado al obtener hospital por nombre", ex);
            }
        }

        public async Task<IEnumerable<HospitalDTO>> ObtenerTodosLosHospitales(CancellationToken ct)
        {
            try
            {
                using var con = _connectionFactory.Create();
                using var cmd = new SqlCommand("sp_ListarHospitales", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                await con.OpenAsync(ct);
                using var rd = await cmd.ExecuteReaderAsync(ct);

                var list = new List<HospitalDTO>();

                while (await rd.ReadAsync(ct))
                {
                    list.Add(new HospitalDTO
                    {
                        HospitalID = rd.GetInt32(rd.GetOrdinal("HospitalID")),
                        Nombre = rd.GetString(rd.GetOrdinal("Nombre")),
                        Direccion = rd.GetString(rd.GetOrdinal("Direccion")),
                        Telefono = rd.GetString(rd.GetOrdinal("Telefono")),
                        Latitud = rd.IsDBNull("Latitud") ? (decimal?)null : rd.GetDecimal("Latitud"),
                        Longitud = rd.IsDBNull("Longitud") ? (decimal?)null : rd.GetDecimal("Longitud"),
                        ContactoPrincipal = rd.GetString(rd.GetOrdinal("ContactoPrincipal")),
                        EmailContacto = rd.GetString(rd.GetOrdinal("EmailContacto"))
                    });
                }

                return list;
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Error en la base de datos al obtener todos los hospitales", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error inesperado al obtener todos los hospitales", ex);
            }
        }

        public async Task<int> RegistrarHospital(RegistrarHospitalDTO dto, CancellationToken ct)
        {
            try
            {
                using var con = _connectionFactory.Create();
                using var cmd = new SqlCommand("sp_RegistrarHospital", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 100) { Value = dto?.Nombre ?? (object)DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Direccion", SqlDbType.NVarChar, 150) { Value = dto?.Direccion ?? (object)DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Telefono", SqlDbType.NVarChar, 15) { Value = dto?.Telefono ?? (object)DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Latitud", SqlDbType.Decimal) { Value = dto?.Latitud ?? (object)DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Longitud", SqlDbType.Decimal) { Value = dto?.Longitud ?? (object)DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@ContactoPrincipal", SqlDbType.NVarChar, 15) { Value = dto?.ContactoPrincipal ?? (object)DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@EmailContacto", SqlDbType.NVarChar, 50) { Value = dto?.EmailContacto ?? (object)DBNull.Value });

                await con.OpenAsync(ct);
                using var rd = await cmd.ExecuteReaderAsync(ct);

                if (!rd.HasRows)
                    throw new InvalidOperationException("No se pudo registrar el hospital");

                await rd.ReadAsync(ct);

                return rd.GetInt32(rd.GetOrdinal("HospitalID"));
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Error en la base de datos al registrar hospital", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error inesperado al registrar hospital", ex);
            }
        }
    }
}
