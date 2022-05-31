using SsmsLite.Core.App;

namespace Demo.Services
{
    public class DemoVersionProvider : IVersionProvider
    {
        public int[] GetBuildAndRevision()
        {
            return new[] { 1, 99 };
        }
    }
}
