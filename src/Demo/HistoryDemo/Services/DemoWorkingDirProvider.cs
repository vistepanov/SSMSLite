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
                var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
                _cachedWorkingDir = Path.Combine(directoryName, "SsmsLite");
                Directory.CreateDirectory(_cachedWorkingDir);
            }

            return _cachedWorkingDir;
        }
    }
}