using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.Data.SqlClient;

namespace Backend.Data
{
    public class TipoSangreRepositorio : ITiposSangreRepositorio
    {
        private readonly IConnectionFactory _connectionFactory;

        public TipoSangreRepositorio(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<IEnumerable<TiposSangreDTO>> ListarTiposSangreAsync(CancellationToken ct)
        {
            try
            {
                using var con = _connectionFactory.Create();
                using var cmd = new SqlCommand("dbo.sp_ListarTiposDeSangre", con)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };

                await con.OpenAsync(ct);

                using var reader = await cmd.ExecuteReaderAsync(ct);

                var list = new List<TiposSangreDTO>();
                while (await reader.ReadAsync(ct))
                {
                    list.Add(new TiposSangreDTO
                    {
                        TipoSangreID = reader.GetInt32("TipoSangreID"),
                        Tipo = reader.GetString("Tipo") 
                    });
                }
                return list;
            }
            catch (SqlException sqlEx)
            {
                throw new InvalidOperationException($"Error al listar tipos de sangre: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error inesperado al listar tipos de sangre: {ex.Message}", ex);
            }
        }
    }
}