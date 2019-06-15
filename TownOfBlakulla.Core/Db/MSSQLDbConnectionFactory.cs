using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace TownOfBlakulla.Core.Db
{
    public class MSSQLDbConnectionFactory : IDbConnectionFactory
    {
        private readonly IDbConnectionSettings settings;

        public MSSQLDbConnectionFactory(IDbConnectionSettings settings)
        {
            this.settings = settings;
        }

        public IDbConnection Get()
        {
            return new SqlConnection(settings.ConnectionString);
        }
    }
}
