using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Data
{
    public class BancoDeSangreRepositorio : IBancoDeSangreRepositorio
    {
        private readonly IConnectionFactory _connectionFactory;
        public BancoDeSangreRepositorio(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<BancoDeSangreDto>> ListarBancosDeSangre()
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("sp_ListarBancos", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            await con.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<BancoDeSangreDto>();
            while(await reader.ReadAsync())
            {
                list.Add(new BancoDeSangreDto
                {
                    BancoId = reader.GetInt32(reader.GetOrdinal("BancoID")),
                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                    Direccion = reader.GetString(reader.GetOrdinal("Direccion")),
                    Telefono = reader.GetString(reader.GetOrdinal("Telefono")),
                    Longitud = reader.GetDecimal(reader.GetOrdinal("Longitud")),
                    Latitud = reader.GetDecimal(reader.GetOrdinal("Latitud")),
                    SitioWeb = reader.GetString(reader.GetOrdinal("SitioWeb")),
                    CorreoElectronico = reader.GetString(reader.GetOrdinal("CorreoElectronico")),
                    RNC = reader.GetString(reader.GetOrdinal("RNC"))
                });
            }
            return list;
        }

        public async Task<int> RegistrarBancoDeSangre(RegistrarBancoDeSangreDto dto)
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("sp_RegistrarBancoSangre", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 100) { Value = dto.Nombre });
            cmd.Parameters.Add(new SqlParameter("@Direccion", SqlDbType.NVarChar, 200) { Value = dto.Direccion });
            cmd.Parameters.Add(new SqlParameter("@Telefono", SqlDbType.NVarChar, 20) { Value = (object?)dto.Telefono ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Longitud", SqlDbType.Decimal, 9) { Value = dto.Longitud });
            cmd.Parameters.Add(new SqlParameter("@Latitud", SqlDbType.Decimal, 9) { Value = dto.Latitud });
            cmd.Parameters.Add(new SqlParameter("@SitioWeb", SqlDbType.NVarChar, 255) { Value = dto.SitioWeb });
            cmd.Parameters.Add(new SqlParameter("@CorreoElectronico", SqlDbType.NVarChar, 100) { Value = dto.CorreoElectronico });
            cmd.Parameters.Add(new SqlParameter("@RNC", SqlDbType.NVarChar, 20) { Value = dto.RNC });

            await con.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();

            if (!rd.HasRows) throw new Exception("No se pudo registrar el Banco de sangre.");
            await rd.ReadAsync();
            return rd.GetInt32(rd.GetOrdinal("BancoID"));
        }
    }
}
