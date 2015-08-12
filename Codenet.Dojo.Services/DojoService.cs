using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Codenet.Dojo.Contracts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Codenet.Dojo.Services
{
    public class DojoService : IDojoService
    {
        public string ProcessSimple(string code, string tests)
        {
            var assemblyPath = Path.GetDirectoryName(typeof (object).Assembly.Location);
            if (assemblyPath == null)
            {
                // do something...
                return "Assembly Path is null";
            }

            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            IEnumerable<SyntaxTree> syntaxTrees = new List<SyntaxTree>() {syntaxTree};
            var compilation = CSharpCompilation.Create("temp.dll")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "mscorlib.dll")))
                .AddReferences(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.dll")))
                .AddReferences(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Core.dll")))
                .AddReferences(MetadataReference.CreateFromFile(Assembly.GetEntryAssembly().Location))
                .AddSyntaxTrees(syntaxTree);
            return "";
        }
    }
}
