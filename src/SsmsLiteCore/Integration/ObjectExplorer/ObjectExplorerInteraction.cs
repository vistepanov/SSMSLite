using System.Collections.Generic;
using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;

namespace SsmsLite.Core.Integration.ObjectExplorer
{
    public class ObjectExplorerInteraction : IObjectExplorerInteraction
    {
        private readonly PackageProvider _packageProvider;

        public ObjectExplorerInteraction(PackageProvider packageProvider)
        {
            _packageProvider = packageProvider;
        }

        public async System.Threading.Tasks.Task SelectNodeAsync(string server, string dbName, IReadOnlyCollection<string> itemPath)
        {
            var objectExplorer = (await _packageProvider.AsyncPackage.GetServiceAsync(typeof(IObjectExplorerService))) as IObjectExplorerService;
            var objNode = ObjectExplorerHelper.GetObjectHierarchyNode(objectExplorer, server, dbName, itemPath);
            ObjectExplorerHelper.SelectNode(objectExplorer, objNode);
        }
    }
}
