namespace SkillBuddy.Core
{
    /// <summary>
    /// Base exception type for all SkillBuddy-related exceptions.
    /// Inherits from <see cref="System.Exception"/>.
    /// </summary>
    public class SkillBuddyException : Exception
    {
        public SkillBuddyException(string? message)
            : base(message) { }

        public SkillBuddyException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// Exception for service provider-related errors.
    /// </summary>
    public class ServiceProviderException : SkillBuddyException
    {
        public ServiceProviderException(string? message)
            : base(message) { }

        public ServiceProviderException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// Exception for invalid message format with optional validation errors.
    /// </summary>
    public class MessageFormatException : SkillBuddyException
    {
        public IDictionary<string, List<string>> ValidationErrors { get; }

        public MessageFormatException(string? message, IDictionary<string, List<string>>? validationErrors = null)
            : base(message)
        {
            ValidationErrors = validationErrors ?? new Dictionary<string, List<string>>();
        }

        public MessageFormatException(string? message, Exception? innerException, IDictionary<string, List<string>>? validationErrors = null)
            : base(message, innerException)
        {
            ValidationErrors = validationErrors ?? new Dictionary<string, List<string>>();
        }
    }

    /// <summary>
    /// Exception for URL shortener failures.
    /// </summary>
    public class UrlShortenerException : SkillBuddyException
    {
        public UrlShortenerException(string? message)
            : base(message) { }

        public UrlShortenerException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// Exception for callback handling errors.
    /// </summary>
    public class CallbackException : SkillBuddyException
    {
        public CallbackException(string? message)
            : base(message) { }

        public CallbackException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// Exception for message sending failures.
    /// </summary>
    public class MessageSendException : SkillBuddyException
    {
        public MessageSendException(string? message)
            : base(message) { }

        public MessageSendException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }
}
