// Copyright (c) Nathan Ellenfield. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace VariableConfig
{
    /// <summary>
    /// Implementation of <see cref="IConfigurationProvider"/> that supports configuration variables.
    /// </summary>
    public class VariableConfigurationProvider : IConfigurationProvider
    {
        private readonly IConfiguration _config;

        /*
         * If we have an odd number of backslashes directly before ${...}, then we know the $ is escaped
         * '(?<=[^\\]|^)'              Find a character to anchor to -- either the start of the string or a non-backslash
         * ((?:\\\\)*)                 Grab any number of '\\' that precede a '$'. This will only match if we have an even number.
         * \$\{(?<variableName>.*?)\}  Anything of the form ${...} is our variable name.
         */
        private static readonly string _regexThatMatchesVariables = @"(?<=[^\\]|^)((?:\\\\)*)\$\{(?<variableName>.*?)\}";

        /// <summary>
        /// Initializes a new instance from the source configuration.
        /// </summary>
        /// <param name="source">The source configuration.</param>
        public VariableConfigurationProvider(VariableConfigurationSource source)
        {
            source = source ?? throw new ArgumentNullException(nameof(source));
            _config = source.Configuration ?? throw new ArgumentNullException(nameof(source.Configuration));
        }

        /// <summary>
        /// Returns the immediate descendant configuration keys for a given parent path based on this
        /// <see cref="IConfigurationProvider"/>'s data and the set of keys returned by all the preceding
        /// <see cref="IConfigurationProvider"/>s.
        /// </summary>
        /// <param name="earlierKeys">The child keys returned by the preceding providers for the same parent path.</param>
        /// <param name="parentPath">The parent path.</param>
        /// <returns>The child keys.</returns>
        public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath)
        {
            var section = parentPath == null ? _config : _config.GetSection(parentPath);
            var children = section.GetChildren();
            return children.Select(c => c.Key)
                .Concat(earlierKeys)
                .OrderBy(k => k, ConfigurationKeyComparer.Instance);
        }

        /// <summary>
        /// Returns a change token if this provider supports change tracking, null otherwise.
        /// </summary>
        /// <returns>The <see cref="IChangeToken"/></returns>
        public IChangeToken GetReloadToken() => _config.GetReloadToken();

        /// <summary>
        /// Loads configuration values from the source represented by this <see cref="IConfigurationProvider"/>.
        /// </summary>
        public void Load() { }

        /// <summary>
        /// Sets a configuration value for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Set(string key, string value) => _config[key] = value;

        /// <summary>
        /// Tries to get a configuration value for the specified key and fills in any variables.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>True</c> if a value for the specified key was found, otherwise <c>false</c>.</returns>
        public bool TryGet(string key, out string value)
        {
            value = _config[key];
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            var getResult = ReplaceVariable(ref value);
            return !string.IsNullOrEmpty(value) && getResult;
        }

        private bool ReplaceVariable(ref string value)
        {
            var getResult = true;
            value = Regex.Replace(value, _regexThatMatchesVariables, match =>
            {
                var variableGroup = match.Groups["variableName"];
                if (!variableGroup.Success)
                {
                    return match.Groups[0].Value;
                }
                getResult = TryGet(variableGroup.Value, out string innerMatchResult);
                return match.Groups.Cast<Group>()
                    .Where(g => g.Name != "0" && !string.IsNullOrEmpty(g.Value))
                    .Select(g => (g.Name == "variableName") ? innerMatchResult : Regex.Replace(g.Value, @"\\\\", @"\")) //Unescape '\\' as '\'
                    .Aggregate((first, second) => first + second);
            });
            value = Regex.Replace(value, @"\\\${", @"${"); //Unescape '\${' as '${'
            value = Regex.Replace(value, @"\\\\\$", @"\$"); //Unescape leftover double backslashes that precede a $
            return getResult;
        }
    }
}
