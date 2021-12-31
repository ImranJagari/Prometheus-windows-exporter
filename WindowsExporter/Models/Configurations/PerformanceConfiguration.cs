using WindowsExporter.Core.Enums;

namespace WindowsExporter.Models.Configurations
{
    public record PerformanceConfiguration : BaseConfiguration
    {
        public Dictionary<string, PerformanceCategoryConfiguration> Categories { get; set; }
    }

    public record PerformanceCategoryConfiguration
    {
        public bool Enabled { get; set; }
        public Dictionary<string, PerformanceCounterConfiguration> Counters { get; set; }
    }

    public record PerformanceCounterConfiguration
    {
        public PrometheusMetricTypeEnum MetricType { get; set; }
        public bool Enabled { get; set; }
    }
}
