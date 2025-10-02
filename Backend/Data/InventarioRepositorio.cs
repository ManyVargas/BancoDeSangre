using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Backend.Data
{
    public class InventarioRepositorio : IInventarioRepositorio
    {
        private readonly IConnectionFactory _connectionFactory;
        public InventarioRepositorio(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ActualizarEstatusInventarioDTO> ActualizarEstatusInvenario(int inventarioID, int estatusID)
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("sp_ActualizarEstatusInventario", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@InventarioID", inventarioID);
            cmd.Parameters.AddWithValue("@EstatusID", estatusID);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return new ActualizarEstatusInventarioDTO
            {
                InventarioID = inventarioID,
                EstatusID = estatusID,
            };
        }


        public async Task<ListarInventarioDTO> ActualizarInventario(int bancoId, int tipoDeSangreId, int nuevaCantidad)
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("dbo.sp_ActualizarInventarioPorBanco", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@BancoID", SqlDbType.Int) { Value = bancoId });
            cmd.Parameters.Add(new SqlParameter("@TipoSangreID", SqlDbType.Int) { Value = tipoDeSangreId });
            cmd.Parameters.Add(new SqlParameter("@NuevaCantidad", SqlDbType.Int) { Value = nuevaCantidad });
            

            await con.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();

            if (!rd.HasRows)
                throw new Exception("No se encontró inventario para ese BancoID y TipoSangreID.");

            await rd.ReadAsync();

            return new ListarInventarioDTO
            {
                InventarioID = rd.GetInt32(rd.GetOrdinal("InventarioID")),
                NombreDeBanco = rd.GetString(rd.GetOrdinal("Nombre_Banco_sangre")),
                TipoDeSangre = rd.GetString(rd.GetOrdinal("TipoDeSangre")),
                CantidadesUnidades = rd.GetInt32(rd.GetOrdinal("CantidadUnidades")),
                FechaActualizacion = rd.GetDateTime(rd.GetOrdinal("FechaActualizacion")),
                FechaVencimiento = rd.GetDateTime(rd.GetOrdinal("FechaVencimiento")),
                Estatus= rd.GetString(rd.GetOrdinal("Estatus"))
            };
        }

        // --------------------------------------------------------------------
        // LISTAR (correcto): dbo.sp_ListarInventarioPorBanco
        // Parámetro: @BancoID
        // Devuelve VARIAS filas
        // --------------------------------------------------------------------
        public async Task<IEnumerable<ListarInventarioDTO>> ListarInventario(int bancoId)
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("dbo.sp_ListarInventarioPorBanco", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@BancoID", SqlDbType.Int) { Value = bancoId });

            await con.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();

            var list = new List<ListarInventarioDTO>();
            while (await rd.ReadAsync())
            {
                list.Add(new ListarInventarioDTO
                {
                    InventarioID = rd.GetInt32(rd.GetOrdinal("InventarioID")),
                    NombreDeBanco = rd.GetString(rd.GetOrdinal("Nombre_Banco_sangre")),
                    TipoDeSangre = rd.GetString(rd.GetOrdinal("TipoDeSangre")),
                    CantidadesUnidades = rd.GetInt32(rd.GetOrdinal("CantidadUnidades")),
                    FechaActualizacion = rd.GetDateTime(rd.GetOrdinal("FechaActualizacion")),
                    FechaVencimiento = rd.GetDateTime(rd.GetOrdinal("FechaVencimiento")),
                    Estatus = rd.GetString(rd.GetOrdinal("Estatus"))
                });
            }

            return list;
        }


        // --------------------------------------------------------------------
        // REGISTRAR: dbo.sp_RegistrarInventario
        // Parámetros: @BancoID, @TipoSangreID, @CantidadUnidades, @FechaVencimiento, @EstatusID
        // Devuelve la fila insertada (NO trae los nombres del banco ni tipo de sangre)
        // --------------------------------------------------------------------
        public async Task<InventarioDto> RegistrarInventario(RegistrarInventarioDto dto)
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("dbo.sp_RegistrarInventario", con)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@BancoID", SqlDbType.Int) { Value = dto.BancoID });
            cmd.Parameters.Add(new SqlParameter("@TipoSangreID", SqlDbType.Int) { Value = dto.TipoDeSangreID });
            cmd.Parameters.Add(new SqlParameter("@CantidadUnidades", SqlDbType.Int) { Value = dto.CantidadesUnidades });
            cmd.Parameters.Add(new SqlParameter("@FechaVencimiento", SqlDbType.Date) { Value = dto.FechaVencimiento.Date });
            cmd.Parameters.Add(new SqlParameter("@EstatusID", SqlDbType.Int) { Value = dto.EstatusID });

            await con.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();

            if (!rd.HasRows)
                throw new Exception("No se pudo registrar el inventario.");

            await rd.ReadAsync();

            return new InventarioDto
            {
                InventarioID = rd.GetInt32(rd.GetOrdinal("InventarioID")),
                NombreDeBanco = rd.GetString(rd.GetOrdinal("Nombre_Banco_sangre")),
                TipoDeSangre = rd.GetString(rd.GetOrdinal("TipoDeSangre")),
                CantidadesUnidades = rd.GetInt32(rd.GetOrdinal("CantidadUnidades")),
                FechaActualizacion = rd.GetDateTime(rd.GetOrdinal("FechaActualizacion")),
                FechaVencimiento = rd.GetDateTime(rd.GetOrdinal("FechaVencimiento")),
                EstatusID = rd.GetInt32(rd.GetOrdinal("EstatusID"))
            };
        }
    }
}
