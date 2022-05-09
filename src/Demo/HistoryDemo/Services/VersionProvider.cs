using SsmsLite.Core.App;

namespace Demo.Services
{
    public class DemoVersionProvider : IVersionProvider
    {
        public int GetBuild()
        {
            return 99;
        }

        public int[] GetBuildAndRevision()
        {
            return new int[] { 1, 99 };
        }
    }
}
