namespace LibraProgramming.Windows.Collections
{
    public interface ICollectionFilter
    {
        bool CanPassFilter(SourceCollectionView sender, object item);
    }
}