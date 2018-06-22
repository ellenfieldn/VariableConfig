// Copyright (c) Nathan Ellenfield. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace VariableConfig.Test.json
{
    [TestClass, TestCategory("Unit Tests"), TestCategory("Acceptance Tests"), TestCategory("Escaping")]
    public class JsonEscaping : Escaping
    {
        public static new IConfiguration Configuration { get; set; }

        public JsonEscaping()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("json/escaping.json")
                .AddVariableConfiguration();
            Configuration = builder.Build();
        }
    }
}
