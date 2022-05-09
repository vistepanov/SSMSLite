using System.Collections.Generic;
using System.Threading.Tasks;
using SsmsLite.Core.Integration.ObjectExplorer;

namespace Demo.Services
{
    public class DemoObjectExplorerInteraction : IObjectExplorerInteraction
    {
        Task IObjectExplorerInteraction.SelectNodeAsync(string server, string dbName, IReadOnlyCollection<string> itemPath)
        {
            return Task.CompletedTask;
        }
    }
}
