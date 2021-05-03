using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Lofi.API.Tests
{
    public static class WaitUtilities
    {
        public static async Task<(bool Success, T? Result, TimeSpan Elapsed)> WaitUntil<T>(Func<Task<(bool Success, T? result)>> condition, TimeSpan? timeout = null, TimeSpan? pause = null)
        {
            timeout ??= TimeSpan.FromSeconds(10);
            pause ??= TimeSpan.FromMilliseconds(100);

            var elapsed = TimeSpan.Zero;
            while (elapsed < timeout)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var (success, result) = await condition();
                stopwatch.Stop();
                elapsed += stopwatch.Elapsed;

                if (success) return (success, result, elapsed);

                await Task.Delay(pause.Value);
                elapsed += pause.Value;
            }

            return (false, default, elapsed);
        }
    }
}