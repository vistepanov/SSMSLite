using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.SqlServer.Management.UI.VSIntegration.ObjectExplorer;

namespace SsmsLite.Core.Integration.ObjectExplorer
{
    public static class ObjectExplorerHelper
    {
        private const string DB_URNPATH = "Server/Database";
        private const string SERVER_URNPATH = "Server";

        private static INodeInformation FindSelectedNode(IObjectExplorerService explorerService)
        {
            explorerService.GetSelectedNodes(out var arraySize, out var array);
            return arraySize == 0 ? null : array[0];
        }

        private static object FollowPropertyPath(object value, string path)
        {
            var currentType = value.GetType();

            foreach (var propertyName in path.Split('.'))
            {
                var property = currentType.GetProperty(propertyName,
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (property == null) continue;
                value = property.GetValue(value, null);
                currentType = property.PropertyType;
            }

            return value;
        }

        public static INodeInformation FindSelectedDatabaseNode(IObjectExplorerService explorerService)
        {
            var selectedNode = FindSelectedNode(explorerService);
            return selectedNode == null ? null : FindParentDatabaseNode(selectedNode);
        }

        private static INodeInformation FindParentDatabaseNode(INodeInformation node)
        {
            return FindParentNode(node, DB_URNPATH);
        }

        public static INodeInformation FindParentServerNode(INodeInformation node)
        {
            return FindParentNode(node, SERVER_URNPATH);
        }

        private static INodeInformation FindParentNode(INodeInformation node, string searchUrnPath)
        {
            do
            {
                if (node.UrnPath == searchUrnPath)
                    return node;

                node = node.Parent;
            } while (node != null);

            return null;
        }

        private static IEnumerable<ObjectExplorerServer> GetServersConnection(
            IObjectExplorerService explorerService)
        {
            var hierarchies = FollowPropertyPath(explorerService, "Tree.Hierarchies") as
                IEnumerable<KeyValuePair<string, IExplorerHierarchy>>;

            return (
                from srvHierarchy in hierarchies
                let provider = srvHierarchy.Value.Root as IServiceProvider
                where provider != null
                let nodeInformation = provider.GetService(typeof(INodeInformation)) as INodeInformation
                select new ObjectExplorerServer(nodeInformation, srvHierarchy.Value)
            ).ToList();
        }

        private static void EnumerateChildrenSynchronously(HierarchyTreeNode node)
        {
            var t = node.GetType();
            var method = t.GetMethod("EnumerateChildren", new[] { typeof(bool) });

            if (method != null)
            {
                method.Invoke(node, new object[] { false });
            }
            else
            {
                node.EnumerateChildren();
            }
        }

        private static string GetInvariantPath(this HierarchyTreeNode node)
        {
            return FollowPropertyPath(node, "InvariantPath") as string;
        }

        private static ObjectExplorerServer GetServerHierarchyNode(IObjectExplorerService explorerService,
            string serverName)
        {
            var cnx = GetServersConnection(explorerService);
            return cnx.SingleOrDefault(p =>
                string.Compare(p.ConnectionInfo.ServerName, serverName, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public static HierarchyTreeNode GetObjectHierarchyNode(IObjectExplorerService explorerService,
            string serverName, string dbName, IEnumerable<string> dbRelativePath)
        {
            var server = GetServerHierarchyNode(explorerService, serverName);
            var rootNode = server.Hierarchy.Root;

            var pathSegments = new List<string> { "Databases", dbName }.Concat(dbRelativePath);

            foreach (var pathSegment in pathSegments)
            {
                EnumerateChildrenSynchronously(rootNode);
                var parentPath = rootNode.GetInvariantPath();
                var currentPath = parentPath + "/" + pathSegment;

                rootNode = rootNode.Nodes.OfType<HierarchyTreeNode>()
                    .SingleOrDefault(p => p.GetInvariantPath() == currentPath);
                if (rootNode == null)
                {
                    throw new Exception($"Could not unfold path: {currentPath}");
                }
            }

            return rootNode;
        }

        public static void SelectNode(IObjectExplorerService explorerService, HierarchyTreeNode node)
        {
            if (node is IServiceProvider provider)
            {
                if (provider.GetService(typeof(INodeInformation)) is INodeInformation containedItem)
                {
                    explorerService.SynchronizeTree(containedItem);
                }
            }
        }
    }
}