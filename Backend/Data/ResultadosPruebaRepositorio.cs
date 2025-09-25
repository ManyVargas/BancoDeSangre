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
    public class ResultadosPruebaRepositorio : IResultadosPrueba
    {
        private readonly IConnectionFactory _connectionFactory;
        public ResultadosPruebaRepositorio(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<IEnumerable<ResultadosPruebaDTO>> ListarResultadosPruebas(CancellationToken ct)
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("sp_ListarResultadosPrueba", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            await con.OpenAsync(ct);

            using var rd = await cmd.ExecuteReaderAsync(ct);

            var list = new List<ResultadosPruebaDTO>();

            while (await rd.ReadAsync(ct))
            {
                list.Add(new ResultadosPruebaDTO
                {
                    ResultadoID = rd.GetInt32(rd.GetOrdinal("ResultadoID")),
                    Nombre = rd.GetString(rd.GetOrdinal("Nombre")),
                    Descripcion = rd.GetString(rd.GetOrdinal("Descripcion"))
                });
            }
            return list;

        }
    }
}
