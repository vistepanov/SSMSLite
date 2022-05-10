using SsmsLite.Core.App;

namespace SsmsLite.Services
{
    public class VersionProvider : IVersionProvider
    {
        public int GetBuild()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            return assembly.GetName().Version.Build;
        }

        public int[] GetBuildAndRevision()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            return new[] { version.Build, version.Revision };
        }
    }
}