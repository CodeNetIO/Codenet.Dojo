using System.Reflection;

namespace Codenet.Dojo.Compilers
{
    /// <summary>
    /// Represents a String Compiler
    /// </summary>
    public interface IStringCompiler
    {
        /// <summary>
        /// Compiles the string into an assembly.
        /// </summary>
        /// <param name="code">The code to compile.</param>
        /// <exception cref="InvalidDataException">
        /// Code could not be compiled.
        /// </exception>
        /// <returns>An assembly of the code string.</returns>
        Assembly Compile(string code);
    }
}
