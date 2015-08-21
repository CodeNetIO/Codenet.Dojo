using Codenet.Dojo.Compilers.Exceptions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Codenet.Dojo.Compilers
{
    public class StringCompiler : IStringCompiler
    {

        public Assembly Compile(string code)
        {
            return Compile(code, default(IEnumerable<byte[]>));
        }

        public Assembly Compile(string code, IEnumerable<byte[]> memoryStreamReferences)
        {
            var bytes = CompileToByteArray(code, memoryStreamReferences);
            // Compiled successfully!  Create and return the assembly.
            return Assembly.Load(bytes);
        }

        public byte[] CompileToByteArray(string code)
        {
            return CompileToByteArray(code, default(IEnumerable<byte[]>));
        }

        public byte[] CompileToByteArray(string code, IEnumerable<byte[]> memoryStreamReferences)
        {
            // Get the file path of object, since that's where the other .NET DLLs are
            // Should be something like c:\windows\microsoft.net\frameworks\v4.0....
            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);

            // Generate a syntax tree from the code
            var syntaxTree = CSharpSyntaxTree.ParseText(code);

            // A random assembly name.  We don't care... as long as it's unique
            string assemblyName = Path.GetRandomFileName();

            // Where should we look when a referenced assembly isn't loaded
            var references = new List<MetadataReference>()
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute).Assembly.Location)
            };

            // Add in the passed-in references
            if (memoryStreamReferences != null)
            {
                foreach (var ms in memoryStreamReferences)
                {
                    references.Add(MetadataReference.CreateFromImage(ms.ToArray()));
                }

            }

            // Create a compilation object to compile the code and make this a DLL.
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                // Go!
                var result = compilation.Emit(ms);

                if (!result.Success)
                {
                    // Had compile errors
                    var errors = new List<CompilationError>();
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        errors.Add(new CompilationError(diagnostic.Id, diagnostic.GetMessage(), diagnostic.Location.GetMappedLineSpan()));
                    }

                    // Return errors through an exception
                    throw new CompilationException(errors);
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    return ms.ToArray();
                }
            }
        }
    }
}
