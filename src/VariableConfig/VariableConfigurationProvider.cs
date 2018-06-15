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
            bool getResult = true;
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            value = Regex.Replace(value, @"([^\\]|^)\\(?>(\\\\)*)\$|\${(?<variableName>.*?)}", match =>
            {
                if(!match.Groups["variableName"].Success)
                {
                    return match.Groups[0].Value;
                }
                getResult &= TryGet(match.Groups["variableName"].Value, out string innerMatchResult);
                return innerMatchResult;
            });
            value = Regex.Replace(value, @"\\\\", @"\");
            value = Regex.Replace(value, @"\\\${", @"${");
            return !string.IsNullOrEmpty(value) && getResult;
        }
    }
}
