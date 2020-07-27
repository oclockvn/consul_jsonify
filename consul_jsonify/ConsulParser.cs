using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace consul_jsonify
{
    public class ConsulParser
    {
        public List<ConsulKv> Parse(string config)
        {
            if (string.IsNullOrWhiteSpace(config))
            {
                return new List<ConsulKv>();
            }

            var lines = config.Split("\n");
            if (lines == null || lines.Length == 0)
            {
                return new List<ConsulKv>();
            }

            var kvLines = lines.Where(line => line.StartsWith("Key ") || line.StartsWith("Value ")).ToList();
            var lineCount = kvLines.Count;
            var configs = new List<ConsulKv>();

            var keyPattern = new Regex(@"^Key\s*(\S*)$");
            var valuePattern = new Regex(@"^Value\s*(\S*)$");

            for (int i = 0; i < lineCount - 1; i += 2)
            {
                var key = keyPattern.Match(kvLines[i]);
                var value = valuePattern.Match(kvLines[i + 1]);

                if (!key.Success)
                {
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
