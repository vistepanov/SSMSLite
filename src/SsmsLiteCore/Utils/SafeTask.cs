using System;
using System.Threading.Tasks;

namespace SsmsLite.Core.Utils
{
    public static class SafeTask
    {
        public static Task RunAsync(Func<Task> function, Action<Exception> errorHandler)
        {
            return Task.Run(async () =>
            {
                try
                {
                    await function();
                }
                catch (Exception ex)
                {
                    errorHandler(ex);
                }
            });
        }

        public static async Task RunSafeAsync(Func<Task> function, Action<Exception> errorHandler)
        {
            try
            {
                await function();
            }
            catch (Exception ex)
            {
                errorHandler(ex);
            }

        }
    }
}
