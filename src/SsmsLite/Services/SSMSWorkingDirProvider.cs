using System;
using System.IO;
using SsmsLite.Core.App;

namespace SSMSPlus.Services
{
    public class SsmsWorkingDirProvider : IWorkingDirProvider
    {
        private string _cachedWorkingDir = string.Empty;

        public string GetWorkingDir()
        {
            if (string.IsNullOrEmpty(_cachedWorkingDir))
            {
                _cachedWorkingDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SSMS Plus");
                Directory.CreateDirectory(_cachedWorkingDir);
            }

            return _cachedWorkingDir;
        }
    }
}