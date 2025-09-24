using Backend.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Data
{
    public class SqlConnectionFactory : IConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("default")
                ?? throw new InvalidOperationException("Connection string 'Default' no encontrada.");
        }
        public SqlConnection Create() => new SqlConnection(_connectionString);
        
    }
}
