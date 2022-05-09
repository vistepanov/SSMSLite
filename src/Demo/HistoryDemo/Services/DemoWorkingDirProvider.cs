using System.IO;
using System.Reflection;
using SsmsLite.Core.App;

namespace Demo.Services
{
    public class DemoWorkingDirProvider : IWorkingDirProvider
    {
        private string _cachedWorkingDir = string.Empty;

        public string GetWorkingDir()
        {
            if (string.IsNullOrEmpty(_cachedWorkingDir))
            {
                _cachedWorkingDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "SSMS Plus");
                Directory.CreateDirectory(_cachedWorkingDir);
            }

            return _cachedWorkingDir;
        }
    }
}
