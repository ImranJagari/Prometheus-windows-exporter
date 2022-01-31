using System.Collections.Generic;
using WindowsExporter.Core.Enums;

namespace WindowsExporter.Models.Configurations
{
    public class PerformanceConfiguration : BaseConfiguration
    {
        public Dictionary<string, PerformanceCategoryConfiguration> Categories { get; set; }
    }

    public class PerformanceCategoryConfiguration
    {
        public bool Enabled { get; set; }
        public Dictionary<string, PerformanceCounterConfiguration> Counters { get; set; }
    }

    public class PerformanceCounterConfiguration
    {
        public PrometheusMetricTypeEnum MetricType { get; set; }
        public bool Enabled { get; set; }
    }
}
