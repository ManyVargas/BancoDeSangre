using System.Data;
using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.Data.SqlClient;

namespace Backend.Data
{
    public class RolRepositorio : IRolRepositorio
    {
        private readonly IConnectionFactory _cf;
        public RolRepositorio(IConnectionFactory cf) => _cf = cf;

        public async Task<IEnumerable<RolDTO>> ListarAsync(CancellationToken ct)
        {
            using var con = _cf.Create();
            using var cmd = new SqlCommand("dbo.sp_ListarRoles", con)
            { CommandType = CommandType.StoredProcedure };

            await con.OpenAsync(ct);
            using var rd = await cmd.ExecuteReaderAsync(ct);

            var list = new List<RolDTO>();
            while (await rd.ReadAsync(ct))
            {
                list.Add(new RolDTO
                {
                    RolID = rd.GetInt32(rd.GetOrdinal("RolID")),
                    NombreRol = rd.GetString(rd.GetOrdinal("NombreRol")),
                    Descripcion = rd.IsDBNull(rd.GetOrdinal("Descripcion")) ? null : rd.GetString(rd.GetOrdinal("Descripcion")),
                    Activo = rd.GetBoolean(rd.GetOrdinal("Activo"))
                });
            }
            return list;
        }

        // Si no tienes un SP específico para obtener por ID, puedes quitar este método o usar un SELECT simple.
        public async Task<RolDTO?> ObtenerPorIdAsync(int id, CancellationToken ct)
        {
            using var con = _cf.Create();
            using var cmd = new SqlCommand(
                "SELECT RolID, NombreRol, Descripcion, Activo FROM Roles WHERE RolID=@id", con);
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });

            await con.OpenAsync(ct);
            using var rd = await cmd.ExecuteReaderAsync(ct);
            if (!await rd.ReadAsync(ct)) return null;

            return new RolDTO
            {
                RolID = rd.GetInt32(rd.GetOrdinal("RolID")),
                NombreRol = rd.GetString(rd.GetOrdinal("NombreRol")),
                Descripcion = rd.IsDBNull(rd.GetOrdinal("Descripcion")) ? null : rd.GetString(rd.GetOrdinal("Descripcion")),
                Activo = rd.GetBoolean(rd.GetOrdinal("Activo"))
            };
        }

        public async Task<int> CrearAsync(CrearRolDTO dto, CancellationToken ct)
        {
            using var con = _cf.Create();
            using var cmd = new SqlCommand("dbo.sp_RegistrarRol", con)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@NombreRol", SqlDbType.NVarChar, 100) { Value = dto.NombreRol });
            cmd.Parameters.Add(new SqlParameter("@Descripcion", SqlDbType.NVarChar, 500) { Value = (object?)dto.Descripcion ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Activo", SqlDbType.Bit) { Value = dto.Activo });

            await con.OpenAsync(ct);
            // El SP debe devolver el id (OUTPUT o SELECT SCOPE_IDENTITY())
            var scalar = await cmd.ExecuteScalarAsync(ct);
            return Convert.ToInt32(scalar);
        }

        public async Task<RolDTO?> ActualizarAsync(int id, ActualizarRolDTO dto, CancellationToken ct)
        {
            using var con = _cf.Create();
            using var cmd = new SqlCommand("dbo.sp_ActualizarRol", con)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@RolID", SqlDbType.Int) { Value = id });
            cmd.Parameters.Add(new SqlParameter("@NombreRol", SqlDbType.NVarChar, 100) { Value = dto.NombreRol });
            cmd.Parameters.Add(new SqlParameter("@Descripcion", SqlDbType.NVarChar, 500) { Value = (object?)dto.Descripcion ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Activo", SqlDbType.Bit) { Value = dto.Activo });

            await con.OpenAsync(ct);
            using var rd = await cmd.ExecuteReaderAsync(ct);

            if (!rd.HasRows || !await rd.ReadAsync(ct)) return null;

            return new RolDTO
            {
                RolID = rd.GetInt32(rd.GetOrdinal("RolID")),
                NombreRol = rd.GetString(rd.GetOrdinal("NombreRol")),
                Descripcion = rd.IsDBNull(rd.GetOrdinal("Descripcion")) ? null : rd.GetString(rd.GetOrdinal("Descripcion")),
                Activo = rd.GetBoolean(rd.GetOrdinal("Activo"))
            };
        }

        public async Task<bool> EliminarAsync(int id, CancellationToken ct)
        {
            using var con = _cf.Create();
            using var cmd = new SqlCommand("dbo.sp_EliminarRol", con)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@RolID", SqlDbType.Int) { Value = id });

            await con.OpenAsync(ct);
            // Si tu SP hace SELECT @@ROWCOUNT, usa ExecuteScalar; si no, ExecuteNonQuery.
            var rowsObj = await cmd.ExecuteScalarAsync(ct);
            if (rowsObj is not null && int.TryParse(rowsObj.ToString(), out var rows))
                return rows > 0;

            var nonQuery = await cmd.ExecuteNonQueryAsync(ct);
            return nonQuery > 0;
        }
    }
}
