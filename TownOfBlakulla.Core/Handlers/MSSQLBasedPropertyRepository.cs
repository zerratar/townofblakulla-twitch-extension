using System;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Newtonsoft.Json;
using TownOfBlakulla.Core.Db;

namespace TownOfBlakulla.Core.Handlers
{
    public class MSSQLBasedPropertyRepository : IPropertyRepository
    {
        private readonly ILogger logger;
        private readonly IDbConnectionFactory connectionFactory;

        public MSSQLBasedPropertyRepository(ILogger logger, IDbConnectionFactory connectionFactory)
        {
            this.logger = logger;
            this.connectionFactory = connectionFactory;
        }

        public T Load<T>(string propertyName)
        {
            try
            {
                using (var connection = this.connectionFactory.Get())
                {
                    // new SqlParameter("p0", SqlDbType.VarChar) { Value = propertyName }
                    var data = connection.QueryFirstOrDefault<string>($"SELECT TOP 1 PropValue FROM [dbo].[PropValues] WHERE PropName = '{propertyName}'");
                    if (!string.IsNullOrEmpty(data))
                    {
                        return JsonConvert.DeserializeObject<T>(data);
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(exc.ToString());
            }
            return default(T);
        }

        public void Save<T>(string propertyName, T value)
        {
            try
            {
                using (var connection = this.connectionFactory.Get())
                {
                    // new SqlParameter("p0", SqlDbType.VarChar) { Value = propertyName }
                    // darn SqlParameter didnt work...
                    // SQLInjection, here we come!
                    var count = connection.QueryFirstOrDefault<int>(
                        $"SELECT COUNT(*) FROM [dbo].[PropValues] WHERE PropName = '{propertyName}'");

                    var stringValue = JsonConvert.SerializeObject(value);
                    connection.Query(
                        count >= 1
                            ? $"UPDATE [dbo].[PropValues] SET PropValue = '{stringValue}' WHERE PropName = '{propertyName}'"
                            : $"INSERT INTO [dbo].[PropValues] (PropName, PropValue) VALUES('{propertyName}', '{stringValue}')");
                }
            }
            catch (Exception exc)
            {
                logger.Error(exc.ToString());
            }
        }
    }
}