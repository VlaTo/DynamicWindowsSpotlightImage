namespace LibraProgramming.Windows.Infrastructure
{
    /// <summary>
    /// Simple object builder interface.
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    public interface IObjectBuilder<out TObject>
    {
        /// <summary>
        /// Constructs target object of type <see cref="TObject" />.
        /// </summary>
        /// <returns></returns>
        TObject Construct();
    }
}