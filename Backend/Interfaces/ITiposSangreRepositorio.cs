using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.DTOs;

namespace Backend.Interfaces
{
    public interface ITiposSangreRepositorio
    {
        Task<IEnumerable<TiposSangreDTO>> ListarTiposSangreAsync(CancellationToken ct);
    }
}
