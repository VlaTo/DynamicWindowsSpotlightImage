using System;

namespace LibraProgramming.Windows.ServiceContainer
{
    /// <summary>
    /// Marks preffered ctor for <see cref="ServiceContainer.ServiceLocator" /> class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor)]
    public class PrefferedConstructorAttribute : Attribute
    {
    }
}