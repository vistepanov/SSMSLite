using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;

namespace SsmsLite.Core.Integration.ObjectExplorer
{
    public class ObjectExplorerServer
    {
        public SqlConnectionInfo ConnectionInfo => NodeInformation.Connection as SqlConnectionInfo;
        public INodeInformation NodeInformation { get; }
        public IExplorerHierarchy Hierarchy { get; }
        public HierarchyTreeNode Root => Hierarchy.Root;

        public ObjectExplorerServer(INodeInformation nodeInformation, IExplorerHierarchy hierarchy)
        {
            NodeInformation = nodeInformation;
            Hierarchy = hierarchy;
        }
    }
}