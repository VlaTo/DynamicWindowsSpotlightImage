using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibraProgramming.Windows.Infrastructure
{
    public static class TaskExtension
    {
        public static void WaitAndUnwrapException(this Task @this)
        {
            @this.GetAwaiter().GetResult();
        }

        public static void WaitAndUnwrapException(this Task @this, CancellationToken ct)
        {
            try
            {
                @this.Wait(ct);
            }
            catch (AggregateException exception)
            {
                throw ExceptionHelper.PrepareForRethrow(exception.InnerException);
            }
        }

        public static TResult WaitAndUnwrapException<TResult>(this Task<TResult> @this)
        {
            return @this.GetAwaiter().GetResult();
        }

        public static TResult WaitAndUnwrapException<TResult>(this Task<TResult> @this, CancellationToken ct)
        {
            try
            {
                @this.Wait(ct);
                return @this.Result;
            }
            catch (AggregateException exception)
            {
                throw ExceptionHelper.PrepareForRethrow(exception.InnerException);
            }
        }

        public static void WaitWithoutException(this Task @this)
        {
            if (@this.IsCompleted)
            {
                return;
            }

            var result = (IAsyncResult) @this;

            result.AsyncWaitHandle.WaitOne();
        }

        public static void WaitWithoutException(this Task @this, CancellationToken ct)
        {
            if (@this.IsCompleted)
            {
                return;
            }

            ct.ThrowIfCancellationRequested();

            var index = WaitHandle.WaitAny(new[]
            {
                ((IAsyncResult) @this).AsyncWaitHandle, ct.WaitHandle
            });

            if (0 != index)
            {
                ct.ThrowIfCancellationRequested();
            }
        }
    }
}