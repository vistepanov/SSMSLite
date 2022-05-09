namespace SsmsLite.Core.App
{
    public interface IVersionProvider
    {
        int GetBuild();
        int[] GetBuildAndRevision();
    }
}
