using Microsoft.SqlServer.Management.UI.VSIntegration;
using Microsoft.SqlServer.Management.UI.VSIntegration.Editors;
using Microsoft.VisualStudio.Shell;

namespace SsmsLite.Core.Integration
{
    public class ServiceCacheIntegration : IServiceCacheIntegration
    {
        public void OpenScriptInNewWindow(string script)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            ServiceCache.ScriptFactory.CreateNewBlankScript(ScriptType.Sql);

            var doc = (EnvDTE.TextDocument)ServiceCache.ExtensibilityModel.Application.ActiveDocument.Object(null);
            doc.EndPoint.CreateEditPoint().Insert(script);
        }
    }
}