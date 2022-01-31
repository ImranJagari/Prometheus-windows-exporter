namespace WindowsExporter.Models.Configurations
{
    public record ServicesWatcherConfiguration : BaseConfiguration
    {
        public ServiceConfiguration[] Services { get; set; }
    }

    public record ServiceConfiguration
    {
        public string ServiceName { get; set; }
        public bool CanRestart { get; set; }
    }
}
