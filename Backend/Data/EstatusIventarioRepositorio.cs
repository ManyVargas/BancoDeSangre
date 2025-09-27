using System.Data;
using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.Data.SqlClient;

namespace Backend.Data
{
    public class EstatusInventarioRepositorio : IEstatusInventarioRepositorio
    {
        private readonly IConnectionFactory _cf;

        public EstatusInventarioRepositorio(IConnectionFactory connectionFactory)
        {
            _cf = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public async Task<IEnumerable<EstatusInventarioDTO>> ListarAsync(CancellationToken ct)
        {
            try
            {
                using var con = _cf.Create();
                using var cmd = new SqlCommand("dbo.sp_ListarEstatusInventario", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                await con.OpenAsync(ct);
                using var rd = await cmd.ExecuteReaderAsync(ct);

                var list = new List<EstatusInventarioDTO>();

                while (await rd.ReadAsync(ct))
                {
                    list.Add(new EstatusInventarioDTO
                    {
                        EstatusID = rd.GetInt32("EstatusID"),
                        Nombre = rd.GetString("Nombre"),
                        Descripcion = rd.IsDBNull("Descripcion") ? null : rd.GetString("Descripcion")
                    });
                }

                return list;
            }
            catch (SqlException sqlEx)
            {
                throw new InvalidOperationException("Error al acceder a la base de datos al listar los estatus de inventario.", sqlEx);
            }
            catch (OperationCanceledException)
            {
                // Re-lanzar sin envolver, ya que es una cancelación explícita
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ocurrió un error inesperado al listar los estatus de inventario.", ex);
            }
        }

        public async Task<EstatusInventarioDTO?> ObtenerPorIdAsync(int id, CancellationToken ct)
        {
            if (id <= 0)
            {
                return null; // ID inválido, no se lanza excepción, se trata como "no encontrado"
            }

            try
            {
                using var con = _cf.Create();
                using var cmd = new SqlCommand("sp_ListarEstatusInventarioPorID", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add(new SqlParameter("@EstatusID", SqlDbType.Int) { Value = id });

                await con.OpenAsync(ct);
                using var rd = await cmd.ExecuteReaderAsync(ct);

                if (!await rd.ReadAsync(ct))
                {
                    return null;
                }

                return new EstatusInventarioDTO
                {
                    EstatusID = rd.GetInt32("EstatusID"),
                    Nombre = rd.GetString("Nombre"),
                    Descripcion = rd.IsDBNull("Descripcion") ? null : rd.GetString("Descripcion")
                };
            }
            catch (SqlException sqlEx)
            {
                throw new InvalidOperationException($"Error al acceder a la base de datos al buscar el estatus con ID {id}.", sqlEx);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ocurrió un error inesperado al buscar el estatus con ID {id}.", ex);
            }
        }
    }
}