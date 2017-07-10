using System;

namespace LibraProgramming.Windows.Dependency
{
    /// <summary>
    /// 
    /// </summary>
    public class ObservablePropertyTrackerException : Exception
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:System.Exception"/>.
        /// </summary>
        public ObservablePropertyTrackerException()
        {
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:System.Exception"/> с указанным сообщением об ошибке.
        /// </summary>
        /// <param name="message">
        /// Сообщение, описывающее ошибку.
        /// </param>
        public ObservablePropertyTrackerException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:System.Exception"/> с указанным сообщением
        /// об ошибке и ссылкой на внутреннее исключение, вызвавшее данное исключение.
        /// </summary>
        /// <param name="message">
        /// Сообщение об ошибке, указывающее причину создания исключения.
        /// </param>
        /// <param name="innerException">
        /// Исключение, вызвавшее текущее исключение, или пустая ссылка (Nothing в Visual Basic), если внутреннее исключение не задано.
        /// </param>
        public ObservablePropertyTrackerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}