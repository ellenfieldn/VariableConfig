using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace VariableConfig.Test
{
    [TestClass, TestCategory("Unit Tests"), TestCategory("Acceptance Tests"), TestCategory("Escaping")]
    public class Escaping
    {
        public static IConfiguration Configuration { get; set; }

        public Escaping()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddXmlFile("escaping.config")
                .AddVariableConfiguration();
            Configuration = builder.Build();
        }

        [TestMethod]
        public void Test1()
        {
            Assert.AreEqual("asd${Z}asdf", Configuration["Test1"]);
        }

        [TestMethod]
        public void Test2()
        {
            Assert.AreEqual("asd$${Z}asd", Configuration["Test2"]);
        }

        [TestMethod]
        public void Test3()
        {
            Assert.AreEqual("asdf${zzzz}asd", Configuration["Test3"]);
        }

        [TestMethod]
        public void Test4()
        {
            Assert.AreEqual("asd$${zzzz}asd", Configuration["Test4"]);
        }

        [TestMethod]
        public void Test5()
        {
            Assert.AreEqual("asdf$zzzzasdf1", Configuration["Test5"]);
        }

        [TestMethod]
        public void Test6()
        {
            Assert.AreEqual(@"asd\${Z}asd", Configuration["Test6"]);
        }

        [TestMethod]
        public void Test7()
        {
            Assert.AreEqual(@"asd\${zzzz}asd", Configuration["Test7"]);
        }

        [TestMethod]
        public void Test8()
        {
            Assert.AreEqual(@"asd\$zzzzasd", Configuration["Test8"]);
        }

        [TestMethod]
        public void Test9()
        {
            Assert.AreEqual(@"asd\zzzzasd", Configuration["Test9"]);
        }

        [TestMethod]
        public void Test10()
        {
            Assert.AreEqual(@"asd\\zzzzasd", Configuration["Test10"]);
        }

        [TestMethod]
        public void Test11()
        {
            Assert.AreEqual(@"zzzz", Configuration["Test11"]);
        }

        [TestMethod]
        public void Test12()
        {
            Assert.AreEqual(@"${Z}", Configuration["Test12"]);
        }

        [TestMethod]
        public void Test13()
        {
            Assert.AreEqual(@"\zzzz", Configuration["Test13"]);
        }

        [TestMethod]
        public void Test14()
        {
            Assert.AreEqual(@"\\server\path", Configuration["Test14"]);
        }
    }
}
