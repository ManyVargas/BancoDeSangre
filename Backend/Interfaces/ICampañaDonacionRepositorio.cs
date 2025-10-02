using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.DTOs;

namespace Backend.Interfaces
{
    public interface ICampanaDonacionRepositorio
    {
        Task<IEnumerable<ListarCampansaDonacionDto>> ListarCampanasAsync();
        Task<int> CrearCampanaAsync(CrearCampanaDonacionDto dto);
    }

}
