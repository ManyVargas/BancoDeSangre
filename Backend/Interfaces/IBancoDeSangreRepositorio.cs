using Backend.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Interfaces
{
    public interface IBancoDeSangreRepositorio
    {
        Task<int> RegistrarBancoDeSangre(RegistrarBancoDeSangreDto dto);
        Task<IEnumerable<BancoDeSangreDto>> ListarBancosDeSangre();
    }
}
