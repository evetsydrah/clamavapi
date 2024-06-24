using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace ClamAvApi.Helper
{
    public static class ScanResultFormatter
    {
        public static string ExecuteCommand(string command, string args)
        {
            var processInfo = new ProcessStartInfo(command, args)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processInfo);
            if (process == null)
            {
                throw new InvalidOperationException("Failed to start process.");
            }

            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output;
        }

        public static Dictionary<string, string> ParseScanResult(string scanResult)
        {
            var result = new Dictionary<string, string>();
            var lines = scanResult.Split('\n');
            foreach (var line in lines)
            {
                if (line.Contains(":") && !line.Contains("SCAN SUMMARY") && !line.Contains("Scanned files") && !line.Contains("Infected files") && !line.Contains("Time") && !line.Contains("Start Date") && !line.Contains("End Date"))
                {
                    var parts = line.Split(new[] { ':' }, 2);
                    if (parts.Length == 2)
                    {
                        var filePath = parts[0].Trim();
                        var fileResult = parts[1].Trim();
                        result[filePath] = fileResult;
                    }
                }
            }
            return result;
        }

        public static Dictionary<string, string> ParseSummary(string scanResult)
        {
            var summary = new Dictionary<string, string>();
            var summaryIndex = scanResult.IndexOf("----------- SCAN SUMMARY -----------");
            if (summaryIndex >= 0)
            {
                var summaryLines = scanResult.Substring(summaryIndex).Split('\n');
                foreach (var line in summaryLines)
                {
                    if (line.Contains(":"))
                    {
                        var parts = line.Split(new[] { ':' }, 2);
                        if (parts.Length == 2)
                        {
                            var key = parts[0].Trim();
                            var value = parts[1].Trim();
                            summary[key] = value;
                        }
                    }
                }
            }

            Console.WriteLine("Parsed summary:");
            foreach (var kvp in summary)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
            }
            return summary;
        }
    }
}
