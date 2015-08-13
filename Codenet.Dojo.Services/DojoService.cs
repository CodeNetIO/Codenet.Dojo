using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Codenet.Dojo.Contracts;
using Codenet.Dojo.Compilers;

namespace Codenet.Dojo.Services
{
    public class DojoService : IDojoService
    {
        private IStringCompiler _stringCompiler;
        public DojoService(IStringCompiler stringCompiler)
        {
            _stringCompiler = stringCompiler;
        }

        public string ProcessSimple(string code, string tests)
        {
            // Code builds fine.  Tests don't.
            // Look into roslyn workspaces and see if we can create a unit test solution
            //   to load up into a workspace.  If we can do that, then add code to that
            //   workspace, that may solve our dependency problems.  Otherwise, we need
            //   to resolve the dependencies for the UnitTesting framework.
            var codeAssembly = _stringCompiler.Compile(code);
            var testAssembly = _stringCompiler.Compile(tests);

            return "Success";
        }
    }
}
