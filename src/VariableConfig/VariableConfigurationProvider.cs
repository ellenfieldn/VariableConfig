using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace VariableConfig
{
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

        public VariableConfigurationProvider(VariableConfigurationSource source)
        {
            source = source ?? throw new ArgumentNullException(nameof(source));
            _config = source.Configuration ?? throw new ArgumentNullException(nameof(source.Configuration));
        }

        public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath)
        {
            var section = parentPath == null ? _config : _config.GetSection(parentPath);
            var children = section.GetChildren();
            return children.Select(c => c.Key)
                .Concat(earlierKeys)
                .OrderBy(k => k, ConfigurationKeyComparer.Instance);
        }

        public IChangeToken GetReloadToken() => _config.GetReloadToken();

        public void Load() { }

        public void Set(string key, string value) => _config[key] = value;

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

        private bool ReplaceVariable(ref string after)
        {
            var getResult = true;
            after = Regex.Replace(after, _regexThatMatchesVariables, match =>
            {
                var variableGroup = match.Groups["variableName"];
                if (!variableGroup.Success)
                {
                    return match.Groups[0].Value;
                }
                getResult = TryGet(variableGroup.Value, out string innerMatchResult);
                return match.Groups.Where(g => g.Name != "0" && !string.IsNullOrEmpty(g.Value))
                    .Select(g => (g.Name == "variableName") ? innerMatchResult : Regex.Replace(g.Value, @"\\\\", @"\")) //Unescape '\\' as '\'
                    .Aggregate((first, second) => first + second);
            });
            after = Regex.Replace(after, @"\\\${", @"${"); //Unescape '\${' as '${'
            after = Regex.Replace(after, @"\\\\\$", @"\$"); //Unescape leftover double backslashes that precede a $
            return getResult;
        }
    }
}
