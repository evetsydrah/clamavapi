using BenchmarkDotNet.Running;
using ClamAvApi.Helper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

var commandLineArgs = Environment.GetCommandLineArgs();

if (commandLineArgs.Contains("--benchmark"))
{
    var summary = BenchmarkRunner.Run<ClamAVBenchmark>();
    return;
}

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ApplicationName = typeof(Program).Assembly.FullName,
    ContentRootPath = AppContext.BaseDirectory,
    WebRootPath = "wwwroot",
    EnvironmentName = Environments.Production
});
builder.WebHost.UseUrls("http://0.0.0.0:5000");

var app = builder.Build();

app.MapGet("/scan", (string filePath) =>
{
    int totalFilesScanned = 0;
    totalFilesScanned = Directory.Exists(filePath) ? Directory.GetFiles(filePath, "*", SearchOption.AllDirectories).Length : (File.Exists(filePath) ? 1 : 0);

    var scanOutput = ScanResultFormatter.ExecuteCommand("clamdscan", $"\"{filePath}\" --multiscan --fdpass");
    var scanResults = ScanResultFormatter.ParseScanResult(scanOutput);
    var summary = ScanResultFormatter.ParseSummary(scanOutput);

    var engineVersion = ScanResultFormatter.ExecuteCommand("clamdscan", "--version").Split('\n').FirstOrDefault();
    var dbVersion = ScanResultFormatter.ExecuteCommand("sigtool", "--info /var/lib/clamav/main.cvd").Split('\n').FirstOrDefault(line => line.Contains("Version:"))?.Split(' ').Last();

    return Results.Ok(new
    {
        EngineVersion = engineVersion,
        DatabaseVersion = dbVersion,
        ScanResult = scanResults,
        TotalScannedFiles = totalFilesScanned,
        InfectedFiles = summary.ContainsKey("Infected files") ? int.Parse(summary["Infected files"]) : 0,
        Time = summary.ContainsKey("Time") ? summary["Time"] : string.Empty,
        StartDate = summary.ContainsKey("Start Date") ? summary["Start Date"] : string.Empty,
        EndDate = summary.ContainsKey("End Date") ? summary["End Date"] : string.Empty
    });
});

app.Run();
