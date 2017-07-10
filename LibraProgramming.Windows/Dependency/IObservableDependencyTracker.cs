namespace LibraProgramming.Windows.Dependency
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IObservablePropertyTracker<in TModel>
        where TModel : ObservableModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        IObservablePropertyTrackerSubscription Subscribe(TModel model);
    }
}