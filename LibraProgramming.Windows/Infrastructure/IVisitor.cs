namespace LibraProgramming.Windows.Infrastructure
{
    public interface IVisitor<in T>
    {
        void Visit(T obj);
    }
}