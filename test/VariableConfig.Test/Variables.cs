using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace VariableConfig.Test
{
    [TestClass, TestCategory("Unit Tests"), TestCategory("Acceptance Tests"), TestCategory("Variables")]
    public class Variables
    {
        public static IConfiguration Configuration { get; set; }

        public Variables()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddXmlFile("variables.config")
                .AddVariableConfiguration();
            Configuration = builder.Build();
        }

        /// <summary>
        /// Retrieve a normal app setting with no variables.
        /// </summary>
        [TestMethod]
        public void NormalSetting()
        {
            Assert.AreEqual("Value1", Configuration["NormalSetting"]);
        }

        /// <summary>
        /// Retrieve an app setting that is used as a variable.
        /// </summary>
        [TestMethod]
        public void VariableSetting()
        {
            Assert.AreEqual("VarValue", Configuration["Variable"]);
        }

        /// <summary>
        /// Retrieve an app setting that uses a variable.
        /// </summary>
        [TestMethod]
        public void CompositeSetting()
        {
            Assert.AreEqual("VarValueComp1", Configuration["Composite"]);
        }

        /// <summary>
        /// Retrieve an app setting that has multiple variables and is recursive
        /// </summary>
        [TestMethod]
        public void SuperCompositeSetting()
        {
            Assert.AreEqual("VarValueComp1ContainsVarValue", Configuration["SuperComposite"]);
        }

        /// <summary>
        /// Retrieve an app setting that as a property on a complex object
        /// </summary>
        [TestMethod]
        public void ComplexObject()
        {
            Assert.AreEqual("VarValueInProperty", Configuration["ComplexObject:PropertyOnObject"]);
        }

        /// <summary>
        /// Verify that you can reference properties from complex objects
        /// </summary>
        [TestMethod]
        public void NestedComplexObject()
        {
            Assert.AreEqual("PropertyIsVarValueInProperty!", Configuration["NestedComplexObject"]);
        }

        /// <summary>
        /// Retrieve an app setting that as a property on a complex object that was automatically deserialized
        /// </summary>
        [TestMethod]
        public void DeserializedComplexObject()
        {
            Assert.Inconclusive("Not Implemented");
        }

        /// <summary>
        /// Attempt to retrieve an app setting that does not exist
        /// </summary>
        [TestMethod]
        public void MissingKey()
        {
            Assert.IsNull(Configuration["MissingKey"]);
        }

        /// <summary>
        /// Attempt to retrieve an app setting that uses a variable that doesn't exist.
        /// </summary>
        [TestMethod]
        public void MissingVariable()
        {
            Assert.AreEqual("${MissingVar}asdf", Configuration["VariableMissing"]);
        }
    }
}
