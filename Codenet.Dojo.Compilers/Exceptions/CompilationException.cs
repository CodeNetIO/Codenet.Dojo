using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codenet.Dojo.Compilers.Exceptions
{
    /// <summary>
    /// A compilation exception
    /// </summary>
    public class CompilationException : Exception
    {
        /// <summary>
        /// Creates an instance of a Compilation Exception
        /// </summary>
        /// <param name="errors"></param>
        public CompilationException(IEnumerable<CompilationError> errors)
        {
            Errors = errors;
        }

        /// <summary>
        /// Compilations errors responsible for the exception
        /// </summary>
        public IEnumerable<CompilationError> Errors { get; private set; }
    }
}
