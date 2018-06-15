using Microsoft.Extensions.Configuration;
using System;

namespace VariableConfig
{
    public static class VariableConfigurationExtensions
    {
        public static IConfigurationBuilder AddVariableConfiguration(this IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder = configurationBuilder ?? throw new ArgumentException(nameof(configurationBuilder));
            var config = configurationBuilder.Build();
            return AddVariableConfiguration(configurationBuilder, config);
        }

        public static IConfigurationBuilder AddVariableConfiguration(this IConfigurationBuilder configurationBuilder, IConfiguration config)
        {
            configurationBuilder = configurationBuilder ?? throw new ArgumentException(nameof(configurationBuilder));
            config = config ?? throw new ArgumentException(nameof(config));
            configurationBuilder.Add(new VariableConfigurationSource { Configuration = config });
            return configurationBuilder;
        }
    }
}
