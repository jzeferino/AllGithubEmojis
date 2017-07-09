using System;
using System.Threading.Tasks;

namespace AllGithubEmojis.Core.Extensions
{
    public static class TaskEx
    {
        public static async Task<Tuple<T1, T2>> WhenAll<T1, T2>(Task<T1> task1, Task<T2> task2)
        {
            await Task.WhenAll(task1, task2);
            return new Tuple<T1, T2>(task1.Result, task2.Result);
        }
    }
}
