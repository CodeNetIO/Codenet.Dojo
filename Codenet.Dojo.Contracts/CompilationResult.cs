using Codenet.Dojo.Compilers.Exceptions;
using System.Runtime.Serialization;


namespace Codenet.Dojo.Contracts
{
    /// <summary>
    /// Represents a compilation result
    /// </summary>
    [DataContract]
    public class CompilationResult : MessageResult
    {
        /// <summary>
        /// Creates an instance of CompilationResult
        /// </summary>
        public CompilationResult()
        { }

        /// <summary>
        /// Creates an instance of CompilationResult from a CompilationError
        /// </summary>
        /// <param name="error"></param>
        public CompilationResult(CompilationException exception)
        {
            CompilationSuccessful = false;
            foreach (var error in exception.Errors)
            {
                Message = string.Format("{0} {1}", error.Id, error.Message);
                Details.Add(string.Format("Starting at Line {0}, Column {1}",
                    error.Location.StartLinePosition.Line, error.Location.StartLinePosition.Character));
            }
        }

        /// <summary>
        /// Creates an instance of a CompilationResult indicating its success.
        /// </summary>
        /// <param name="successful">Indicates whether compilation was successful.</param>
        public CompilationResult(bool successful)
        {
            CompilationSuccessful = successful;
        }
        /// <summary>
        /// Indicates whether or not the compilation attempt was successful.
        /// </summary>
        [DataMember]
        public bool CompilationSuccessful { get; private set; }
    }
}
