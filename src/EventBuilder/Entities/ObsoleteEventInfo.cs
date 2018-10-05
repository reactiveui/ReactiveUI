namespace EventBuilder.Entities
{
    /// <summary>
    /// Represents an obsolete event.
    /// </summary>
    public class ObsoleteEventInfo
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the obsolete event is an error.
        /// </summary>
        public bool IsError { get; set; }
    }
}
