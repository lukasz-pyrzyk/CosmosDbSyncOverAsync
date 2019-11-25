using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CosmosdbHang.Repository
{
    internal class AsyncLazy<T> : Lazy<Task<T>>
    {
        internal AsyncLazy(Func<Task<T>> taskFactory) :
            base(() => Task.Factory.StartNew(taskFactory).Unwrap())
        { }

        internal ConfiguredTaskAwaitable<T>.ConfiguredTaskAwaiter GetAwaiter()
        {
            var t = Value.ConfigureAwait(false);
            return t.GetAwaiter();
        }
    }
}