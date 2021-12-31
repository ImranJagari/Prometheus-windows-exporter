namespace WindowsExporter.Models.Configurations
{
    public record BaseConfiguration
    {
        public bool Enabled { get; set; }
        public string PrefixKeyName { get; set; }
    }
}
