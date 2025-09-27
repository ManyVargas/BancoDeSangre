using System.Data;
using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.Data.SqlClient;

namespace Backend.Data
{
    public class NotificacionRepositorio : INotificacionRepositorio
    {
        private readonly IConnectionFactory _cf;
        public NotificacionRepositorio(IConnectionFactory cf) => _cf = cf;

        public async Task<IEnumerable<NotificacionDTO>> ListarAsync(CancellationToken ct)
        {
            using var con = _cf.Create();
            using var cmd = new SqlCommand(@"
                SELECT NotificacionID, UsuarioID, Mensaje, FechaCreacion, Leido
                FROM dbo.Notificaciones
                ORDER BY FechaCreacion DESC;", con);

            await con.OpenAsync(ct);
            using var rd = await cmd.ExecuteReaderAsync(ct);

            var list = new List<NotificacionDTO>();
            while (await rd.ReadAsync(ct))
            {
                list.Add(new NotificacionDTO
                {
                    NotificacionID = rd.GetInt32(rd.GetOrdinal("NotificacionID")),
                    UsuarioID = rd.GetInt32(rd.GetOrdinal("UsuarioID")),
                    Mensaje = rd.GetString(rd.GetOrdinal("Mensaje")),
                    FechaCreacion = rd.GetDateTime(rd.GetOrdinal("FechaCreacion")),
                    Leido = rd.GetBoolean(rd.GetOrdinal("Leido"))
                });
            }
            return list;
        }

        public async Task<IEnumerable<NotificacionDTO>> ListarPorUsuarioAsync(int usuarioId, CancellationToken ct)
        {
            using var con = _cf.Create();
            using var cmd = new SqlCommand(@"
                SELECT NotificacionID, UsuarioID, Mensaje, FechaCreacion, Leido
                FROM dbo.Notificaciones
                WHERE UsuarioID = @u
                ORDER BY FechaCreacion DESC;", con);
            cmd.Parameters.Add(new SqlParameter("@u", SqlDbType.Int) { Value = usuarioId });

            await con.OpenAsync(ct);
            using var rd = await cmd.ExecuteReaderAsync(ct);

            var list = new List<NotificacionDTO>();
            while (await rd.ReadAsync(ct))
            {
                list.Add(new NotificacionDTO
                {
                    NotificacionID = rd.GetInt32(rd.GetOrdinal("NotificacionID")),
                    UsuarioID = rd.GetInt32(rd.GetOrdinal("UsuarioID")),
                    Mensaje = rd.GetString(rd.GetOrdinal("Mensaje")),
                    FechaCreacion = rd.GetDateTime(rd.GetOrdinal("FechaCreacion")),
                    Leido = rd.GetBoolean(rd.GetOrdinal("Leido"))
                });
            }
            return list;
        }

        public async Task<NotificacionDTO?> ObtenerPorIdAsync(int id, CancellationToken ct)
        {
            using var con = _cf.Create();
            using var cmd = new SqlCommand(@"
                SELECT NotificacionID, UsuarioID, Mensaje, FechaCreacion, Leido
                FROM dbo.Notificaciones
                WHERE NotificacionID = @id;", con);
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });

            await con.OpenAsync(ct);
            using var rd = await cmd.ExecuteReaderAsync(ct);
            if (!await rd.ReadAsync(ct)) return null;

            return new NotificacionDTO
            {
                NotificacionID = rd.GetInt32(rd.GetOrdinal("NotificacionID")),
                UsuarioID = rd.GetInt32(rd.GetOrdinal("UsuarioID")),
                Mensaje = rd.GetString(rd.GetOrdinal("Mensaje")),
                FechaCreacion = rd.GetDateTime(rd.GetOrdinal("FechaCreacion")),
                Leido = rd.GetBoolean(rd.GetOrdinal("Leido"))
            };
        }

        public async Task<int> CrearAsync(CrearNotificacionDTO dto, CancellationToken ct)
        {
            using var con = _cf.Create();
            using var cmd = new SqlCommand(@"
                INSERT INTO dbo.Notificaciones (UsuarioID, Mensaje)
                OUTPUT INSERTED.NotificacionID
                VALUES (@u, @m);", con);

            cmd.Parameters.Add(new SqlParameter("@u", SqlDbType.Int) { Value = dto.UsuarioID });
            cmd.Parameters.Add(new SqlParameter("@m", SqlDbType.NVarChar, -1) { Value = dto.Mensaje });

            await con.OpenAsync(ct);
            var id = (int)await cmd.ExecuteScalarAsync(ct);
            return id;
        }

        public async Task<NotificacionDTO?> ActualizarAsync(int id, ActualizarNotificacionDTO dto, CancellationToken ct)
        {
            using var con = _cf.Create();
            using var cmd = new SqlCommand(@"
                UPDATE dbo.Notificaciones
                SET Mensaje = @m, Leido = @l
                WHERE NotificacionID = @id;

                SELECT NotificacionID, UsuarioID, Mensaje, FechaCreacion, Leido
                FROM dbo.Notificaciones
                WHERE NotificacionID = @id;", con);

            cmd.Parameters.Add(new SqlParameter("@m", SqlDbType.NVarChar, -1) { Value = dto.Mensaje });
            cmd.Parameters.Add(new SqlParameter("@l", SqlDbType.Bit) { Value = dto.Leido });
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });

            await con.OpenAsync(ct);
            using var rd = await cmd.ExecuteReaderAsync(ct);
            if (!await rd.ReadAsync(ct)) return null;

            return new NotificacionDTO
            {
                NotificacionID = rd.GetInt32(rd.GetOrdinal("NotificacionID")),
                UsuarioID = rd.GetInt32(rd.GetOrdinal("UsuarioID")),
                Mensaje = rd.GetString(rd.GetOrdinal("Mensaje")),
                FechaCreacion = rd.GetDateTime(rd.GetOrdinal("FechaCreacion")),
                Leido = rd.GetBoolean(rd.GetOrdinal("Leido"))
            };
        }

        public async Task<bool> MarcarLeidaAsync(int id, bool leido, CancellationToken ct)
        {
            using var con = _cf.Create();
            using var cmd = new SqlCommand(@"
                UPDATE dbo.Notificaciones SET Leido = @l WHERE NotificacionID = @id;
                SELECT @@ROWCOUNT;", con);

            cmd.Parameters.Add(new SqlParameter("@l", SqlDbType.Bit) { Value = leido });
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });

            await con.OpenAsync(ct);
            var rows = (int)await cmd.ExecuteScalarAsync(ct);
            return rows > 0;
        }

        public async Task<bool> EliminarAsync(int id, CancellationToken ct)
        {
            using var con = _cf.Create();
            using var cmd = new SqlCommand("DELETE dbo.Notificaciones WHERE NotificacionID = @id; SELECT @@ROWCOUNT;", con);
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });

            await con.OpenAsync(ct);
            var rows = (int)await cmd.ExecuteScalarAsync(ct);
            return rows > 0;
        }
    }
}
