using Backend.DTOs;
using Backend.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Data
{
    public class BancoDeSangreRepositorio : IBancoDeSangreRepositorio
    {
        private readonly IConnectionFactory _connectionFactory;
        public BancoDeSangreRepositorio(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Task<int> RegistrarBancoDeSangre(RegistrarBancoDeSangreDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
