using System.Collections.Generic;
using System.Threading.Tasks;

namespace SsmsLite.Core.Integration.ObjectExplorer
{
    public interface IObjectExplorerInteraction
    {
        Task SelectNodeAsync(string server, string database, IReadOnlyCollection<string> itemPath);
    }
}