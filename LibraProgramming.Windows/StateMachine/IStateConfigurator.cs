using System;

namespace LibraProgramming.Windows.StateMachine
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TTrigger"></typeparam>
    public interface IStateConfigurator<TState, in TTrigger>
        where TState : struct
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        IStateConfigurator<TState, TTrigger> Permit(TTrigger trigger, TState target);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trigger"></param>
        /// <returns></returns>
        IStateConfigurator<TState, TTrigger> Ignore(TTrigger trigger);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IStateConfigurator<TState, TTrigger> OnEnter(Action<TState> action);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IStateConfigurator<TState, TTrigger> OnExit(Action<TState> action);
    }
}