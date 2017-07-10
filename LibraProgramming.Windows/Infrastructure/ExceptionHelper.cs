using System;
using System.Runtime.ExceptionServices;

namespace LibraProgramming.Windows.Infrastructure
{
    internal static class ExceptionHelper
    {
        internal static Exception PrepareForRethrow(Exception exception)
        {
            ExceptionDispatchInfo.Capture(exception).Throw();
            return exception;
        }
    }
}