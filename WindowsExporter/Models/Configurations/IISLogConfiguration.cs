namespace WindowsExporter.Models.Configurations
{
    public record IISLogConfiguration : BaseConfiguration
    {
        public string PathToLogs { get; set; }
    }
}
