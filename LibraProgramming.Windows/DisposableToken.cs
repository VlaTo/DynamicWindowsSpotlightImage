using System;

namespace LibraProgramming.Windows
{
    /// <summary>
    /// 
    /// </summary>
    public class DisposableToken : IDisposable
    {
        private readonly Action cleanup;

        public DisposableToken(Action cleanup)
        {
            if (null == cleanup)
            {
                throw new ArgumentNullException(nameof(cleanup));
            }

            this.cleanup = cleanup;
        }

        public virtual void Dispose()
        {
            cleanup.Invoke();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    public class DisposableToken<TObject> : IDisposable
    {
        private readonly TObject obj;
        private readonly Action<TObject> cleanup;

        public DisposableToken(TObject obj, Action<TObject> cleanup)
        {
            if (null == obj)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (null == cleanup)
            {
                throw new ArgumentNullException(nameof(cleanup));
            }

            this.obj = obj;
            this.cleanup = cleanup;
        }

        public void Dispose()
        {
            cleanup.Invoke(obj);
        }
    }
}