using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClamAvApi.Models
{
    public class Outputs
    {
        [JsonPropertyName("Result Scan")]
        public Dictionary<string, string> ResultScan { get; set; } = new();
        [JsonPropertyName("Known viruses")]
        public string KnownViruses { get; set; }
        [JsonPropertyName("Engine version")]
        public string EngineVersion { get; set; }
        [JsonPropertyName("Scanned directories")]
        public int ScannedDirectories { get; set; }
        [JsonPropertyName("Scanned files")]
        public int ScannedFiles { get; set; }
        [JsonPropertyName("Infected files")]
        public int InfectedFiles { get; set; }
        [JsonPropertyName("Data scanned")]
        public string DataScanned { get; set; }
        [JsonPropertyName("Data read")]
        public string DataRead { get; set; }
        [JsonPropertyName("Time")]
        public string Time { get; set; }
        [JsonPropertyName("Start Date")]
        public string StartDate { get; set; }
        [JsonPropertyName("End Date")]
        public string EndDate { get; set; }
    }
}
