namespace DatabaseComparer.Models
{
    public class ConnectionModel
    {
        public string SourceServer { get; set; } = string.Empty;
        public string SourceDatabase { get; set; } = string.Empty;
        public string SourceAuthType { get; set; } = "Windows";
        public string? SourceUsername { get; set; }
        public string? SourcePassword { get; set; }

        public string DestinationServer { get; set; } = string.Empty;
        public string DestinationDatabase { get; set; } = string.Empty;
        public string DestinationAuthType { get; set; } = "Windows";
        public string? DestinationUsername { get; set; }
        public string? DestinationPassword { get; set; }

        public List<string> SourceDatabaseList { get; set; } = new();
        public List<string> DestinationDatabaseList { get; set; } = new();
    }
}