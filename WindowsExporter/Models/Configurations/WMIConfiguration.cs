using WindowsExporter.Core.Enums;

namespace WindowsExporter.Models.Configurations
{
    public record WMIConfiguration : BaseConfiguration
    {
        public Dictionary<string, WMISearcherConfiguration> Searchers { get; set; }
    }

    public record WMISearcherConfiguration
    {
        public bool Enabled { get; set; }
        public Dictionary<string, WMIObjectConfiguration> Properties { get; set; }
    }

    public record WMIObjectConfiguration
    {
        public PrometheusMetricTypeEnum MetricType { get; set; }
        public bool Enabled { get; set; }
    }
}
