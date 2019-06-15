using System.Data;

namespace TownOfBlakulla.Core.Db
{
    public interface IDbConnectionFactory
    {
        IDbConnection Get();
    }
}