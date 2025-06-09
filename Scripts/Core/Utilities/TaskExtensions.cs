using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Utilities
{
    /// <summary>
    /// Taskに関する拡張メソッド
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// タイムアウト付きで実行
        /// </summary>
        public static async Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeout)
        {
            using var cts = new CancellationTokenSource(timeout);
            try
            {
                return await task.WaitAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException();
            }
        }

        /// <summary>
        /// リトライ付き実行
        /// </summary>
        public static async Task<T> WithRetry<T>(this Func<Task<T>> taskFactory, int maxRetries)
        {
            Exception? last = null;
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    return await taskFactory();
                }
                catch (Exception ex)
                {
                    last = ex;
                    if (i < maxRetries - 1)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i)));
                    }
                }
            }
            throw last ?? new Exception("Unknown error");
        }
    }
}
