using Microsoft.CodeAnalysis;

namespace Codenet.Dojo.Compilers.Exceptions
{
    /// <summary>
    /// Represents a compilation error
    /// </summary>
    public class CompilationError
    {
        /// <summary>
        /// Creates an instance of a CompilationError
        /// </summary>
        /// <param name="id">The error Id</param>
        /// <param name="message">The error message</param>
        /// <param name="location">The location of the error in the source</param>
        public CompilationError(string id, string message, FileLinePositionSpan location)
        {
            Id = id;
            Message = message;
            Location = location;
        }

        /// <summary>
        /// The error Id
        /// </summary>
        public string Id { get; private set; }
        /// <summary>
        /// The error message
        /// </summary>
        public string Message { get; private set; }
        /// <summary>
        /// The location of the error in the source
        /// </summary>
        public FileLinePositionSpan Location { get; private set; }
    }
}
