namespace SsmsLite.Core.Utils.Validation
{
    public interface IValidatable<out T>
    {
        T Validate();
    }
}
