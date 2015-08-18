using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Codenet.Dojo.Compilers.Tests
{
    [TestClass]
    public class StringCompilerTests
    {
        #region Fields
        private Dictionary<string, Assembly> _assemblies;
        #endregion

        #region Compilation Objects

        #region SIMPLE_STATIC_METHOD
        private const string SIMPLE_STATIC_METHOD = @"
            using System;

            public static class SimpleStatic
            {
                public static string GetAString()
                {
                    return ""A String"";
                }
            }
            ";
        #endregion

        #region SIMPLE_STATIC_FAILURE_METHOD
        private const string SIMPLE_STATIC_FAILURE_METHOD = @"
            using System;

            public static class SimpleStatic
            {
                public static string GetAString()
                {
                    return ""A Failure"";
                }
            }
            ";
        #endregion

        #region SIMPLE_NONSTATIC_METHOD
        private const string SIMPLE_NONSTATIC_METHOD = @"
            using System;

            public class SimpleNonStatic
            {
                private string _theString;

                public SimpleNonStatic(string theString)
                {
                    _theString = theString;
                }

                public string GetAString()
                {
                    return _theString;
                }

                public string GetAString(string whatever)
                {
                    return whatever;
                }
            }
            ";

        #endregion

        #region SIMPLE_STATIC_METHOD_TEST
        private const string SIMPLE_STATIC_METHOD_TEST = @"
            using System;
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            [TestClass]
            public class SimpleStaticTests
            {
                [TestMethod]
                public void SimpleStatic_GetAString()
                {
                    Assert.AreEqual(""A String"", SimpleStatic.GetAString());
                }
            }
            ";
        #endregion

        #endregion

        #region Constructors
        public StringCompilerTests()
        {
            _assemblies = new Dictionary<string, Assembly>();
        }
        #endregion

        [TestMethod]
        public void SimpleStaticMethod()
        {
            // Create assembly
            var stringCompiler = new StringCompiler();
            var assembly = stringCompiler.Compile(SIMPLE_STATIC_METHOD);
            Assert.IsNotNull(assembly);

            // Verify that the class exists
            var exportedType = assembly.ExportedTypes.FirstOrDefault();
            Assert.IsNotNull(exportedType);
            Assert.AreEqual("SimpleStatic", exportedType.Name);

            // Verify that the method exists & returns the appropriate response
            var getAStringMethod = exportedType.GetMethod("GetAString");
            Assert.IsNotNull(getAStringMethod);
            Assert.AreEqual("A String", getAStringMethod.Invoke(null,null));
        }

        [TestMethod]
        public void SimpleNonStaticMethod()
        {
            // Create assembly
            var stringCompiler = new StringCompiler();
            var assembly = stringCompiler.Compile(SIMPLE_NONSTATIC_METHOD);
            Assert.IsNotNull(assembly);

            // Verify that the class exists
            var exportedType = assembly.ExportedTypes.FirstOrDefault();
            Assert.IsNotNull(exportedType);
            Assert.AreEqual("SimpleNonStatic", exportedType.Name);

            // Get the contructor info and call it
            var constructor = exportedType.GetConstructors().FirstOrDefault();
            Assert.IsNotNull(constructor);
            var instance = constructor.Invoke(new object[] {"My String!"});
            Assert.IsNotNull(instance);

            // Verify that the method exists & returns the injected string
            var constructorStringMethod = exportedType.GetMethods().FirstOrDefault(m => m.Name == "GetAString" && !m.GetParameters().Any());
            Assert.IsNotNull(constructorStringMethod);
            Assert.AreEqual("My String!", constructorStringMethod.Invoke(instance, null));

            // Verify that the overloaded method works as well
            var parameterStringMethod = exportedType.GetMethods().FirstOrDefault(m => m.Name == "GetAString" && m.GetParameters().Count() == 1);
            Assert.IsNotNull(parameterStringMethod);
            Assert.AreEqual("Injected String!", parameterStringMethod.Invoke(instance, new object[] {"Injected String!"}));
        }

        [TestMethod]
        public void SimpleStaticMethodUnitTest()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            // Create class assembly
            var stringCompiler = new StringCompiler();
            var bytes = stringCompiler.CompileToByteArray(SIMPLE_STATIC_METHOD);
            Assert.IsNotNull(bytes);

            var codeAssembly = Assembly.Load(bytes);
            _assemblies[codeAssembly.FullName] = codeAssembly;

            var testAssembly = stringCompiler.Compile(SIMPLE_STATIC_METHOD_TEST, new [] { bytes });
            Assert.IsNotNull(testAssembly);

            // Verify that the class exists
            var exportedType = testAssembly.ExportedTypes.FirstOrDefault();
            Assert.IsNotNull(exportedType);
            Assert.AreEqual("SimpleStaticTests", exportedType.Name);

            // Get the contructor info and call it
            var constructor = exportedType.GetConstructors().FirstOrDefault();
            Assert.IsNotNull(constructor);
            var instance = constructor.Invoke(new object[] {});
            Assert.IsNotNull(instance);

            // Verify that the method exists & returns the appropriate response
            var getAStringMethod = exportedType.GetMethod("SimpleStatic_GetAString");
            Assert.IsNotNull(getAStringMethod);
            getAStringMethod.Invoke(instance, null);
        }

        [TestMethod]
        public void SimpleStaticFailureMethod()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            // Create class assembly
            var stringCompiler = new StringCompiler();
            var bytes = stringCompiler.CompileToByteArray(SIMPLE_STATIC_FAILURE_METHOD);
            Assert.IsNotNull(bytes);

            var codeAssembly = Assembly.Load(bytes);
            _assemblies[codeAssembly.FullName] = codeAssembly;

            var testAssembly = stringCompiler.Compile(SIMPLE_STATIC_METHOD_TEST, new[] { bytes });
            Assert.IsNotNull(testAssembly);

            // Verify that the class exists
            var exportedType = testAssembly.ExportedTypes.FirstOrDefault();
            Assert.IsNotNull(exportedType);
            Assert.AreEqual("SimpleStaticTests", exportedType.Name);

            // Get the contructor info and call it
            var constructor = exportedType.GetConstructors().FirstOrDefault();
            Assert.IsNotNull(constructor);
            var instance = constructor.Invoke(new object[] { });
            Assert.IsNotNull(instance);

            // Verify that the method exists & returns the appropriate response
            var getAStringMethod = exportedType.GetMethod("SimpleStatic_GetAString");
            Assert.IsNotNull(getAStringMethod);
            try
            {
                getAStringMethod.Invoke(instance, null);
            }
            catch(Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                Assert.AreEqual("Assert.AreEqual failed. Expected:<A String>. Actual:<A Failure>. ", ex.Message);
            }
        }

        #region Event Handlers
        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly result;
            _assemblies.TryGetValue(args.Name, out result);
            return result;
        }
        #endregion
    }
}
