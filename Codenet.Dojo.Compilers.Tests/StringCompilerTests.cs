using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Codenet.Dojo.Compilers.Tests
{
    [TestClass]
    public class StringCompilerTests
    {
        #region Objects
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

        [TestMethod]
        public void StringCompiler_SimpleStaticMethod()
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
        public void StringCompiler_SimpleNonStaticMethod()
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
    }
}
