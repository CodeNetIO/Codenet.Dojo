using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Codenet.Dojo.Compilers.Tests
{
    [TestClass]
    public class StringCompilerTests
    {
        private static string SIMPLE_STATIC_METHOD = @"
            using System;

            public static class SimpleStatic
            {
                public static string GetAString()
                {
                    return ""A String"";
                }
            }
            ";

        [TestMethod]
        public void StringCompiler_SimpleStaticMethod()
        {
            var stringCompiler = new StringCompiler();
            var assembly = stringCompiler.Compile(SIMPLE_STATIC_METHOD);
            Assert.IsNotNull(assembly);

            var exportedType = assembly.ExportedTypes.FirstOrDefault();
            Assert.IsNotNull(exportedType);
            Assert.AreEqual("SimpleStatic", exportedType.Name);

            var getAStringMethod = exportedType.GetMethod("GetAString");
            Assert.IsNotNull(getAStringMethod);
            Assert.AreEqual("A String", getAStringMethod.Invoke(null,null));
        }
    }
}
