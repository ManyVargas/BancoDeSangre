using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Interfaces
{
    public interface IResultadosPrueba
    {
        Task<IEnumerable<DTOs.ResultadosPruebaDTO>> ListarResultadosPruebas(CancellationToken ct);
    }
}
