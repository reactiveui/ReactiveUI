using System;

namespace ReactiveUI
{
    /// <summary>
    /// Indicates that an object implementing <see cref="IHandleObservableErrors"/> has errored and nothing is attached
    /// to <see cref="IHandleObservableErrors.ThrownExceptions"/> to handle that error.
    /// </summary>
    public class UnhandledErrorException : Exception
    {
        /// <summary>
        /// Creates a new instance of <c>UnhandledErrorException</c>.
        /// </summary>
        public UnhandledErrorException()
        {
        }

        /// <summary>
        /// Creates a new instance of <c>UnhandledErrorException</c>.
        /// </summary>
        /// <param name="message">
        /// The exception message.
        /// </param>
        public UnhandledErrorException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a new instance of <c>UnhandledErrorException</c>.
        /// </summary>
        /// <param name="message">
        /// The exception message.
        /// </param>
        /// <param name="innerException">
        /// The exception that caused this exception.
        /// </param>
        public UnhandledErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}