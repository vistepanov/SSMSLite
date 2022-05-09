using System;

namespace SsmsLite.Core.Ui.Utils
{
    public delegate void ErrorHandler(Exception ex);

    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }
}
