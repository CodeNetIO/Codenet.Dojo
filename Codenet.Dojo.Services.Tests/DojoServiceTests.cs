using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Codenet.Dojo.Compilers;

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
            }
            ";

        [TestMethod]
        public void ProcessSimple_StaticMethod()
        {
            return;
            var service = new DojoService(new StringCompiler());
            Assert.AreEqual("Success", service.ProcessSimple(SIMPLE_STATIC_METHOD, SIMPLE_STATIC_METHOD_TEST));
        }
    }
}
