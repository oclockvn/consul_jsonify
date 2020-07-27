using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace consul_jsonify
{
    public class ConsulParser
    {
        public List<ConsulKv> Parse(string config, out int lineCount)
        {
            lineCount = 0;
            if (string.IsNullOrWhiteSpace(config))
            {
                return new List<ConsulKv>();
            }

            var lines = config.Split("\n");
            if (lines == null || lines.Length == 0)
            {
                return new List<ConsulKv>();
            }

            var kvLines = lines.Where(line => line.StartsWith("Key") || line.StartsWith("Value")).ToList();
            lineCount = kvLines.Count;
            var configs = new List<ConsulKv>();

            Debug.WriteLine($"Total {lineCount} key/value lines / {lines.Length} total lines");

            var keyPattern = new Regex(@"^Key\s*(\S*)$");
            var valuePattern = new Regex(@"^Value\s*(\S*)$");

            for (int i = 0; i < lineCount - 1; i += 2)
            {
                var keyConfig = kvLines[i];
                var valueConfig = kvLines[i + 1];
                var key = keyPattern.Match(keyConfig);
                var value = valuePattern.Match(valueConfig);

                if (!key.Success)
                {
                    Debug.WriteLine($"Line {keyConfig} has no valid key");
                    continue;
                }

                var k = key.Groups[1].Value;
                var v = value.Success ? value.Groups[1].Value : string.Empty;

                Debug.WriteLine($"{k}: {v}");

                configs.Add(new ConsulKv(k, v));
            }

            return configs;
        }
    }
}
