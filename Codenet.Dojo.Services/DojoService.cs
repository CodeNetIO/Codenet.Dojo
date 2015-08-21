using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Codenet.Dojo.Contracts;
using Codenet.Dojo.Compilers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Codenet.Dojo.Compilers.Exceptions;

namespace Codenet.Dojo.Services
{
    /// <summary>
    /// A compilation and processing service for the Codenet Dojo
    /// </summary>
    public class DojoService : IDojoService
    {
        private IStringCompiler _stringCompiler;
        private Dictionary<string, Assembly> _assemblies;
        public DojoService(IStringCompiler stringCompiler)
        {
            _stringCompiler = stringCompiler;
            _assemblies = new Dictionary<string, Assembly>();
            
        }

        /// <summary>
        /// Performs a simple compilation and unit test operation
        /// </summary>
        /// <param name="code">The code to compile.</param>
        /// <param name="tests">The unit tests to compile and run.</param>
        /// <returns>An IEnumerable of MessageResult indicating the result of the compilation and tests.</returns>
        public IEnumerable<MessageResult> ProcessSimple(string code, string tests)
        {
            var result = new List<MessageResult>();

            try
            {
                // See if the code will compile and see how long it takes.
                var codeBytes = _stringCompiler.CompileToByteArray(code);
                var codeAssembly = Assembly.Load(codeBytes);

                _assemblies[codeAssembly.FullName] = codeAssembly;
                var testAssembly = _stringCompiler.Compile(tests, new[] { codeBytes });

                var exportedType = testAssembly.ExportedTypes.FirstOrDefault();
                var constructor = exportedType.GetConstructors().FirstOrDefault(c => !c.GetParameters().Any());
                var instance = constructor.Invoke(new object[] { });

                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                foreach (var type in testAssembly.GetTypes())
                {
                    foreach (var method in type.GetMethods().Where(m => m.GetCustomAttributes().Any(a => a.GetType() == typeof(TestMethodAttribute))))
                    {
                        method.Invoke(instance, null);
                    }
                }
                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
                result.Add(new CompilationResult(true)
                {
                    Message = "Completed Successfully!"
                });
            }
            catch (CompilationException ex)
            {
                // Add a compilation result, since it failed.
                result.Add(new CompilationResult(ex));
            }
            
            return result;
        }

        #region Private Event Handlers
        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly result;
            _assemblies.TryGetValue(args.Name, out result);
            return result;
        }

        #endregion
    }
}
