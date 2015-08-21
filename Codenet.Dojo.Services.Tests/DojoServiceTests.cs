using Codenet.Dojo.Compilers;
using Codenet.Dojo.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Codenet.Dojo.Services.Tests
{
    [TestClass]
    public class DojoServiceTests
    {

        private static string SIMPLE_STATIC_METHOD = @"
            using System;

            public static class SimpleStatic
            {
                public static string GetAString()
                {
                    return ""A String"";
                }

                public static string SetAndGet(string theString)
                {
                    return theString;
                }
            }
            ";

        private static string SIMPLE_STATIC_METHOD_TEST = @"
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

                [TestMethod]
                public void SimpleStatic_SetAndGet()
                {
                    Assert.AreEqual(""Oh Yeah! - Koolaid Guy"", SimpleStatic.SetAndGet(""Oh Yeah! - Koolaid Guy""));
                }
            }
            ";

        [TestMethod]
        public void ProcessSimple_StaticMethod()
        {
            var service = new DojoService(new StringCompiler());
            var results = service.ProcessSimple(SIMPLE_STATIC_METHOD, SIMPLE_STATIC_METHOD_TEST).ToList();
            Assert.AreEqual(1,results.Count);
            var result = (results[0] as CompilationResult);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.CompilationSuccessful);
            Assert.AreEqual("Completed Successfully!", result.Message);
        }
    }
}
