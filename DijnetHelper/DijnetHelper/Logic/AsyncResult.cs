using System;
using System.Threading;
using System.Threading.Tasks;

namespace DijnetHelper.Logic
{
    // async result provider
    public class AsyncResult<T> : IDisposable
    {
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(0, 1);

        public bool HasResult { get; set; }

        public T Result { get; private set; }

        public void Set(T result)
        {
            Result = result;
            HasResult = true;
            semaphore.Release();
        }

        // async method cannot have out parameter, access result by Result property, if returned true
        public async Task<bool> WaitAsync(TimeSpan timeout)
        {
            Result = default;
            HasResult = false;
            return await semaphore.WaitAsync(timeout);
        }

        public void Dispose()
        {
            semaphore.Dispose();
        }
    }
}
