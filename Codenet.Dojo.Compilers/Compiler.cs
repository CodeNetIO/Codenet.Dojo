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
            return Compile(code, default(IEnumerable<Assembly>));
        }
        public Assembly Compile(string code, IEnumerable<Assembly> assemblyReferences)
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
            if (assemblyReferences != null)
            {
                foreach (var assembly in assemblyReferences)
                {
                    using (var stream = new MemoryStream())
                    {
                        var formatter = new BinaryFormatter();
                        formatter.Serialize(stream, assembly);
                        references.Add(MetadataReference.CreateFromImage(stream.ToArray()));
                    }
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
                    var sb = new StringBuilder();
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        sb.AppendFormat("{0}: {1}{2}", diagnostic.Id, diagnostic.GetMessage(), Environment.NewLine);
                    }

                    // Return errors through an exception
                    throw new InvalidDataException(sb.ToString());
                }
                else
                {
                    // Compiled successfully!  Create and return the assembly.
                    ms.Seek(0, SeekOrigin.Begin);
                    return Assembly.Load(ms.ToArray());
                }
            }
        }
    }
}
