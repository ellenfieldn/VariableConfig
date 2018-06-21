// Copyright (c) Nathan Ellenfield. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Configuration;

namespace VariableConfig
{
    /// <summary>
    /// Represents data in an <see cref="IConfiguration"/> as an <see cref="IConfigurationSource"/>.
    /// </summary>
    public class VariableConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// The underlying configuration.
        /// </summary>
        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// Builds the <see cref="VariableConfigurationProvider"/> for this source.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
        /// <returns>A <see cref="VariableConfigurationProvider"/></returns>
        public IConfigurationProvider Build(IConfigurationBuilder builder) => new VariableConfigurationProvider(this);
    }
}
