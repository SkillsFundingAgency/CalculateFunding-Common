﻿using System.Linq;
using System.Threading.Tasks;

namespace CalculateFunding.Common.Helpers
{
    public static class TaskHelper
    {
        public static async Task WhenAllAndThrow(params Task[] tasks)
        {
            if (tasks == null) return;

            await Task.WhenAll(tasks);

            foreach (Task task in tasks)
            {
                if (task.Exception != null)
                {
                    throw task.Exception;
                }
            }
        }

        public static async Task<TResult[]> WhenAllAndThrow<TResult>(params Task<TResult>[] tasks)
        {
            if (tasks == null) return new TResult[0];

            await Task.WhenAll(tasks);

            foreach (Task<TResult> task in tasks)
            {
                if (task.Exception != null)
                {
                    throw task.Exception;
                }
            }

            return tasks.Select(_ => _.Result).ToArray();
        }
    }
}
