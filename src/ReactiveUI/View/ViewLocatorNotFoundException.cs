using System;
using System.Runtime.Serialization;

namespace ReactiveUI
{
    /// <summary>
    /// An exception that is thrown if we are unable to find the View Locator.
    /// </summary>
    [Serializable]
    public class ViewLocatorNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewLocatorNotFoundException"/> class.
        /// </summary>
        public ViewLocatorNotFoundException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewLocatorNotFoundException"/> class.
        /// </summary>
        /// <param name="message">A user friendly message.</param>
        public ViewLocatorNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewLocatorNotFoundException"/> class.
        /// </summary>
        /// <param name="message">A user friendly message.</param>
        /// <param name="innerException">Any exception this exception is wrapping.</param>
        public ViewLocatorNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewLocatorNotFoundException"/> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        protected ViewLocatorNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
