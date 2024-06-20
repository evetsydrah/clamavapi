using ClamAvApi.Models;
using System;
using System.Collections.Generic;

namespace ClamAvApi.Helper
{
    public static class ClamAVParser
    {
        public static ScanResult ParseClamAVOutput(string output, string errors, int exitCode)
        {
            var result = new ScanResult
            {
                Errors = errors,
                ExitCode = exitCode
            };

            var lines = output.Split('\n');
            var isSummarySection = false;

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                if (line.StartsWith("----------- SCAN SUMMARY -----------"))
                {
                    isSummarySection = true;
                    continue;
                }

                if (!isSummarySection)
                {
                    var parts = line.Split(new[] { ": " }, 2, StringSplitOptions.None);
                    if (parts.Length == 2)
                    {
                        result.Outputs.ResultScan[parts[0].Trim()] = parts[1].Trim();
                    }
                }
                else
                {
                    var parts = line.Split(new[] { ':' }, 2, StringSplitOptions.None);
                    if (parts.Length == 2)
                    {
                        var key = parts[0].Trim();
                        var value = parts[1].Trim();

                        switch (key)
                        {
                            case "Known viruses":
                                result.Outputs.KnownViruses = value;
                                break;
                            case "Engine version":
                                result.Outputs.EngineVersion = value;
                                break;
                            case "Scanned directories":
                                result.Outputs.ScannedDirectories = int.Parse(value);
                                break;
                            case "Scanned files":
                                result.Outputs.ScannedFiles = int.Parse(value);
                                break;
                            case "Infected files":
                                result.Outputs.InfectedFiles = int.Parse(value);
                                break;
                            case "Data scanned":
                                result.Outputs.DataScanned = value;
                                break;
                            case "Data read":
                                result.Outputs.DataRead = value;
                                break;
                            case "Time":
                                result.Outputs.Time = value;
                                break;
                            case "Start Date":
                                result.Outputs.StartDate = value;
                                break;
                            case "End Date":
                                result.Outputs.EndDate = value;
                                break;
                        }
                    }
                }
            }

            return result;
        }
    }
}
