using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ClamAvApi.Models;
using ClamAvApi.Helper;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/scan", async (ScanRequest request) =>
{
    var scanResult = await ScanPath(request.Path, request.IsDirectory, true);
    return Results.Json(scanResult, new JsonSerializerOptions { WriteIndented = true });
});

if (args.Length > 0)
{
    string path = args[0];
    bool isDirectory = Directory.Exists(path);
    var scanResult = await ScanPath(path, isDirectory, false);
    Console.WriteLine(JsonSerializer.Serialize(scanResult, new JsonSerializerOptions { WriteIndented = true }));
}
else
{
    app.Run();
}

static async Task<ScanResult> ScanPath(string path, bool isDirectory, bool fromApi)
{
    if (string.IsNullOrWhiteSpace(path))
    {
        return new ScanResult { Errors = "A valid path is required.", ExitCode = 1 };
    }

    var fullPath = fromApi ? Path.GetFullPath(Path.Combine("/data", path)) : path;
    if (fromApi && !fullPath.StartsWith("/data"))
    {
        return new ScanResult { Errors = "Invalid path.", ExitCode = 1 };
    }

    string arguments = isDirectory ? $"-r {fullPath}" : fullPath;

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

    var output = outputBuilder.ToString();
    var errors = errorBuilder.ToString();

    var parsedResult = ClamAVParser.ParseClamAVOutput(output, errors, process.ExitCode);

    return parsedResult;
}
