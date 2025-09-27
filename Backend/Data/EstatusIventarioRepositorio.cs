using System.Data;
using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.Data.SqlClient;

namespace Backend.Data
{
    public class EstatusInventarioRepositorio : IEstatusInventarioRepositorio
    {
        private readonly IConnectionFactory _cf;
        public EstatusInventarioRepositorio(IConnectionFactory cf) => _cf = cf;

        public async Task<IEnumerable<EstatusInventarioDTO>> ListarAsync(CancellationToken ct)
        {
            using var con = _cf.Create();
            using var cmd = new SqlCommand("dbo.sp_ListarEstatusInventario", con) { CommandType = CommandType.StoredProcedure };

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

        public async Task<EstatusInventarioDTO?> ObtenerPorIdAsync(int id, CancellationToken ct)
        {
            using var con = _cf.Create();
            using var cmd = new SqlCommand("SELECT EstatusID, Nombre, Descripcion FROM EstatusInventario WHERE EstatusID=@id", con);
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });

            await con.OpenAsync(ct);
            using var rd = await cmd.ExecuteReaderAsync(ct);

            if (!await rd.ReadAsync(ct)) return null;

            return new EstatusInventarioDTO
            {
                EstatusID = rd.GetInt32("EstatusID"),
                Nombre = rd.GetString("Nombre"),
                Descripcion = rd.IsDBNull("Descripcion") ? null : rd.GetString("Descripcion")
            };
        }

        public async Task<int> CrearAsync(CrearEstatusInventarioDTO dto, CancellationToken ct)
        {
            using var con = _cf.Create();
            using var cmd = new SqlCommand("INSERT INTO EstatusInventario (Nombre, Descripcion) OUTPUT INSERTED.EstatusID VALUES (@n, @d)", con);
            cmd.Parameters.Add(new SqlParameter("@n", SqlDbType.NVarChar, 100) { Value = dto.Nombre });
            cmd.Parameters.Add(new SqlParameter("@d", SqlDbType.NVarChar, 500) { Value = (object?)dto.Descripcion ?? DBNull.Value });

            await con.OpenAsync(ct);
            var id = (int)await cmd.ExecuteScalarAsync(ct);
            return id;
        }

        public async Task<EstatusInventarioDTO?> ActualizarAsync(int id, ActualizarEstatusInventarioDTO dto, CancellationToken ct)
        {
            using var con = _cf.Create();
            using var cmd = new SqlCommand(@"
                UPDATE EstatusInventario SET Nombre=@n, Descripcion=@d WHERE EstatusID=@id;
                SELECT EstatusID, Nombre, Descripcion FROM EstatusInventario WHERE EstatusID=@id;", con);

            cmd.Parameters.Add(new SqlParameter("@n", SqlDbType.NVarChar, 100) { Value = dto.Nombre });
            cmd.Parameters.Add(new SqlParameter("@d", SqlDbType.NVarChar, 500) { Value = (object?)dto.Descripcion ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });

            await con.OpenAsync(ct);
            using var rd = await cmd.ExecuteReaderAsync(ct);
            if (!await rd.ReadAsync(ct)) return null;

            return new EstatusInventarioDTO
            {
                EstatusID = rd.GetInt32("EstatusID"),
                Nombre = rd.GetString("Nombre"),
                Descripcion = rd.IsDBNull("Descripcion") ? null : rd.GetString("Descripcion")
            };
        }

        public async Task<bool> EliminarAsync(int id, CancellationToken ct)
        {
            using var con = _cf.Create();
            using var cmd = new SqlCommand("DELETE FROM EstatusInventario WHERE EstatusID=@id; SELECT @@ROWCOUNT;", con);
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });

            await con.OpenAsync(ct);
            var rows = (int)await cmd.ExecuteScalarAsync(ct);
            return rows > 0;
        }
    }
}
