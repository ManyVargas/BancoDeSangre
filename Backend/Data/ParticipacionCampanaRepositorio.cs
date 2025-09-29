using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.Data.SqlClient;

namespace Backend.Data
{
    public class ParticipacionCampanaRepositorio : IParticipacionCampanaRepositorio
    {
        private readonly IConnectionFactory _cf;
        public ParticipacionCampanaRepositorio(IConnectionFactory cf)
            => _cf = cf ?? throw new ArgumentNullException(nameof(cf));

        // ===== Helpers =====
        private static ParticipacionCampanaDetalleDTO Map(SqlDataReader rd)
        {
            int oParticipacionID = rd.GetOrdinal("ParticipacionID");
            int oCampanaID = rd.GetOrdinal("CampanaID");
            int oNombreCampana = rd.GetOrdinal("NombreCampana");
            int oDonanteID = rd.GetOrdinal("DonanteID");
            int oNombreDonante = rd.GetOrdinal("NombreDonante");
            int oFechaDonacion = rd.GetOrdinal("FechaDonacion");
            int oCantidadUnidades = rd.GetOrdinal("CantidadUnidades");

            return new ParticipacionCampanaDetalleDTO
            {
                ParticipacionID = rd.GetInt32(oParticipacionID),
                CampanaID = rd.GetInt32(oCampanaID),
                NombreCampana = rd.IsDBNull(oNombreCampana) ? string.Empty : rd.GetString(oNombreCampana),
                DonanteID = rd.GetInt32(oDonanteID),
                NombreDonante = rd.IsDBNull(oNombreDonante) ? string.Empty : rd.GetString(oNombreDonante),
                FechaDonacion = rd.GetDateTime(oFechaDonacion),
                CantidadUnidades = rd.GetInt32(oCantidadUnidades)
            };
        }

        private const string SELECT_BASE = @"
            SELECT  p.ParticipacionID,
                    p.CampanaID,
                    c.Nombre  AS NombreCampana,
                    p.DonanteID,
                    d.Nombre  AS NombreDonante,
                    p.FechaDonacion,
                    p.CantidadUnidades
            FROM dbo.ParticipacionCampana p
            INNER JOIN dbo.CampanasDonacion c ON p.CampanaID = c.CampanaID
            INNER JOIN dbo.Donantes d         ON p.DonanteID = d.DonanteID";

        // ===== Implementación =====

        public async Task<int> CrearAsync(CrearParticipacionCampanaDTO dto, CancellationToken ct)
        {
            using var con = _cf.Create();
            using var cmd = new SqlCommand(@"
                INSERT INTO dbo.ParticipacionCampana (CampanaID, DonanteID, FechaDonacion, CantidadUnidades)
                OUTPUT INSERTED.ParticipacionID
                VALUES (@CampanaID, @DonanteID, @FechaDonacion, @CantidadUnidades);", con);

            cmd.Parameters.Add(new SqlParameter("@CampanaID", SqlDbType.Int) { Value = dto.CampanaID });
            cmd.Parameters.Add(new SqlParameter("@DonanteID", SqlDbType.Int) { Value = dto.DonanteID });
            cmd.Parameters.Add(new SqlParameter("@FechaDonacion", SqlDbType.DateTime2) { Value = dto.FechaDonacion });
            cmd.Parameters.Add(new SqlParameter("@CantidadUnidades", SqlDbType.Int) { Value = dto.CantidadUnidades });

            await con.OpenAsync(ct);
            var id = (int)await cmd.ExecuteScalarAsync(ct);
            return id;
        }

        public async Task<ParticipacionCampanaDetalleDTO?> ObtenerPorIdAsync(int id, CancellationToken ct)
        {
            using var con = _cf.Create();
            using var cmd = new SqlCommand(SELECT_BASE + " WHERE p.ParticipacionID = @id;", con);
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });

            await con.OpenAsync(ct);
            using var rd = await cmd.ExecuteReaderAsync(ct);
            if (!await rd.ReadAsync(ct)) return null;
            return Map(rd);
        }

        public async Task<bool> EliminarAsync(int id, CancellationToken ct)
        {
            using var con = _cf.Create();
            using var cmd = new SqlCommand(@"
                DELETE FROM dbo.ParticipacionCampana WHERE ParticipacionID = @id;
                SELECT @@ROWCOUNT;", con);

            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });

            await con.OpenAsync(ct);
            var rows = (int)await cmd.ExecuteScalarAsync(ct);
            return rows > 0;
        }

        public async Task<ParticipacionCampanaDetalleDTO?> ActualizarCantidadUnidadesAsync(
            int participacionId, int cantidadUnidades, CancellationToken ct)
        {
            using var con = _cf.Create();
            using var cmd = new SqlCommand("dbo.sp_ActualizarCantidadUnidades", con)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@ParticipacionID", SqlDbType.Int) { Value = participacionId });
            cmd.Parameters.Add(new SqlParameter("@CantidadUnidades", SqlDbType.Int) { Value = cantidadUnidades });

            try
            {
                await con.OpenAsync(ct);
                using var rd = await cmd.ExecuteReaderAsync(ct);
                if (!rd.HasRows || !await rd.ReadAsync(ct)) return null;
                return Map(rd);
            }
            catch (SqlException ex)
            {
                // Mensajes de RAISERROR del SP
                throw new InvalidOperationException(ex.Message, ex);
            }
        }
    }
}
