using Backend.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Interfaces
{
    public interface IInventarioRepositorio
    {
        Task<InventarioDto> RegistrarInventario(RegistrarInventarioDto inventarioDto);
        Task<IEnumerable<ListarInventarioDTO>> ListarInventario(int bancoID);
        Task<ListarInventarioDTO> ActualizarInventario(int bancoId, int tipoDeSangreId, int nuevaCantidad);
        Task<ActualizarEstatusInventarioDTO> ActualizarEstatusInvenario(int InventarioID, int EstatusID);
    }
}
