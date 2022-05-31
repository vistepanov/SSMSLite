namespace SsmsLite.Core.App
{
    public interface IVersionProvider
    {
        int[] GetBuildAndRevision();
    }
}
