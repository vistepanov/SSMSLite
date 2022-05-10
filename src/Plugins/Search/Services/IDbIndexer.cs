using System.Threading.Tasks;
using SsmsLite.Core.Integration.Connection;

namespace SsmsLite.Search.Services
{
    public interface IDbIndexer
    {
        int DbExists(DbConnectionString dbConnectionString);
        Task<int> IndexAsync(DbConnectionString dbConnectionString);
        Task<int> ReIndexAsync(DbConnectionString dbConnectionString);
    }
}