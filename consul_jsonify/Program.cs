using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace consul_jsonify
{
    class Program
    {
        static void Main(string[] args)
        {
            string url;
            string key;
            do
            {
                Console.Write("Enter consul url: ");
                url = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(url))
                {
                    break;
                }
            } while (true);

            Console.Write("Enter config key: ");
            key = Console.ReadLine();

            var process = Process.Start(new ProcessStartInfo
            {
                Arguments = $"/C consul kv get -recurse -detailed -http-addr={url?.TrimEnd('/')} {key}",
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Normal,
                UseShellExecute = false,
                FileName = "cmd.exe",
                RedirectStandardOutput = true
            });

            var output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            var parser = new ConsulParser();
            var configs = parser.Parse(output);

            if (configs.Count > 0)
            {
                Console.WriteLine(output);
                Debug.WriteLine(output);

                Console.WriteLine("Saving output...");

                // remove duplicate key and convert to dictionary for better json output
                var jsonFormat = configs
                    .GroupBy(x => x.Key)
                    .Select(x => x.First())
                    .ToDictionary(c => c.Key, v => v.Value);

                var json = JsonSerializer.Serialize(jsonFormat, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });

                File.WriteAllText("consul.json", json);
            }
            else
            {
                Console.WriteLine("No configs found!");
            }

            Console.WriteLine("Done! Press any key to exit");
            Console.Read();
        }
    }
}
