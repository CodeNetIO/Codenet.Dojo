using System.Collections.Generic;
using System.Runtime.Serialization;


namespace Codenet.Dojo.Contracts
{
    /// <summary>
    /// Represents a message result
    /// </summary>
    [DataContract]
    public class MessageResult
    {
        /// <summary>
        /// Creates an instance of MessageResult
        /// </summary>
        public MessageResult()
        {
            Details = new List<string>();
        }

        /// <summary>
        /// Creates an instance of MessageResult with the specified message detail
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="details">The message details</param>
        public MessageResult(string message, IList<string> details)
        {
            Message = message;
            Details = details;
        }
        /// <summary>
        /// The result message
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// Detailed information about the message, if available
        /// </summary>
        [DataMember]
        public IList<string>Details { get; set; }
    }
}
