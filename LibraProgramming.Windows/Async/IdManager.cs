using System.Threading;

namespace LibraProgramming.Windows.Async
{
    //ReSharper disable UnusedTypeParameter
    internal static class IdManager<TTag>
    //ReSharper restore UnusedTypeParameter
    {
        private static int lastId;

        public static int GetId(ref int id)
        {
            if (0 != id)
            {
                return id;
            }

            int candidate;

            do
            {
                candidate = Interlocked.Increment(ref lastId);
            } while (0 == candidate);

            Interlocked.CompareExchange(ref id, candidate, 0);

            return id;
        }
    }
}