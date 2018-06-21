// Copyright (c) Nathan Ellenfield. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace VariableConfig.Test
{
    [TestClass]
    public class VariableConfigurationExtensionsTests
    {
        [TestMethod]
        public void AutomaticallyBuildSourceConfig()
        {
            var autoBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddXmlFile("variables.config")
                .AddVariableConfiguration();
            var autoConfiguration = autoBuilder.Build();

            Assert.AreEqual("VarValueComp1", autoConfiguration["Composite"]);
        }

        [TestMethod]
        public void ManuallyBuildSourceConfig()
        {
            var sourceBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddXmlFile("variables.config");
            var sourceConfig = sourceBuilder.Build();
            var manualBuilder = new ConfigurationBuilder()
                .AddVariableConfiguration(sourceConfig);
            var manualConfiguration = manualBuilder.Build();

            Assert.AreEqual("VarValueComp1", manualConfiguration["Composite"]);
        }
    }
}
