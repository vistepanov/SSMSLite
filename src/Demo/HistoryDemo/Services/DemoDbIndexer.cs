using SSMSPlusSearch.Services;
using System.Threading.Tasks;
using SsmsLite.Core.Integration.Connection;

namespace Demo.Services
{
    public class DemoDbIndexer : IDbIndexer
    {
        private const int DbId = 2;

        int IDbIndexer.DbExists(DbConnectionString dbConnectionString)
        {
            Task.Delay(1000);
            return DbId;
        }

        async Task<int> IDbIndexer.IndexAsync(DbConnectionString dbConnectionString)
        {
            await Task.Delay(2000);
            return DbId;
        }

        async Task<int> IDbIndexer.ReIndexAsync(DbConnectionString dbConnectionString)
        {
            await Task.Delay(2000);
            return DbId;
        }
    }
}