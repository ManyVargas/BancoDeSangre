using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.Data.SqlClient;

namespace Backend.Data
{
    public class DonanteRepositorio : IDonanteRepositorio
    {
        private readonly IConnectionFactory _connectionFactory;
        public DonanteRepositorio(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        //CREAR DONANTE
        public async Task<int> CrearAsync(RegistrarDonanteDTO dto, CancellationToken ct)
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("dbo.sp_RegistrarDonante", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 100) { Value = dto.Nombre });
            cmd.Parameters.Add(new SqlParameter("@CedulaID", SqlDbType.NVarChar, 20) { Value = dto.CedulaID });
            cmd.Parameters.Add(new SqlParameter("@FechaNacimiento", SqlDbType.Date) { Value = dto.FechaNacimiento });
            cmd.Parameters.Add(new SqlParameter("@Telefono", SqlDbType.VarChar, 20) { Value = dto.Telefono });
            cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = dto.Email });
            cmd.Parameters.Add(new SqlParameter("@TipoSangreID", SqlDbType.Int) { Value = dto.TipoSangreID });
            cmd.Parameters.Add(new SqlParameter("@UltimaDonacion", SqlDbType.Date) { Value = dto.UltimaDonacion });
            cmd.Parameters.Add(new SqlParameter("@Latitud", SqlDbType.Decimal) { Value = dto.Latitud });
            cmd.Parameters.Add(new SqlParameter("@Longitud", SqlDbType.Decimal) { Value = dto.Longitud });
            cmd.Parameters.Add(new SqlParameter("@Disponibilidad", SqlDbType.Bit) { Value = dto.Disponibilidad });
           
            await con.OpenAsync(ct);
            using var rd = await cmd.ExecuteReaderAsync(ct);
            // el SP retorna la fila insertada; toma el ID
            if (!rd.HasRows) throw new Exception("No se pudo registrar el donante.");
            await rd.ReadAsync(ct);
            return rd.GetInt32(rd.GetOrdinal("DonanteID"));
        }
        public async Task<DonantesDTO?> ActualizarAsync(int id, ActualizarDonanteDTO dto, CancellationToken ct)
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("dbo.sp_RegistrarDonante", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@DonanteID", SqlDbType.Int) { Value = id });
            cmd.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 100) { Value = dto.Nombre });
            cmd.Parameters.Add(new SqlParameter("@CedulaID", SqlDbType.NVarChar, 20) { Value = dto.CedulaID });
            cmd.Parameters.Add(new SqlParameter("@FechaNacimiento", SqlDbType.Date) { Value = dto.FechaNacimiento });
            cmd.Parameters.Add(new SqlParameter("@Telefono", SqlDbType.VarChar, 20) { Value = (object?)dto.Telefono ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = dto.Email });
            cmd.Parameters.Add(new SqlParameter("@TipoSangreID", SqlDbType.Int) { Value = dto.TipoSangreID });
            cmd.Parameters.Add(new SqlParameter("@UltimaDonacion", SqlDbType.Date) { Value = (object?)dto.UltimaDonacion ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Latitud", SqlDbType.Decimal) { Value = (object?)dto.Latitud ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Longitud", SqlDbType.Decimal) { Value = (object?)dto.Longitud ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Disponibilidad", SqlDbType.Bit) { Value = dto.Disponibilidad });

            await con.OpenAsync();

           using var reader = await cmd.ExecuteReaderAsync(ct);

            if (!reader.HasRows) return null;

            await reader.ReadAsync(ct);

            return new DonantesDTO
            {
                DonanteID = reader.GetInt32(reader.GetOrdinal("DonanteID")),
                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                CedulaID = reader.GetString(reader.GetOrdinal("CedulaID")),
                FechaNacimiento = reader.GetDateTime(reader.GetOrdinal("FechaNacimiento")),
                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString(reader.GetOrdinal("Telefono")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                TipoSangreID = reader.GetInt32(reader.GetOrdinal("TipoSangreID")),
                UltimaDonacion = reader.GetDateTime(reader.GetOrdinal("UltimaDonacion")),
                Latitud = reader.GetDecimal(reader.GetOrdinal("Latitud")),
                Longitud = reader.GetDecimal(reader.GetOrdinal("Longitud")),
                Disponibilidad = reader.GetBoolean(reader.GetOrdinal("Disponibilidad"))
            };

        }

        public async Task<bool> EliminarAsync(int id, CancellationToken ct)
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("dbo.sp_EliminarDonante", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@DonanteID", SqlDbType.Int) { Value = id });

            await con.OpenAsync(ct);

            var filasAfectadas = await cmd.ExecuteNonQueryAsync(ct);

            return filasAfectadas > 0;
        }

        public async Task<IEnumerable<DonantesDTO>> ListarAsync(CancellationToken ct)
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("dbo.sp_ListarDonantes", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            await con.OpenAsync(ct);
            using var reader = await cmd.ExecuteReaderAsync(ct);
            
            var list = new List<DonantesDTO>();
            while (await reader.ReadAsync(ct))
            {
                list.Add(new DonantesDTO
                {
                    DonanteID = reader.GetInt32(reader.GetOrdinal("DonanteID")),
                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                    CedulaID = reader.GetString(reader.GetOrdinal("CedulaID")),
                    FechaNacimiento = reader.GetDateTime(reader.GetOrdinal("FechaNacimiento")),
                    Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString(reader.GetOrdinal("Telefono")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    TipoSangreID = reader.GetInt32(reader.GetOrdinal("TipoSangreID")),
                    UltimaDonacion = reader.GetDateTime(reader.GetOrdinal("UltimaDonacion")),
                    Latitud = reader.GetDecimal(reader.GetOrdinal("Latitud")),
                    Longitud = reader.GetDecimal(reader.GetOrdinal("Longitud")),
                    Disponibilidad = reader.GetBoolean(reader.GetOrdinal("Disponibilidad"))
                });
            }
            return list;

        }

       public async Task<IEnumerable<DonantesDTO>> ListarPorTipoSangreAsync(string tipoSangre, CancellationToken ct)
{
    using var con = _connectionFactory.Create();
    using var cmd = new SqlCommand("dbo.sp_ListarDonantesPorTipoSangre", con)
    {
        CommandType = CommandType.StoredProcedure
    };

    // Agregar parámetro para filtrar por tipo de sangre
    cmd.Parameters.Add(new SqlParameter("@TipoSangre", SqlDbType.NVarChar, 10) { Value = tipoSangre });

    await con.OpenAsync(ct);

    using var reader = await cmd.ExecuteReaderAsync(ct);

    var list = new List<DonantesDTO>();
    while (await reader.ReadAsync(ct))
    {
        list.Add(new DonantesDTO
        {
            DonanteID = reader.GetInt32(reader.GetOrdinal("DonanteID")),
            Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
            CedulaID = reader.GetString(reader.GetOrdinal("CedulaID")),
            FechaNacimiento = reader.GetDateTime(reader.GetOrdinal("FechaNacimiento")),
            Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString(reader.GetOrdinal("Telefono")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            TipoSangreID = reader.GetInt32(reader.GetOrdinal("TipoSangreID")),
            UltimaDonacion = reader.GetDateTime(reader.GetOrdinal("UltimaDonacion")),
            Latitud = reader.GetDecimal(reader.GetOrdinal("Latitud")),
            Longitud = reader.GetDecimal(reader.GetOrdinal("Longitud")),
            Disponibilidad = reader.GetBoolean(reader.GetOrdinal("Disponibilidad"))
        });
    }

    return list;
}


        public async Task<DonantesDTO?> ObtenerPorIdAsync(int id, CancellationToken ct)
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("dbo.sp_ObtenerDonantePorId", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Agregar parámetro para el ID del donante
            cmd.Parameters.Add(new SqlParameter("@DonanteID", SqlDbType.Int) { Value = id });

            await con.OpenAsync(ct);

            using var reader = await cmd.ExecuteReaderAsync(ct);

            if (await reader.ReadAsync(ct))
            {
                return new DonantesDTO
                {
                    DonanteID = reader.GetInt32(reader.GetOrdinal("DonanteID")),
                    Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                    CedulaID = reader.GetString(reader.GetOrdinal("CedulaID")),
                    FechaNacimiento = reader.GetDateTime(reader.GetOrdinal("FechaNacimiento")),
                    Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString(reader.GetOrdinal("Telefono")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    TipoSangreID = reader.GetInt32(reader.GetOrdinal("TipoSangreID")),
                    UltimaDonacion = reader.GetDateTime(reader.GetOrdinal("UltimaDonacion")),
                    Latitud = reader.GetDecimal(reader.GetOrdinal("Latitud")),
                    Longitud = reader.GetDecimal(reader.GetOrdinal("Longitud")),
                    Disponibilidad = reader.GetBoolean(reader.GetOrdinal("Disponibilidad"))
                };
            }

            // Si no se encontró ningún registro, devuelve null
            return null;
        }

    }
}
