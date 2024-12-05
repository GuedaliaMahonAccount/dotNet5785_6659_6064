namespace BO
{
    /// <summary>
    /// Represents exceptions that occur in the business logic layer.
    /// </summary>
    public class LogicException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogicException"/> class.
        /// </summary>
        public LogicException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogicException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public LogicException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogicException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public LogicException(string message, Exception innerException) : base(message, innerException) { }
    }
}
