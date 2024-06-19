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

app.MapPost("/scan-directory", async (ScanRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Directory))
    {
        return Results.BadRequest("A valid directory path is required.");
    }

    var fullPath = Path.GetFullPath(Path.Combine("/data", request.Directory));
    if (!fullPath.StartsWith("/data"))
    {
        return Results.BadRequest("Invalid directory path.");
    }

    var processInfo = new ProcessStartInfo
    {
        FileName = "clamscan",
        Arguments = $"-r {fullPath}",
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

    var jsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true
    };

    return Results.Json(parsedResult, jsonOptions);
});

app.Run();
