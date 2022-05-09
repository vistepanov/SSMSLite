using System;
using System.Threading;
using System.Threading.Tasks;

namespace SsmsLite.Core.Utils
{
    public static class FuncHelper
    {
        public static Action Debounce(this Action func, int milliseconds = 300)
        {
            var debounceFunc = Debounce<bool>((a) => func(), milliseconds);
            return () => debounceFunc(true);
        }

        public static Action<T> Debounce<T>(this Action<T> func, int milliseconds = 300)
        {
            var last = 0;
            return arg =>
            {
                var current = Interlocked.Increment(ref last);
                Task.Delay(milliseconds).ContinueWith(task =>
                {
                    if (current == last) func(arg);
                    task.Dispose();
                });
            };
        }

        public static Func<Task> Debounce(this Func<Task> func, int milliseconds = 300)
        {
            var debounceFun = Debounce<bool>((a) => func(), milliseconds);
            return () => debounceFun(true);
        }

        private static Func<T, Task> Debounce<T>(this Func<T, Task> func, int milliseconds = 300)
        {
            var last = 0;
            return (arg) =>
            {
                var current = Interlocked.Increment(ref last);

                Task.Delay(milliseconds).ContinueWith(task =>
                {
                    if (current == last)
                        func(arg);
                    task.Dispose();
                });

                return Task.CompletedTask;
            };
        }
    }
}