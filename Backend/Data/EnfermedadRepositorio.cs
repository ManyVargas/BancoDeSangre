using System.Data;
using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.Data.SqlClient;

namespace Backend.Data
{
    public class EnfermedadRepositorio : IEnfermedadRepositorio
    {
        private readonly IConnectionFactory _cf;
        public EnfermedadRepositorio(IConnectionFactory cf) => _cf = cf;

        // SP real: dbo.sp_RegistrarEnfermedades
        public async Task<int> CrearAsync(CrearEnfermedadDTO dto, CancellationToken ct)
        {
            using var con = _cf.Create();
            using var cmd = new SqlCommand("dbo.sp_RegistrarEnfermedades", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@NombreEnfermedad", SqlDbType.NVarChar, 100) { Value = dto.NombreEnfermedad });
            cmd.Parameters.Add(new SqlParameter("@Descripcion", SqlDbType.NVarChar, 500) { Value = (object?)dto.Descripcion ?? DBNull.Value });

            await con.OpenAsync(ct);
            using var rd = await cmd.ExecuteReaderAsync(ct);
            if (!rd.HasRows) throw new InvalidOperationException("No se pudo registrar la enfermedad.");
            await rd.ReadAsync(ct);
            return rd.GetInt32("EnfermedadID");
        }

        // SQL directo (no hay SP)
        public async Task<IEnumerable<EnfermedadDTO>> ListarAsync(CancellationToken ct)
        {
            using var con = _cf.Create();
            using var cmd = new SqlCommand(
                "SELECT EnfermedadID, NombreEnfermedad, Descripcion FROM dbo.CatalogoEnfermedades ORDER BY NombreEnfermedad", con);

            await con.OpenAsync(ct);
            using var rd = await cmd.ExecuteReaderAsync(ct);
            var list = new List<EnfermedadDTO>();
            while (await rd.ReadAsync(ct))
            {
                list.Add(new EnfermedadDTO
                {
                    EnfermedadID = rd.GetInt32("EnfermedadID"),
                    NombreEnfermedad = rd.GetString("NombreEnfermedad"),
                    Descripcion = rd.IsDBNull("Descripcion") ? null : rd.GetString("Descripcion")
                });
            }
            return list;
        }

        public async Task<EnfermedadDTO?> ObtenerPorIdAsync(int id, CancellationToken ct)
        {
            using var con = _cf.Create();
            using var cmd = new SqlCommand(
                "SELECT EnfermedadID, NombreEnfermedad, Descripcion FROM dbo.CatalogoEnfermedades WHERE EnfermedadID=@id", con);
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });

            await con.OpenAsync(ct);
            using var rd = await cmd.ExecuteReaderAsync(ct);
            if (!await rd.ReadAsync(ct)) return null;

            return new EnfermedadDTO
            {
                EnfermedadID = rd.GetInt32("EnfermedadID"),
                NombreEnfermedad = rd.GetString("NombreEnfermedad"),
                Descripcion = rd.IsDBNull("Descripcion") ? null : rd.GetString("Descripcion")
            };
        }

        public async Task<EnfermedadDTO?> ActualizarAsync(int id, ActualizarEnfermedadDTO dto, CancellationToken ct)
        {
            using var con = _cf.Create();
            using var cmd = new SqlCommand(
                @"UPDATE dbo.CatalogoEnfermedades 
                  SET NombreEnfermedad=@n, Descripcion=@d 
                  WHERE EnfermedadID=@id;
                  SELECT EnfermedadID, NombreEnfermedad, Descripcion 
                  FROM dbo.CatalogoEnfermedades WHERE EnfermedadID=@id;", con);

            cmd.Parameters.Add(new SqlParameter("@n", SqlDbType.NVarChar, 100) { Value = dto.NombreEnfermedad });
            cmd.Parameters.Add(new SqlParameter("@d", SqlDbType.NVarChar, 500) { Value = (object?)dto.Descripcion ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });

            await con.OpenAsync(ct);
            using var rd = await cmd.ExecuteReaderAsync(ct);
            if (!await rd.ReadAsync(ct)) return null;

            return new EnfermedadDTO
            {
                EnfermedadID = rd.GetInt32("EnfermedadID"),
                NombreEnfermedad = rd.GetString("NombreEnfermedad"),
                Descripcion = rd.IsDBNull("Descripcion") ? null : rd.GetString("Descripcion")
            };
        }

        public async Task<bool> EliminarAsync(int id, CancellationToken ct)
        {
            using var con = _cf.Create();
            using var cmd = new SqlCommand("DELETE dbo.CatalogoEnfermedades WHERE EnfermedadID=@id; SELECT @@ROWCOUNT;", con);
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });

            await con.OpenAsync(ct);
            var rows = (int)await cmd.ExecuteScalarAsync(ct);
            return rows > 0;
        }
    }
}
