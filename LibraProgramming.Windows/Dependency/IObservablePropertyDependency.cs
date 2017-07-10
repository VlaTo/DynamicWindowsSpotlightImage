namespace LibraProgramming.Windows.Dependency
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    internal interface IObservablePropertyDependency<TModel>
        where TModel : ObservableModel
    {
        /// <summary>
        /// 
        /// </summary>
        PropertyPath PropertyPath
        {
            get;
        }
    }
}