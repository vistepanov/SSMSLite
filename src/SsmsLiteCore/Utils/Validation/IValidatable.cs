namespace SsmsLite.Core.Utils.Validation
{
    public interface IValidatable<T>
    {
        T Validate();
    }
}
