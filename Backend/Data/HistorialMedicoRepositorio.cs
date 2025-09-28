using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Data
{
    public class HistorialMedicoRepositorio : IHistorialMedicoRepositorio
    {
        private readonly IConnectionFactory _connectionFactory;
        public HistorialMedicoRepositorio(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // LISTAR: dbo.sp_ListarHistorialMedico (@DonanteID, @PacienteID)
        public async Task<IEnumerable<HistorialMedicoDto>> ListarHistorialMedico(int? donanteId, int? pacienteId)
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("dbo.sp_ListarHistorialMedico", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@DonanteID", SqlDbType.Int) { Value = (object?)donanteId ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@PacienteID", SqlDbType.Int) { Value = (object?)pacienteId ?? DBNull.Value });

            await con.OpenAsync();

            // (opcional, para verificar que DB estás usando desde la API)
            Console.WriteLine($"[API] Conectado a: {con.DataSource} / BD: {con.Database}");

            using var reader = await cmd.ExecuteReaderAsync();

            // helper mini: si la columna no existe, devuelve -1 en lugar de lanzar excepción
            int TryOrd(string name)
            {
                try { return reader.GetOrdinal(name); } catch { return -1; }
            }

            var list = new List<HistorialMedicoDto>();
            while (await reader.ReadAsync())
            {
                var oHist = reader.GetOrdinal("HistorialID");      // obligatoria
                var oMed = TryOrd("Medicamentos");
                var oApto = TryOrd("AptoParaDonar");
                var oFecha = TryOrd("FechaRevision");
                var oDon = TryOrd("DonanteID");
                var oPac = TryOrd("PacienteID");

                list.Add(new HistorialMedicoDto
                {
                    HistorialID = reader.GetInt32(oHist),
                    Medicamentos = (oMed >= 0 && !reader.IsDBNull(oMed)) ? reader.GetString(oMed) : string.Empty,
                    AptoParaDonar = (oApto >= 0 && !reader.IsDBNull(oApto)) ? reader.GetBoolean(oApto) : false,
                    // tu DTO se llama FehaRevision (sin 'c')
                    FehaRevision = (oFecha >= 0 && !reader.IsDBNull(oFecha)) ? reader.GetDateTime(oFecha) : DateTime.MinValue,
                    DonanteID = (oDon >= 0 && !reader.IsDBNull(oDon)) ? reader.GetInt32(oDon) : (int?)null,
                    PacienteID = (oPac >= 0 && !reader.IsDBNull(oPac)) ? reader.GetInt32(oPac) : (int?)null
                });
            }

            return list;
        }



        // REGISTRAR: dbo.sp_RegistrarHistorialMedico (@Medicamentos, @AptoParaDonar, @FechaRevision, @DonanteID, @PacienteID)
        // Debe devolver al menos "HistorialID" (por OUTPUT o SELECT SCOPE_IDENTITY() AS HistorialID)
        public async Task<int> RegistrarHistorialMedico(RegistrarHistorialMedicoDto dto)
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("dbo.sp_RegistrarHistorialMedico", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@Medicamentos", SqlDbType.NVarChar, -1) { Value = (object?)dto.Medicamentos ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@AptoParaDonar", SqlDbType.Bit) { Value = dto.AptoParaDonar });

            // Si viene DateTime por defecto (0001-01-01), lo mandamos como NULL para que el SP ponga la fecha
            object fechaParam = dto.FechaRevision == default ? DBNull.Value : dto.FechaRevision.Date;
            cmd.Parameters.Add(new SqlParameter("@FechaRevision", SqlDbType.Date) { Value = fechaParam });

            // En tu DTO DonanteID/PacienteID son int no-null; usamos 0 como “no enviado”
            cmd.Parameters.Add(new SqlParameter("@DonanteID", SqlDbType.Int) { Value = dto.DonanteID > 0 ? dto.DonanteID : DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@PacienteID", SqlDbType.Int) { Value = dto.PacienteID > 0 ? dto.PacienteID : DBNull.Value });

            await con.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();

            if (!rd.HasRows) throw new Exception("No se pudo registrar el historial médico.");
            await rd.ReadAsync();

            return rd.GetInt32(rd.GetOrdinal("HistorialID"));
        }

        // ACTUALIZAR: dbo.sp_ActualizarHistorialMedico (valores nuevos + originales + flags IsNull + HistorialID)
        // Devuelve la fila actualizada o null si no coincidió el WHERE (concurrencia/no existe)
        public async Task<HistorialMedicoDto?> ActualizarHistorialMedico(HistorialMedicoActualizarDto dto)
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("dbo.sp_ActualizarHistorialMedico", con)
            { CommandType = CommandType.StoredProcedure };

            // ID a actualizar
            cmd.Parameters.Add(new SqlParameter("@HistorialID", SqlDbType.Int) { Value = dto.HistorialID });

            // Los que mandes NO nulos se actualizan; los NULL NO se tocan
            cmd.Parameters.Add(new SqlParameter("@Medicamentos", SqlDbType.NVarChar, -1) { Value = (object?)dto.Medicamentos ?? DBNull.Value });

            // ⚠️ Si no haces AptoParaDonar nullable en tu DTO, siempre actualizarás ese campo:
            cmd.Parameters.Add(new SqlParameter("@AptoParaDonar", SqlDbType.Bit)
            {
                Value = /* si cambiaste a bool? */ (object?)dto.AptoParaDonar ?? DBNull.Value
                /* si NO cambiaste, usa siempre dto.AptoParaDonar (se actualizará siempre) */
            });

            // Igual para FechaRevision:
            cmd.Parameters.Add(new SqlParameter("@FechaRevision", SqlDbType.Date)
            {
                Value = /* si cambiaste a DateTime? */ (object?)dto.FechaRevision ?? DBNull.Value
                /* si NO cambiaste, usa dto.FechaRevision (se actualizará siempre) */
            });

            cmd.Parameters.Add(new SqlParameter("@DonanteID", SqlDbType.Int) { Value = (object?)dto.DonanteID ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@PacienteID", SqlDbType.Int) { Value = (object?)dto.PacienteID ?? DBNull.Value });

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (!reader.HasRows) return null;
            await reader.ReadAsync();

            return new HistorialMedicoDto
            {
                HistorialID = reader.GetInt32(reader.GetOrdinal("HistorialID")),
                Medicamentos = reader.IsDBNull(reader.GetOrdinal("Medicamentos")) ? null : reader.GetString(reader.GetOrdinal("Medicamentos")),
                AptoParaDonar = reader.GetBoolean(reader.GetOrdinal("AptoParaDonar")),
                FehaRevision = reader.GetDateTime(reader.GetOrdinal("FechaRevision")),
                DonanteID = reader.IsDBNull(reader.GetOrdinal("DonanteID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("DonanteID")),
                PacienteID = reader.IsDBNull(reader.GetOrdinal("PacienteID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("PacienteID"))
            };
        }
    }
}
