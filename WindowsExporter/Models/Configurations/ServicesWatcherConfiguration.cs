namespace WindowsExporter.Models.Configurations
{
    public class ServicesWatcherConfiguration : BaseConfiguration
    {
        public ServiceConfiguration[] Services { get; set; }
    }

    public class ServiceConfiguration
    {
        public string ServiceName { get; set; }
        public bool CanRestart { get; set; }
    }
}
