using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Codenet.Dojo.Contracts;
using Codenet.Dojo.Compilers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Codenet.Dojo.Services
{
    public class DojoService : IDojoService
    {
        private IStringCompiler _stringCompiler;
        private Dictionary<string, Assembly> _assemblies;
        public DojoService(IStringCompiler stringCompiler)
        {
            _stringCompiler = stringCompiler;
            _assemblies = new Dictionary<string, Assembly>();
            
        }

        public string ProcessSimple(string code, string tests)
        {
            var codeBytes= _stringCompiler.CompileToByteArray(code);
            var codeAssembly = Assembly.Load(codeBytes);
            _assemblies[codeAssembly.FullName] = codeAssembly;
            var testAssembly = _stringCompiler.Compile(tests, new [] { codeBytes});

            var exportedType = testAssembly.ExportedTypes.FirstOrDefault();
            var constructor = exportedType.GetConstructors().FirstOrDefault(c=>!c.GetParameters().Any());
            var instance = constructor.Invoke(new object[] { });

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            foreach (var type in testAssembly.GetTypes())
            {
                foreach(var method in type.GetMethods().Where(m=>m.GetCustomAttributes().Any(a=>a.GetType() == typeof(TestMethodAttribute))))
                {
                    method.Invoke(instance, null);
                }
            }
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;

            return "Success";
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
