using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.Data.SqlClient;

namespace Backend.Data
{
    public class CampanaDonacionRepositorio : ICampanaDonacionRepositorio
    {
        private readonly IConnectionFactory _connectionFactory;

        public CampanaDonacionRepositorio(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<ListarCampansaDonacionDto>> ListarCampanasAsync()
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("dbo.sp_ListarCampanas", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<ListarCampansaDonacionDto>();
            while (await reader.ReadAsync())
            {
                list.Add(new ListarCampansaDonacionDto
                {
                    CampanaID = reader.GetInt32(reader.GetOrdinal("CampanaID")),
                    Nombre = reader.GetString(reader.GetOrdinal("NombreCampaña")),
                    Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                    FechaInicio = reader.GetDateTime(reader.GetOrdinal("FechaInicio")),
                    FechaFin = reader.GetDateTime(reader.GetOrdinal("FechaFin")),
                    Ubicacion = reader.GetString(reader.GetOrdinal("Ubicacion")),
                    BancoNombre = reader.GetString(reader.GetOrdinal("NombreBanco"))
                });
            }
            return list;
        }

        public async Task<int> CrearCampanaAsync(CrearCampanaDonacionDto dto)
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("dbo.sp_RegistrarCampaña", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 100) { Value = dto.Nombre });
            cmd.Parameters.Add(new SqlParameter("@Descripcion", SqlDbType.NVarChar, -1) { Value = (object?)dto.Descripcion ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@FechaInicio", SqlDbType.DateTime2){ Value = dto.FechaInicio });
            cmd.Parameters.Add(new SqlParameter("@FechaFin", SqlDbType.DateTime2) { Value = dto.FechaFin });
            cmd.Parameters.Add(new SqlParameter("@Ubicacion", SqlDbType.NVarChar, 200) { Value = dto.Ubicacion });
            cmd.Parameters.Add(new SqlParameter("@BancoID", SqlDbType.Int) { Value = dto.BancoID });


            await con.OpenAsync();
            var id = (int)await cmd.ExecuteScalarAsync();
            return id;
        }
    }
}
