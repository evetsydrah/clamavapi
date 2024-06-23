using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/scan", (string filePath) =>
{
    var processInfo = new ProcessStartInfo("clamdscan", filePath)
    {
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    using var process = Process.Start(processInfo);
    if (process == null) return Results.Problem("Failed to start clamdscan process.");

    var output = process.StandardOutput.ReadToEnd();
    process.WaitForExit();

    return Results.Ok(output);
});

app.Run();
