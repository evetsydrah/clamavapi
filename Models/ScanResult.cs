namespace ClamAvApi.Models
{
    public class ScanResult
    {
        public Outputs Outputs { get; set; } = new();
        public string Errors { get; set; }
        public int ExitCode { get; set; }
    }
}
