using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

public class ClamAVBenchmark
{
    private const string DirectoryPath = "/home/kevinwu/spikes/ClamAVApi/mountFiles";

    [Benchmark]
    public async Task ScanEntireDirectory()
    {
        string arguments = $"-r {DirectoryPath}";

        var processInfo = new ProcessStartInfo
        {
            FileName = "clamscan",
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = processInfo };
        process.Start();

        var outputBuilder = new StringBuilder();
        var errorBuilder = new StringBuilder();
        using (var outputReader = process.StandardOutput)
        using (var errorReader = process.StandardError)
        {
            while (!outputReader.EndOfStream)
            {
                outputBuilder.AppendLine(await outputReader.ReadLineAsync());
            }

            while (!errorReader.EndOfStream)
            {
                errorBuilder.AppendLine(await errorReader.ReadLineAsync());
            }
        }

        await process.WaitForExitAsync();
    }

    [Benchmark]
    public async Task ScanEachFileIndividually()
    {
        var files = Directory.GetFiles(DirectoryPath, "*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            string arguments = file;

            var processInfo = new ProcessStartInfo
            {
                FileName = "clamscan",
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = new Process { StartInfo = processInfo };
            process.Start();

            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();
            using (var outputReader = process.StandardOutput)
            using (var errorReader = process.StandardError)
            {
                while (!outputReader.EndOfStream)
                {
                    outputBuilder.AppendLine(await outputReader.ReadLineAsync());
                }

                while (!errorReader.EndOfStream)
                {
                    errorBuilder.AppendLine(await errorReader.ReadLineAsync());
                }
            }

            await process.WaitForExitAsync();
        }
    }
}
