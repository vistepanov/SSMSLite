using System;
using System.Threading.Tasks;

namespace SsmsLite.Core.Ui.Utils
{
    public static class TaskExtensions
    {
        public static async Task FireAndForgetSafeAsync(this Task task, ErrorHandler handler = null)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                handler?.Invoke(ex);
            }
        }
    }
}
