using System.Data;
using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.Data.SqlClient;

namespace Backend.Data
{
    public class EnfermedadRepositorio : IEnfermedadRepositorio
    {
        private readonly IConnectionFactory _cf;

        public EnfermedadRepositorio(IConnectionFactory cf) => _cf = cf ?? throw new ArgumentNullException(nameof(cf));

        public async Task<int> CrearAsync(CrearEnfermedadDTO dto, CancellationToken ct)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.NombreEnfermedad))
                throw new ArgumentException("El nombre de la enfermedad es obligatorio.", nameof(dto.NombreEnfermedad));

            try
            {
                using var con = _cf.Create();
                using var cmd = new SqlCommand("dbo.sp_RegistrarEnfermedad", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@NombreEnfermedad", SqlDbType.NVarChar, 50)
                {
                    Value = dto.NombreEnfermedad.Trim()
                });

                cmd.Parameters.Add(new SqlParameter("@Descripcion", SqlDbType.NVarChar, 255)
                {
                    Value = (object?)dto.Descripcion?.Trim() ?? DBNull.Value
                });

                await con.OpenAsync(ct);
                var result = await cmd.ExecuteScalarAsync(ct);

                if (result == null || result == DBNull.Value)
                    throw new InvalidOperationException("El procedimiento almacenado no devolvió un ID de enfermedad.");

                if (!int.TryParse(result.ToString(), out int enfermedadId))
                    throw new InvalidOperationException("El ID devuelto por la base de datos no es un entero válido.");

                return enfermedadId;
            }
            catch (SqlException sqlEx)
            {
                throw new InvalidOperationException("Error al registrar la enfermedad en la base de datos.", sqlEx);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ocurrió un error inesperado al registrar la enfermedad.", ex);
            }
        }

        public async Task<IEnumerable<EnfermedadDTO>> ListarAsync(CancellationToken ct)
        {
            try
            {
                using var con = _cf.Create();
                using var cmd = new SqlCommand("dbo.sp_ListarEnfermedades", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                await con.OpenAsync(ct);
                using var rd = await cmd.ExecuteReaderAsync(ct);

                var list = new List<EnfermedadDTO>();
                while (await rd.ReadAsync(ct))
                {
                    list.Add(new EnfermedadDTO
                    {
                        EnfermedadID = rd.GetInt32("EnfermedadID"),
                        NombreEnfermedad = rd.GetString("NombreEnfermedad"),
                        Descripcion = rd.IsDBNull("Descripcion") ? null : rd.GetString("Descripcion")
                    });
                }

                return list;
            }
            catch (SqlException sqlEx)
            {
                throw new InvalidOperationException("Error al obtener la lista de enfermedades desde la base de datos.", sqlEx);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ocurrió un error inesperado al listar las enfermedades.", ex);
            }
        }

        public async Task<EnfermedadDTO?> ObtenerPorIdAsync(int id, CancellationToken ct)
        {
            if (id <= 0)
                return null;

            try
            {
                using var con = _cf.Create();
                using var cmd = new SqlCommand("dbo.sp_ListarEnfermedadPorID", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@EnfermedadID", SqlDbType.Int) { Value = id });

                await con.OpenAsync(ct);
                using var rd = await cmd.ExecuteReaderAsync(ct);

                if (!await rd.ReadAsync(ct))
                    return null;

                return new EnfermedadDTO
                {
                    EnfermedadID = rd.GetInt32("EnfermedadID"),
                    NombreEnfermedad = rd.GetString("NombreEnfermedad"),
                    Descripcion = rd.IsDBNull("Descripcion") ? null : rd.GetString("Descripcion")
                };
            }
            catch (SqlException sqlEx)
            {
                throw new InvalidOperationException($"Error al obtener la enfermedad con ID {id} desde la base de datos.", sqlEx);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ocurrió un error inesperado al obtener la enfermedad con ID {id}.", ex);
            }
        }

        public async Task<EnfermedadDTO?> ActualizarAsync(int id, ActualizarEnfermedadDTO dto, CancellationToken ct)
        {
            if (id <= 0)
                return null;

            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.NombreEnfermedad))
                throw new ArgumentException("El nombre de la enfermedad es obligatorio.", nameof(dto.NombreEnfermedad));

            try
            {
                using var con = _cf.Create();
                using var cmd = new SqlCommand("dbo.sp_ActualizarEnfermedad", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@EnfermedadID", SqlDbType.Int) { Value = id });
                cmd.Parameters.Add(new SqlParameter("@NombreEnfermedad", SqlDbType.NVarChar, 50)
                {
                    Value = dto.NombreEnfermedad.Trim()
                });
                cmd.Parameters.Add(new SqlParameter("@Descripcion", SqlDbType.NVarChar, -1)
                {
                    Value = (object?)dto.Descripcion?.Trim() ?? DBNull.Value
                });

                await con.OpenAsync(ct);
                using var rd = await cmd.ExecuteReaderAsync(ct);

                if (!await rd.ReadAsync(ct))
                    return null; // SP no devolvió datos (ej: ID no existe)

                return new EnfermedadDTO
                {
                    EnfermedadID = rd.GetInt32("EnfermedadID"),
                    NombreEnfermedad = rd.GetString("NombreEnfermedad"),
                    Descripcion = rd.IsDBNull("Descripcion") ? null : rd.GetString("Descripcion")
                };
            }
            catch (SqlException sqlEx)
            {
                throw new InvalidOperationException($"Error al actualizar la enfermedad con ID {id} en la base de datos.", sqlEx);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ocurrió un error inesperado al actualizar la enfermedad con ID {id}.", ex);
            }
        }

        public async Task<bool> EliminarAsync(int id, CancellationToken ct)
        {
            if (id <= 0)
                return false;

            try
            {
                using var con = _cf.Create();
                using var cmd = new SqlCommand("dbo.sp_EliminarEnfermedad", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@EnfermedadID", SqlDbType.Int) { Value = id });

                await con.OpenAsync(ct);
                var result = await cmd.ExecuteScalarAsync(ct);

                if (result == null || result == DBNull.Value)
                    return false;

                return Convert.ToInt32(result) > 0;
            }
            catch (SqlException sqlEx)
            {
                throw new InvalidOperationException($"Error al eliminar la enfermedad con ID {id} de la base de datos.", sqlEx);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ocurrió un error inesperado al eliminar la enfermedad con ID {id}.", ex);
            }
        }
    }
}