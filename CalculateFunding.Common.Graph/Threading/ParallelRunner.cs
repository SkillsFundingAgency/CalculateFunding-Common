using System;
using System.Threading;
using System.Threading.Tasks;
using CalculateFunding.Common.Helpers;

namespace CalculateFunding.Common.Graph.Threading
{
    public static class ParallelRunner
    {
        public static async Task RunForAllItems<TItem>(TItem[] items,
            Func<TItem, Task> invocation,
            int degreeOfParallelism = 5)
        {
            Task[] invocations = new Task[items.Length];
            SemaphoreSlim throttler = new SemaphoreSlim(degreeOfParallelism);

            for (int item = 0; item < items.Length; item++)
            {
                await throttler.WaitAsync();

                TItem currentItem = items[item];

                invocations[item] =
                    Task.Run(async () =>
                    {
                        try
                        {
                            await invocation(currentItem);
                        }
                        finally
                        {
                            throttler.Release();
                        }
                    });
            }

            await TaskHelper.WhenAllAndThrow(invocations);
        }
    }
}