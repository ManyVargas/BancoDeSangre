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
    public class DonanteRepositorio : IDonanteRepositorio
    {
        private readonly IConnectionFactory _connectionFactory;

        public DonanteRepositorio(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        //CREAR DONANTE
        public async Task<int> CrearAsync(RegistrarDonanteDTO dto, CancellationToken ct)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            try
            {
                using var con = _connectionFactory.Create();
                using var cmd = new SqlCommand("dbo.sp_RegistrarDonante", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 100) { Value = dto.Nombre ?? throw new ArgumentException("Nombre no puede ser nulo", nameof(dto.Nombre)) });
                cmd.Parameters.Add(new SqlParameter("@CedulaID", SqlDbType.VarChar, 20) { Value = dto.CedulaID ?? throw new ArgumentException("CedulaID no puede ser nulo", nameof(dto.CedulaID)) });
                cmd.Parameters.Add(new SqlParameter("@FechaNacimiento", SqlDbType.Date) { Value = dto.FechaNacimiento });
                cmd.Parameters.Add(new SqlParameter("@Telefono", SqlDbType.VarChar, 20) { Value = (object?)dto.Telefono ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = dto.Email ?? throw new ArgumentException("Email no puede ser nulo", nameof(dto.Email)) });
                cmd.Parameters.Add(new SqlParameter("@TipoSangreID", SqlDbType.Int) { Value = dto.TipoSangreID });
                cmd.Parameters.Add(new SqlParameter("@UltimaDonacion", SqlDbType.Date) { Value = (object?)dto.UltimaDonacion ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Latitud", SqlDbType.Decimal) { Value = (object?)dto.Latitud ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Longitud", SqlDbType.Decimal) { Value = (object?)dto.Longitud ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Disponibilidad", SqlDbType.Bit) { Value = dto.Disponibilidad });

                await con.OpenAsync(ct);

                using var rd = await cmd.ExecuteReaderAsync(ct);

                if (!rd.HasRows)
                    throw new InvalidOperationException("No se pudo registrar el donante.");

                await rd.ReadAsync(ct);
                return rd.GetInt32("DonanteID");
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 2627 || sqlEx.Number == 2601) // Error de clave duplicada
                {
                    throw new InvalidOperationException($"Ya existe un donante con la cédula {dto.CedulaID}.", sqlEx);
                }
                else if (sqlEx.Number == 547) // Error de clave foránea
                {
                    throw new InvalidOperationException($"El TipoSangreID {dto.TipoSangreID} no existe.", sqlEx);
                }
                else
                {
                    throw new InvalidOperationException($"Error al registrar donante: {sqlEx.Message}", sqlEx);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error inesperado al registrar donante: {ex.Message}", ex);
            }
        }

        public async Task<ListarDonantesDTO?> ActualizarAsync(int id, ActualizarDonanteDTO dto, CancellationToken ct)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (id <= 0)
                throw new ArgumentException("ID debe ser mayor a 0", nameof(id));

            try
            {
                using var con = _connectionFactory.Create();
                using var cmd = new SqlCommand("dbo.sp_ActualizarDonante", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@DonanteID", SqlDbType.Int) { Value = id });
                cmd.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 100) { Value = dto.Nombre ?? throw new ArgumentException("Nombre no puede ser nulo", nameof(dto.Nombre)) });
                cmd.Parameters.Add(new SqlParameter("@CedulaID", SqlDbType.VarChar, 20) { Value = dto.CedulaID ?? throw new ArgumentException("CedulaID no puede ser nulo", nameof(dto.CedulaID)) });
                cmd.Parameters.Add(new SqlParameter("@FechaNacimiento", SqlDbType.Date) { Value = dto.FechaNacimiento });
                cmd.Parameters.Add(new SqlParameter("@Telefono", SqlDbType.VarChar, 20) { Value = (object?)dto.Telefono ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = dto.Email ?? throw new ArgumentException("Email no puede ser nulo", nameof(dto.Email)) });
                cmd.Parameters.Add(new SqlParameter("@TipoSangreID", SqlDbType.Int) { Value = dto.TipoSangreID });
                cmd.Parameters.Add(new SqlParameter("@UltimaDonacion", SqlDbType.Date) { Value = (object?)dto.UltimaDonacion ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Latitud", SqlDbType.Decimal) { Value = (object?)dto.Latitud ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Longitud", SqlDbType.Decimal) { Value = (object?)dto.Longitud ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Disponibilidad", SqlDbType.Bit) { Value = dto.Disponibilidad });

                await con.OpenAsync(ct);

                using var reader = await cmd.ExecuteReaderAsync(ct);

                if (!reader.HasRows)
                    return null;

                await reader.ReadAsync(ct);

                return new ListarDonantesDTO
                {
                    DonanteID = reader.GetInt32("DonanteID"),
                    Nombre = reader.GetString("Nombre"),
                    CedulaID = reader.GetString("CedulaID"),
                    FechaNacimiento = reader.GetDateTime("FechaNacimiento"),
                    Telefono = reader.IsDBNull("Telefono") ? null : reader.GetString("Telefono"),
                    Email = reader.GetString("Email"),
                    TipoSangre = reader.GetString("TipoSangre"),
                    UltimaDonacion = reader.IsDBNull("UltimaDonacion") ? (DateTime?)null : reader.GetDateTime("UltimaDonacion"),
                    Latitud = reader.IsDBNull("Latitud") ? (decimal?)null : reader.GetDecimal("Latitud"),
                    Longitud = reader.IsDBNull("Longitud") ? (decimal?)null : reader.GetDecimal("Longitud"),
                    Disponibilidad = reader.GetBoolean("Disponibilidad")
                };
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 2627 || sqlEx.Number == 2601) // Error de clave duplicada
                {
                    throw new InvalidOperationException($"Ya existe un donante con la cédula {dto.CedulaID}.", sqlEx);
                }
                else if (sqlEx.Number == 547) // Error de clave foránea
                {
                    throw new InvalidOperationException($"El TipoSangreID {dto.TipoSangreID} no existe.", sqlEx);
                }
                else
                {
                    throw new InvalidOperationException($"Error al actualizar donante: {sqlEx.Message}", sqlEx);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error inesperado al actualizar donante: {ex.Message}", ex);
            }
        }

        public async Task<bool> EliminarAsync(int id, CancellationToken ct)
        {
            if (id <= 0)
                throw new ArgumentException("ID debe ser mayor a 0", nameof(id));

            try
            {
                using var con = _connectionFactory.Create();
                using var cmd = new SqlCommand("dbo.sp_EliminarDonante", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@DonanteID", SqlDbType.Int) { Value = id }); // Cambié el nombre del parámetro

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
                throw new InvalidOperationException($"Error al eliminar donante: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error inesperado al eliminar donante: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<ListarDonantesDTO>> ListarAsync(CancellationToken ct)
        {
            try
            {
                using var con = _connectionFactory.Create();
                using var cmd = new SqlCommand("dbo.sp_ListarDonantes", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                await con.OpenAsync(ct);

                using var reader = await cmd.ExecuteReaderAsync(ct);

                var list = new List<ListarDonantesDTO>();
                while (await reader.ReadAsync(ct))
                {
                    list.Add(new ListarDonantesDTO
                    {
                        DonanteID = reader.GetInt32("DonanteID"),
                        Nombre = reader.GetString("Nombre"),
                        CedulaID = reader.GetString("CedulaID"),
                        FechaNacimiento = reader.GetDateTime("FechaNacimiento"),
                        Telefono = reader.IsDBNull("Telefono") ? null : reader.GetString("Telefono"),
                        Email = reader.GetString("Email"),
                        TipoSangre = reader.GetString("TipoSangre"),
                        UltimaDonacion = reader.IsDBNull("UltimaDonacion") ? (DateTime?)null : reader.GetDateTime("UltimaDonacion"),
                        Latitud = reader.IsDBNull("Latitud") ? (decimal?)null : reader.GetDecimal("Latitud"),
                        Longitud = reader.IsDBNull("Longitud") ? (decimal?)null : reader.GetDecimal("Longitud"),
                        Disponibilidad = reader.GetBoolean("Disponibilidad")
                    });
                }
                return list;
            }
            catch (SqlException sqlEx)
            {
                throw new InvalidOperationException($"Error al listar donantes: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error inesperado al listar donantes: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<ListarDonantesDTO>> ListarPorTipoSangreAsync(string tipoSangre, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(tipoSangre))
                throw new ArgumentException("Tipo de sangre no puede ser nulo o vacío", nameof(tipoSangre));

            try
            {
                using var con = _connectionFactory.Create();
                using var cmd = new SqlCommand("dbo.sp_ListarDonantesPorTipoSangre", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@TipoSangre", SqlDbType.NVarChar, 50) { Value = tipoSangre }); // Cambié el nombre del parámetro

                await con.OpenAsync(ct);

                using var reader = await cmd.ExecuteReaderAsync(ct);

                var list = new List<ListarDonantesDTO>();
                while (await reader.ReadAsync(ct))
                {
                    list.Add(new ListarDonantesDTO
                    {
                        DonanteID = reader.GetInt32("DonanteID"),
                        Nombre = reader.GetString("Nombre"),
                        CedulaID = reader.GetString("CedulaID"),
                        FechaNacimiento = reader.GetDateTime("FechaNacimiento"),
                        Telefono = reader.IsDBNull("Telefono") ? null : reader.GetString("Telefono"),
                        Email = reader.GetString("Email"),
                        TipoSangre = reader.GetString("TipoSangre"),
                        UltimaDonacion = reader.IsDBNull("UltimaDonacion") ? (DateTime?)null : reader.GetDateTime("UltimaDonacion"),
                        Latitud = reader.IsDBNull("Latitud") ? (decimal?)null : reader.GetDecimal("Latitud"),
                        Longitud = reader.IsDBNull("Longitud") ? (decimal?)null : reader.GetDecimal("Longitud"),
                        Disponibilidad = reader.GetBoolean("Disponibilidad")
                    });
                }

               
                return list;
            }
            catch (SqlException sqlEx)
            {
                throw new InvalidOperationException($"Error al listar donantes por tipo de sangre '{tipoSangre}': {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error inesperado al listar donantes por tipo de sangre: {ex.Message}", ex);
            }
        }

        public async Task<ListarDonantesDTO?> ObtenerPorIdAsync(int id, CancellationToken ct)
        {
            if (id <= 0)
                throw new ArgumentException("ID debe ser mayor a 0", nameof(id));

            try
            {
                using var con = _connectionFactory.Create();
                using var cmd = new SqlCommand("dbo.sp_ListarDonanteID", con) // Corregí el nombre del SP
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@DonanteID", SqlDbType.Int) { Value = id });

                await con.OpenAsync(ct);

                using var reader = await cmd.ExecuteReaderAsync(ct);

                if (await reader.ReadAsync(ct))
                {
                    return new ListarDonantesDTO
                    {
                        DonanteID = reader.GetInt32("DonanteID"),
                        Nombre = reader.GetString("Nombre"),
                        CedulaID = reader.GetString("CedulaID"),
                        FechaNacimiento = reader.GetDateTime("FechaNacimiento"),
                        Telefono = reader.IsDBNull("Telefono") ? null : reader.GetString("Telefono"),
                        Email = reader.GetString("Email"),
                        TipoSangre = reader.GetString("TipoSangre"),
                        UltimaDonacion = reader.IsDBNull("UltimaDonacion") ? (DateTime?)null : reader.GetDateTime("UltimaDonacion"),
                        Latitud = reader.IsDBNull("Latitud") ? (decimal?)null : reader.GetDecimal("Latitud"),
                        Longitud = reader.IsDBNull("Longitud") ? (decimal?)null : reader.GetDecimal("Longitud"),
                        Disponibilidad = reader.GetBoolean("Disponibilidad")
                    };
                }
                return null;
            }
            catch (SqlException sqlEx)
            {
                throw new InvalidOperationException($"Error al obtener donante con ID {id}: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error inesperado al obtener donante: {ex.Message}", ex);
            }
        }
    }
}