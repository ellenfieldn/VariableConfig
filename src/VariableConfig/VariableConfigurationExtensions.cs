// Copyright (c) Nathan Ellenfield. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using VariableConfig;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// IConfigurationBuilder extension methods for the VariableConfigurationProvider.
    /// </summary>
    public static class VariableConfigurationExtensions
    {
        /// <summary>
        /// Adds the VariableConfigurationProvider to <paramref name="configurationBuilder"/>.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder" /> to add to.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVariableConfiguration(this IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder = configurationBuilder ?? throw new ArgumentException(nameof(configurationBuilder));
            var config = configurationBuilder.Build();
            return AddVariableConfiguration(configurationBuilder, config);
        }

        /// <summary>
        /// Adds the VariableConfigurationProvider to <paramref name="configurationBuilder"/>.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="config">The source <see cref="IConfiguration" /> to pull data from.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddVariableConfiguration(this IConfigurationBuilder configurationBuilder, IConfiguration config)
        {
            configurationBuilder = configurationBuilder ?? throw new ArgumentException(nameof(configurationBuilder));
            config = config ?? throw new ArgumentException(nameof(config));
            configurationBuilder.Add(new VariableConfigurationSource { Configuration = config });
            return configurationBuilder;
        }
    }
}
