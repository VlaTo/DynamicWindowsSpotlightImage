using System.Windows.Input;
using LibraProgramming.Windows.Commands;

namespace LibraProgramming.Windows.StateMachine
{
    public static class StateMachineExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <typeparam name="TTrigger"></typeparam>
        /// <param name="machine"></param>
        /// <param name="trigger"></param>
        /// <returns></returns>
        public static ICommand CreateCommand<TState, TTrigger>(this StateMachine<TState, TTrigger> machine,
            TTrigger trigger)
            where TState : struct
        {
            return new RelayCommand(
                () => machine.Fire(trigger),
                () => machine.CanFire(trigger)
                );
        }
    }
}