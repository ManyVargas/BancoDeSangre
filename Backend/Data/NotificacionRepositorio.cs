using System.Data;
using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.Data.SqlClient;

namespace Backend.Data
{
    public class NotificacionRepositorio : INotificacionRepositorio
    {
        private readonly IConnectionFactory _cf;

        public NotificacionRepositorio(IConnectionFactory cf) => _cf = cf ?? throw new ArgumentNullException(nameof(cf));

        public async Task<IEnumerable<ListarNotificacionDTO>> ListarPorDonanteAsync(int donanteId, CancellationToken ct)
        {
            if (donanteId <= 0)
                throw new ArgumentException("El ID del donante debe ser mayor a 0.", nameof(donanteId));

            try
            {
                using var con = _cf.Create();
                using var cmd = new SqlCommand("sp_ListarNotificacionesPorDonante", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@DonanteID", SqlDbType.Int) { Value = donanteId });

                await con.OpenAsync(ct);
                using var rd = await cmd.ExecuteReaderAsync(ct);

                var list = new List<ListarNotificacionDTO>();
                while (await rd.ReadAsync(ct))
                {
                    list.Add(new ListarNotificacionDTO
                    {
                        NotificacionID = rd.GetInt32("NotificacionID"),
                        NombreDonante = rd.GetString("NombreDonante"),
                        Mensaje = rd.GetString("Mensaje"),
                        FechaEnvio = rd.GetDateTime("FechaEnvio"),
                        Leida = rd.GetBoolean("Leido")
                    });
                }
                return list;
            }
            catch (SqlException sqlEx)
            {
                throw new InvalidOperationException("Error al obtener las notificaciones del donante desde la base de datos.", sqlEx);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ocurrió un error inesperado al listar notificaciones para el donante con ID {donanteId}.", ex);
            }
        }

        public async Task<ListarNotificacionDTO?> ListarPorIdAsync(int id, CancellationToken ct)
        {
            if (id <= 0)
                return null;

            try
            {
                using var con = _cf.Create();
                using var cmd = new SqlCommand("dbo.sp_ListarNotificacionPorID", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add(new SqlParameter("@NotificacionID", SqlDbType.Int) { Value = id });

                await con.OpenAsync(ct);
                using var rd = await cmd.ExecuteReaderAsync(ct);
                if (!await rd.ReadAsync(ct)) return null;

                return new ListarNotificacionDTO
                {
                    NotificacionID = rd.GetInt32("NotificacionID"),
                    NombreDonante = rd.GetString("NombreDonante"),
                    Mensaje = rd.GetString("Mensaje"),
                    FechaEnvio = rd.GetDateTime("FechaEnvio"),
                    Leida = rd.GetBoolean("Leido")
                };
            }
            catch (SqlException sqlEx)
            {
                throw new InvalidOperationException($"Error al obtener la notificación con ID {id} desde la base de datos.", sqlEx);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ocurrió un error inesperado al obtener la notificación con ID {id}.", ex);
            }
        }

        public async Task<int> CrearAsync(CrearNotificacionDTO dto, CancellationToken ct)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.DonanteID <= 0)
                throw new ArgumentException("El ID del donante es inválido.", nameof(dto.DonanteID));

            if (dto.solicitudID<= 0)
                throw new ArgumentException("El ID de la solicitud es inválido.", nameof(dto.solicitudID));

            if (string.IsNullOrWhiteSpace(dto.Mensaje))
                throw new ArgumentException("El mensaje no puede estar vacío.", nameof(dto.Mensaje));

            try
            {
                using var con = _cf.Create();
                using var cmd = new SqlCommand("dbo.sp_CrearNotificacion", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@DonanteID", SqlDbType.Int) { Value = dto.DonanteID });
                cmd.Parameters.Add(new SqlParameter("@SolicitudID", SqlDbType.Int) { Value = dto.solicitudID });
                cmd.Parameters.Add(new SqlParameter("@Mensaje", SqlDbType.NVarChar, -1) { Value = dto.Mensaje.Trim() });

                await con.OpenAsync(ct);
                var result = await cmd.ExecuteScalarAsync(ct);

                if (result == null || result == DBNull.Value)
                    throw new InvalidOperationException("El procedimiento almacenado no devolvió un ID de notificación.");

                return Convert.ToInt32(result);
            }
            catch (SqlException sqlEx)
            {
                throw new InvalidOperationException("Error al crear la notificación en la base de datos.", sqlEx);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ocurrió un error inesperado al crear la notificación.", ex);
            }
        }

        public async Task<ListarNotificacionDTO?> ActualizarAsync(int id, ActualizarNotificacionDTO dto, CancellationToken ct)
        {
            if (id <= 0)
                return null;

            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.Mensaje))
                throw new ArgumentException("El mensaje no puede estar vacío.", nameof(dto.Mensaje));

            try
            {
                using var con = _cf.Create();
                using var cmd = new SqlCommand("dbo.sp_ActualizarNotificacion", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@NotificacionID", SqlDbType.Int) { Value = id });
                cmd.Parameters.Add(new SqlParameter("@Mensaje", SqlDbType.NVarChar, 255) { Value = dto.Mensaje.Trim() });
                cmd.Parameters.Add(new SqlParameter("@Leido", SqlDbType.Bit) { Value = dto.Leido });

                await con.OpenAsync(ct);
                using var rd = await cmd.ExecuteReaderAsync(ct);
                if (!await rd.ReadAsync(ct)) return null;

                return new ListarNotificacionDTO
                {
                    NotificacionID = rd.GetInt32("NotificacionID"),
                    NombreDonante = rd.GetString("NombreDonante"),
                    SolicitudID = rd.GetInt32("SolicitudID"),
                    Mensaje = rd.GetString("Mensaje"),
                    FechaEnvio = rd.GetDateTime("FechaEnvio"),
                    Leida = rd.GetBoolean("Leido")
                };
            }
            catch (SqlException sqlEx)
            {
                throw new InvalidOperationException($"Error al actualizar la notificación con ID {id} en la base de datos.", sqlEx);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ocurrió un error inesperado al actualizar la notificación con ID {id}.", ex);
            }
        }

        public async Task<bool> MarcarLeidaAsync(int id, bool leido, CancellationToken ct)
        {
            if (id <= 0)
                return false;

            try
            {
                using var con = _cf.Create();
                using var cmd = new SqlCommand("dbo.sp_MarcarNotificacionLeida", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@NotificacionID", SqlDbType.Int) { Value = id });
                cmd.Parameters.Add(new SqlParameter("@Leido", SqlDbType.Bit) { Value = leido });

                await con.OpenAsync(ct);
                var result = await cmd.ExecuteScalarAsync(ct);

                if (result == null || result == DBNull.Value)
                    return false;

                return Convert.ToInt32(result) > 0;
            }
            catch (SqlException sqlEx)
            {
                throw new InvalidOperationException($"Error al marcar como {(leido ? "leída" : "no leída")} la notificación con ID {id} en la base de datos.", sqlEx);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ocurrió un error inesperado al marcar como {(leido ? "leída" : "no leída")} la notificación con ID {id}.", ex);
            }
        }

        public async Task<bool> EliminarAsync(int id, CancellationToken ct)
        {
            if (id <= 0)
                return false;

            try
            {
                using var con = _cf.Create();
                using var cmd = new SqlCommand("dbo.sp_EliminarNotificacion", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@NotificacionID", SqlDbType.Int) { Value = id });

                await con.OpenAsync(ct);
                var result = await cmd.ExecuteScalarAsync(ct);

                if (result == null || result == DBNull.Value)
                    return false;

                return Convert.ToInt32(result) > 0;
            }
            catch (SqlException sqlEx)
            {
                throw new InvalidOperationException($"Error al eliminar la notificación con ID {id} de la base de datos.", sqlEx);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ocurrió un error inesperado al eliminar la notificación con ID {id}.", ex);
            }
        }
    }
}