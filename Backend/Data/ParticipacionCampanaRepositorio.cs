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

        public ParticipacionCampanaRepositorio(IConnectionFactory connectionFactory)
        {
            _cf = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<int> CrearAsync(CrearParticipacionCampanaDTO dto, CancellationToken ct)
        {
            try
            {
                using var con = _cf.Create();
                using var cmd = new SqlCommand("sp_RegistrarParticipacionCampaña", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@CampanaID", SqlDbType.Int) { Value = dto.CampanaID });
                cmd.Parameters.Add(new SqlParameter("@DonanteID", SqlDbType.Int) { Value = dto.DonanteID });
                cmd.Parameters.Add(new SqlParameter("@FechaDonacion", SqlDbType.DateTime2) { Value = dto.FechaDonacion });
                cmd.Parameters.Add(new SqlParameter("@CantidadUnidades", SqlDbType.Int) { Value = dto.CantidadUnidades });

                await con.OpenAsync(ct);
                var id = (int)await cmd.ExecuteScalarAsync(ct);
                return id;
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al registrar la participación en la campaña (SQL Error).", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error inesperado al registrar la participación en la campaña.", ex);
            }
        }

        public async Task<ListarParticipantesCampana?> ObtenerPorPaticipantesPorIdAsync(int id, CancellationToken ct)
        {
            try
            {
                using var con = _cf.Create();
                using var cmd = new SqlCommand("sp_ListarParticipantesCampaña", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@CampanaID", SqlDbType.Int) { Value = id });

                await con.OpenAsync(ct);

                using var rd = await cmd.ExecuteReaderAsync(ct);
                if (await rd.ReadAsync(ct))
                {
                    return new ListarParticipantesCampana
                    {
                        ParticipacionID = rd.GetInt32(rd.GetOrdinal("ParticipacionID")),
                        NombreCampana = rd.GetString(rd.GetOrdinal("NombreDeCampaña")),
                        NombreDonante = rd.GetString(rd.GetOrdinal("NombreDonante")),
                        TipoDeSangre = rd.GetString(rd.GetOrdinal("TipoDeSangre")),
                        FechaDonacion = rd.GetDateTime(rd.GetOrdinal("FechaDonacion")),
                        CantidadUnidades = rd.GetInt32(rd.GetOrdinal("CantidadUnidades"))
                    };
                }

                return null;
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al obtener participantes de la campaña (SQL Error).", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error inesperado al obtener participantes de la campaña.", ex);
            }
        }
    }
}
