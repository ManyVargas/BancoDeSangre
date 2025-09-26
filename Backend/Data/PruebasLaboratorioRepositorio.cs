using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.Data.SqlClient;

namespace Backend.Data
{
    public class PruebasLaboratorioRepositorio : IPruebasLaboratorio
    {
        private readonly IConnectionFactory _connectionFactory;
        public PruebasLaboratorioRepositorio(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ListarPruebasLaboratorioDTO?> ActualizarResultadoPrueba(int PruebaID,int ResultadoID,string Observaciones, ActualizarResultadoPrueba dto , CancellationToken ct)
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("sp_ActualizarPruebaLaboratorio", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter("@PruebaID", SqlDbType.Int) { Value = PruebaID });
            cmd.Parameters.Add(new SqlParameter("ResultadoID", SqlDbType.Int) { Value = ResultadoID });
            cmd.Parameters.Add(new SqlParameter("Observaciones", SqlDbType.NVarChar, 255) {Value = Observaciones });

            await con.OpenAsync(ct);

            using var rd = await cmd.ExecuteReaderAsync(ct);

            if (await rd.ReadAsync(ct))
            {
                return new ListarPruebasLaboratorioDTO
                {
                    PruebaID = rd.GetInt32(rd.GetOrdinal("PruebaID")),
                    NombreDonante = rd.GetString(rd.GetOrdinal("NombreDonante")),
                    Resultado = rd.GetString(rd.GetOrdinal("ResultadoPrueba")),
                    TipoPrueba = rd.GetString(rd.GetOrdinal("TipoPrueba")),
                    FechaPrueba = rd.GetDateTime(rd.GetOrdinal("FechaPrueba")),
                    Laboratorio = rd.GetString(rd.GetOrdinal("Laboratorio")),
                    ValidezHasta = rd.GetDateTime(rd.GetOrdinal("ValidezHasta")),
                    Observaciones = rd.IsDBNull(rd.GetOrdinal("Observaciones")) ? null : rd.GetString(rd.GetOrdinal("Observaciones"))
                };
            }
            return null;
        }

        public async Task<bool> EliminarPruebaLaboratorio(int pruebaId, CancellationToken ct)
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("sp_EliminarPruebaLaboratorio", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter("@PruebaID", SqlDbType.Int) { Value = pruebaId });

            await con.OpenAsync(ct);

           using var rd = await cmd.ExecuteReaderAsync(ct);

            if (await rd.ReadAsync(ct))
            {
                var filasAfectadas = rd.GetInt32("FilasAfectadas");
                return filasAfectadas > 0;
            }

            return false;

        }

        public async Task<ListarPruebasLaboratorioDTO> ListarPruebaPorID(int? PruebaID, CancellationToken ct)
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("sp_ListarPruebaPorId", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@PruebaID", SqlDbType.Int) {Value = PruebaID});


            await con.OpenAsync(ct);

            using var rd = await cmd.ExecuteReaderAsync(ct);

            if(await rd.ReadAsync(ct))
            {
                return new ListarPruebasLaboratorioDTO
                {
              
                    PruebaID = rd.GetInt32(rd.GetOrdinal("PruebaID")),
                    NombreDonante = rd.GetString(rd.GetOrdinal("NombreDonante")),
                    Resultado = rd.GetString(rd.GetOrdinal("ResultadoPrueba")),
                    TipoPrueba = rd.GetString(rd.GetOrdinal("TipoPrueba")),
                    FechaPrueba = rd.GetDateTime(rd.GetOrdinal("FechaPrueba")),
                    Laboratorio = rd.GetString(rd.GetOrdinal("Laboratorio")),
                    ValidezHasta = rd.GetDateTime(rd.GetOrdinal("ValidezHasta")),
                    Observaciones = rd.IsDBNull(rd.GetOrdinal("Observaciones")) ? null : rd.GetString(rd.GetOrdinal("Observaciones"))
                };
            }
            return null;
        }

        public async Task<IEnumerable<ListarPruebasLaboratorioDTO>> ListarPruebaPorTipo(string Tipo, CancellationToken ct)
        {

            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("sp_ListarPruebasPorTipo", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@TipoPrueba", SqlDbType.NVarChar,20) { Value = Tipo });

            await con.OpenAsync(ct);

            using var rd = await cmd.ExecuteReaderAsync(ct);

            var list = new List<ListarPruebasLaboratorioDTO>();

            while (await rd.ReadAsync(ct))
            {
                list.Add(new ListarPruebasLaboratorioDTO
                {
                    PruebaID = rd.GetInt32(rd.GetOrdinal("PruebaID")),
                    NombreDonante = rd.GetString(rd.GetOrdinal("NombreDonante")),
                    Resultado = rd.GetString(rd.GetOrdinal("ResultadoPrueba")),
                    TipoPrueba = rd.GetString(rd.GetOrdinal("TipoPrueba")),
                    FechaPrueba = rd.GetDateTime(rd.GetOrdinal("FechaPrueba")),
                    Laboratorio = rd.GetString(rd.GetOrdinal("Laboratorio")),
                    ValidezHasta = rd.GetDateTime(rd.GetOrdinal("ValidezHasta")),
                    Observaciones = rd.IsDBNull(rd.GetOrdinal("Observaciones")) ? null : rd.GetString(rd.GetOrdinal("Observaciones"))
                });
            }
            return list;
        }
        

        public async Task<IEnumerable<ListarPruebasLaboratorioDTO>> ListarPruebasLaboratorio(CancellationToken ct)
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("sp_ListarPruebasLaboratorio", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            await con.OpenAsync(ct);

            using var rd = await cmd.ExecuteReaderAsync(ct);
            var list = new List<ListarPruebasLaboratorioDTO>();

            while (await rd.ReadAsync(ct))
            {
                list.Add(new ListarPruebasLaboratorioDTO
                {
                    PruebaID = rd.GetInt32(rd.GetOrdinal("PruebaID")),
                    NombreDonante = rd.GetString(rd.GetOrdinal("Nombre")),
                    Resultado = rd.GetString(rd.GetOrdinal("Resultado")),
                    TipoPrueba = rd.GetString(rd.GetOrdinal("TipoPrueba")),
                    FechaPrueba = rd.GetDateTime(rd.GetOrdinal("FechaPrueba")),
                    Laboratorio = rd.GetString(rd.GetOrdinal("Laboratorio")),
                    ValidezHasta = rd.GetDateTime(rd.GetOrdinal("ValidezHasta")),
                    Observaciones = rd.IsDBNull(rd.GetOrdinal("Observaciones")) ? null : rd.GetString(rd.GetOrdinal("Observaciones"))
                });
            }
            return list;
        }

        public async Task<IEnumerable<ListarPruebasLaboratorioDTO>> ListarPruebasLaboratorioPorDonante(int? donanteId, CancellationToken ct)
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("sp_ListarPruebasPorDonante", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@DonanteID", SqlDbType.Int) { Value = donanteId });

            await con.OpenAsync(ct);

            using var rd = await cmd.ExecuteReaderAsync(ct);
            var list = new List<ListarPruebasLaboratorioDTO>();

            while (await rd.ReadAsync(ct))
            {
                list.Add(new ListarPruebasLaboratorioDTO
                {
                    PruebaID = rd.GetInt32(rd.GetOrdinal("PruebaID")),
                    NombreDonante = rd.GetString(rd.GetOrdinal("NombreDonante")),
                    Resultado = rd.GetString(rd.GetOrdinal("ResultadoPrueba")),
                    TipoPrueba = rd.GetString(rd.GetOrdinal("TipoPrueba")),
                    FechaPrueba = rd.GetDateTime(rd.GetOrdinal("FechaPrueba")),
                    Laboratorio = rd.GetString(rd.GetOrdinal("Laboratorio")),
                    ValidezHasta = rd.GetDateTime(rd.GetOrdinal("ValidezHasta")),
                    Observaciones = rd.IsDBNull(rd.GetOrdinal("Observaciones")) ? null : rd.GetString(rd.GetOrdinal("Observaciones"))
                });
            }
            return list;

            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ListarPruebasLaboratorioDTO>> ListarPruebasPorRangoDeFechas(DateTime FechaInicio, DateTime FechaFin, CancellationToken ct)
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("sp_ListarPruebasPorRangoFechas", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("FechaInicio", SqlDbType.DateTime) { Value = FechaInicio });
            cmd.Parameters.Add(new SqlParameter("FechaFin", SqlDbType.DateTime) { Value = FechaFin });

            await con.OpenAsync(ct);

            using var rd = await cmd.ExecuteReaderAsync(ct);
            var list = new List<ListarPruebasLaboratorioDTO>();
            while (await rd.ReadAsync(ct))
            {
                list.Add(new ListarPruebasLaboratorioDTO
                {
                    PruebaID = rd.GetInt32(rd.GetOrdinal("PruebaID")),
                    NombreDonante = rd.GetString(rd.GetOrdinal("NombreDonante")),
                    Resultado = rd.GetString(rd.GetOrdinal("ResultadoPrueba")),
                    TipoPrueba = rd.GetString(rd.GetOrdinal("TipoPrueba")),
                    FechaPrueba = rd.GetDateTime(rd.GetOrdinal("FechaPrueba")),
                    Laboratorio = rd.GetString(rd.GetOrdinal("Laboratorio")),
                    ValidezHasta = rd.GetDateTime(rd.GetOrdinal("ValidezHasta")),
                    Observaciones = rd.IsDBNull(rd.GetOrdinal("Observaciones")) ? null : rd.GetString(rd.GetOrdinal("Observaciones"))
                });
            }
            return list;
        }

        public async Task<int> RegistrarPruebaLaboratorio(RegistrarPruebaLaboratorioDTO prueba, CancellationToken ct)
        {
            using var con = _connectionFactory.Create();
            using var cmd = new SqlCommand("sp_RegistrarPruebaLaboratorio", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter("@DonanteID", SqlDbType.Int) { Value = prueba.DonanteID });
            cmd.Parameters.Add(new SqlParameter("@ResultadoID", SqlDbType.Int) { Value = prueba.ResultadoID });
            cmd.Parameters.Add(new SqlParameter("@TipoPrueba", SqlDbType.NVarChar, 20) { Value = prueba.TipoPrueba });
            cmd.Parameters.Add(new SqlParameter("@FechaPrueba", SqlDbType.DateTime) { Value = prueba.FechaPrueba });
            cmd.Parameters.Add(new SqlParameter("@Laboratorio", SqlDbType.NVarChar, 100) { Value = prueba.Laboratorio });
            cmd.Parameters.Add(new SqlParameter("@ValidezHasta", SqlDbType.DateTime) { Value = prueba.ValidezHasta });
            cmd.Parameters.Add(new SqlParameter("@Observaciones", SqlDbType.NVarChar, 500) { Value = (object?)prueba.Observaciones ?? DBNull.Value });
            
            await con.OpenAsync(ct);

            using var rd = await cmd.ExecuteReaderAsync(ct);

            if (!rd.HasRows)
                throw new InvalidOperationException("No se pudo registrar el donante.");

            await rd.ReadAsync(ct);

            return rd.GetInt32("NuevaPruebaID");

        }
    }
}
