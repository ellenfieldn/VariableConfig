using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace VariableConfig.Test.json
{
    [TestClass, TestCategory("Unit Tests"), TestCategory("Acceptance Tests"), TestCategory("Variables")]
    public class JsonVariables : Variables
    {
        public static new IConfiguration Configuration { get; set; }

        public JsonVariables()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("json/variables.json")
                .AddVariableConfiguration();
            Configuration = builder.Build();
        }
    }
}
