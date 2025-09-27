using System.Data;
using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.Data.SqlClient;

namespace Backend.Data
{
    public class HistorialEnfermedadRepositorio : IHistorialEnfermedadRepositorio
    {
        private readonly IConnectionFactory _cf;
        public HistorialEnfermedadRepositorio(IConnectionFactory cf) => _cf = cf;

        // INSERT relación via SP: dbo.sp_RegistrarHistorialEnfermedad
        public async Task<bool> AgregarAsync(HistorialEnfermedadDTO dto, CancellationToken ct)
        {
            using var con = _cf.Create();
            using var cmd = new SqlCommand("dbo.sp_RegistrarHistorialEnfermedad", con)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@HistorialID", SqlDbType.Int) { Value = dto.HistorialID });
            cmd.Parameters.Add(new SqlParameter("@EnfermedadID", SqlDbType.Int) { Value = dto.EnfermedadID });

            await con.OpenAsync(ct);
            await cmd.ExecuteNonQueryAsync(ct);
            return true;
        }

        // SELECT via SP: dbo.sp_ListarEnfermedadesPorHistorial
        public async Task<IEnumerable<HistorialEnfermedadDetalleDTO>> ListarPorHistorialAsync(int historialId, CancellationToken ct)
        {
            using var con = _cf.Create();
            using var cmd = new SqlCommand("dbo.sp_ListarEnfermedadesPorHistorial", con)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@HistorialID", SqlDbType.Int) { Value = historialId });

            await con.OpenAsync(ct);
            using var rd = await cmd.ExecuteReaderAsync(ct);

            var list = new List<HistorialEnfermedadDetalleDTO>();
            while (await rd.ReadAsync(ct))
            {
                list.Add(new HistorialEnfermedadDetalleDTO
                {
                    HistorialID = rd.GetInt32(rd.GetOrdinal("HistorialID")),
                    EnfermedadID = rd.GetInt32(rd.GetOrdinal("EnfermedadID")),
                    NombreEnfermedad = rd.GetString(rd.GetOrdinal("NombreEnfermedad")),
                    Descripcion = rd.IsDBNull(rd.GetOrdinal("Descripcion")) ? null : rd.GetString(rd.GetOrdinal("Descripcion"))
                });
            }
            return list;
        }
    }
}
