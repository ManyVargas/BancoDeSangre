using Backend.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DTOs
{
    public class SolicitudesDonacionRepositorio : ISolicitudesDonacionRepositorio
    {
        private readonly IConnectionFactory _connectionFactory;

        public SolicitudesDonacionRepositorio(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // -----------------------------------------------------------
        // CREAR: dbo.sp_CrearSolicitudDonacion
        // Devuelve el nuevo SolicitudID (SCOPE_IDENTITY)
        // -----------------------------------------------------------
        public async Task<int> CrearSolicitudesDonacion(CrearSolicitudesDonacionDto dto)
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("dbo.sp_CrearSolicitudDonacion", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@PacienteID", SqlDbType.Int) { Value = dto.PacienteId });
            cmd.Parameters.Add(new SqlParameter("@TipoSangreID", SqlDbType.Int) { Value = dto.TipoSangreId });
            cmd.Parameters.Add(new SqlParameter("@CantidadRequerida", SqlDbType.Int) { Value = dto.CantidadRequerida });
            cmd.Parameters.Add(new SqlParameter("@FechaLimite", SqlDbType.Date) { Value = dto.FechaLimite.Date });
            cmd.Parameters.Add(new SqlParameter("@HospitalID", SqlDbType.Int) { Value = dto.HospitalId });
            cmd.Parameters.Add(new SqlParameter("@Estado", SqlDbType.VarChar, 20) { Value = dto.Estado ?? (object)"Pendiente" });
            cmd.Parameters.Add(new SqlParameter("@DireccionPaciente", SqlDbType.NVarChar, 255) { Value = (object?)dto.DireccionPaciente ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Anonimo", SqlDbType.Bit) { Value = dto.Anonimmo });
            cmd.Parameters.Add(new SqlParameter("@FechaInicio", SqlDbType.DateTime) { Value = dto.FechaInicio });
            cmd.Parameters.Add(new SqlParameter("@BancoID", SqlDbType.Int) { Value = dto.BancoId });

            await con.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();

            if (!rd.HasRows)
                throw new InvalidOperationException("No se pudo crear la solicitud de donación.");

            await rd.ReadAsync();
            return rd.GetInt32(rd.GetOrdinal("SolicitudID"));
        }

        // -----------------------------------------------------------
        // ACTUALIZAR ESTADO: dbo.sp_ActualizarEstadoSolicitud
        // Actualiza y devuelve la SOLICITUD con nombres (join manual)
        // -----------------------------------------------------------
        public async Task<SolicitudesDonacionDto> ActualizarEstadoSolicitud(int solicitudId, string estado)
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("dbo.sp_ActualizarEstadoSolicitud", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@SolicitudID", SqlDbType.Int) { Value = solicitudId });
            cmd.Parameters.Add(new SqlParameter("@Estado", SqlDbType.VarChar, 20) { Value = estado });

            await con.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();

            if (!rd.HasRows)
                throw new KeyNotFoundException("Solicitud no encontrada.");

            await rd.ReadAsync();

            return new SolicitudesDonacionDto
            {
                SolicituId = rd.GetInt32(rd.GetOrdinal("SolicitudID")), // (tu DTO usa 'SolicituId')
                NombreSolicitante = rd.GetString(rd.GetOrdinal("NombreSolicitante")),
                TipoSangre = rd.GetString(rd.GetOrdinal("TipoSangre")),
                CantidadRequerida = rd.GetInt32(rd.GetOrdinal("CantidadRequerida")),
                FechaLimite = rd.GetDateTime(rd.GetOrdinal("FechaLimite")),
                NombreHospital = rd.GetString(rd.GetOrdinal("NombreHospital")),
                Estado = rd.GetString(rd.GetOrdinal("Estado")),
                DireccionPaciente = rd.IsDBNull(rd.GetOrdinal("DireccionPaciente"))
                                        ? string.Empty
                                        : rd.GetString(rd.GetOrdinal("DireccionPaciente")),
                Anonimmo = rd.GetBoolean(rd.GetOrdinal("Anonimo")),   // tu DTO tiene 'Anonimmo'
                FechaInicio = rd.GetDateTime(rd.GetOrdinal("FechaInicio")),
                NombreBanco = rd.GetString(rd.GetOrdinal("NombreBanco"))
            };
        }


        // -----------------------------------------------------------
        // LISTAR TODAS: dbo.sp_ListarSolicitudes
        // -----------------------------------------------------------
        public async Task<IEnumerable<SolicitudesDonacionDto>> ListarSolicitudes()
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("dbo.sp_ListarSolicitudes", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            await con.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();

            var list = new List<SolicitudesDonacionDto>();
            while (await rd.ReadAsync())
            {
                list.Add(new SolicitudesDonacionDto
                {
                    SolicituId = rd.GetInt32(rd.GetOrdinal("SolicitudID")),
                    NombreSolicitante = rd.GetString(rd.GetOrdinal("NombreSolicitante")),
                    TipoSangre = rd.GetString(rd.GetOrdinal("TipoSangre")),
                    CantidadRequerida = rd.GetInt32(rd.GetOrdinal("CantidadRequerida")),
                    FechaLimite = rd.GetDateTime(rd.GetOrdinal("FechaLimite")),
                    NombreHospital = rd.GetString(rd.GetOrdinal("NombreHospital")),
                    Estado = rd.GetString(rd.GetOrdinal("Estado")),
                    DireccionPaciente = rd.GetString(rd.GetOrdinal("DireccionPaciente")),
                    Anonimmo = rd.GetBoolean(rd.GetOrdinal("Anonimo")),
                    FechaInicio = rd.GetDateTime(rd.GetOrdinal("FechaInicio")),
                    NombreBanco = rd.GetString(rd.GetOrdinal("NombreBanco"))
                });
            }

            return list;
        }

        // -----------------------------------------------------------
        // LISTAR POR ESTADO: dbo.sp_ListarSolicitudesEstado
        // @EstadoSolicitud
        // -----------------------------------------------------------
        public async Task<IEnumerable<SolicitudesDonacionDto>> ListarSolicitudesPorEstado(string estado)
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("dbo.sp_ListarSolicitudesEstado", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@EstadoSolicitud", SqlDbType.VarChar, 20) { Value = estado });

            await con.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();

            var list = new List<SolicitudesDonacionDto>();
            while (await rd.ReadAsync())
            {
                list.Add(new SolicitudesDonacionDto
                {
                    SolicituId = rd.GetInt32(rd.GetOrdinal("SolicitudID")),
                    NombreSolicitante = rd.GetString(rd.GetOrdinal("NombreSolicitante")),
                    TipoSangre = rd.GetString(rd.GetOrdinal("TipoSangre")),
                    CantidadRequerida = rd.GetInt32(rd.GetOrdinal("CantidadRequerida")),
                    FechaLimite = rd.GetDateTime(rd.GetOrdinal("FechaLimite")),
                    NombreHospital = rd.GetString(rd.GetOrdinal("NombreHospital")),
                    Estado = rd.GetString(rd.GetOrdinal("Estado")),
                    DireccionPaciente = rd.GetString(rd.GetOrdinal("DireccionPaciente")),
                    Anonimmo = rd.GetBoolean(rd.GetOrdinal("Anonimo")),
                    FechaInicio = rd.GetDateTime(rd.GetOrdinal("FechaInicio")),
                    NombreBanco = rd.GetString(rd.GetOrdinal("NombreBanco"))
                });
            }

            return list;
        }
    }
}
