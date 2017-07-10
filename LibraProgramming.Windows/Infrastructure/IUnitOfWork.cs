using System;

namespace LibraProgramming.Windows.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        void Complete();
    }
}