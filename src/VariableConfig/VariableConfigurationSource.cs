using Microsoft.Extensions.Configuration;
using System;

namespace VariableConfig
{
    public class VariableConfigurationSource : IConfigurationSource
    {
        public IConfiguration Configuration { get; set; }

        public IConfigurationProvider Build(IConfigurationBuilder builder) => new VariableConfigurationProvider(this);
    }
}
