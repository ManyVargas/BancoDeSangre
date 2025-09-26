using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.DTOs;

namespace Backend.Interfaces
{
    public interface IPruebasLaboratorio
    {
        Task<IEnumerable<ListarPruebasLaboratorioDTO>> ListarPruebasLaboratorio(CancellationToken ct);
        Task<IEnumerable<ListarPruebasLaboratorioDTO>> ListarPruebasPorRangoDeFechas(DateTime fechaInicio, DateTime FechaFin,CancellationToken ct);
        Task<int> RegistrarPruebaLaboratorio(RegistrarPruebaLaboratorioDTO prueba, CancellationToken ct);
        Task<IEnumerable< ListarPruebasLaboratorioDTO>> ListarPruebasLaboratorioPorDonante(int? donanteId, CancellationToken ct);
        Task<ListarPruebasLaboratorioDTO> ListarPruebaPorID(int? PruebaID, CancellationToken ct);
        Task<IEnumerable<ListarPruebasLaboratorioDTO>> ListarPruebaPorTipo(string Tipo, CancellationToken ct);
        Task<ListarPruebasLaboratorioDTO?> ActualizarResultadoPrueba(int PruebaID, int ResultadoID, string Observaciones, ActualizarResultadoPrueba dto, CancellationToken ct);
        Task<bool> EliminarPruebaLaboratorio(int pruebaId, CancellationToken ct);
        
    }
}
