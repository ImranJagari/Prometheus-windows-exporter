using System.Collections.Generic;
using WindowsExporter.Core.Enums;

namespace WindowsExporter.Models.Configurations
{
    public class WMIConfiguration : BaseConfiguration
    {
        public Dictionary<string, WMISearcherConfiguration> Searchers { get; set; }
    }

    public class WMISearcherConfiguration
    {
        public bool Enabled { get; set; }
        public Dictionary<string, WMIObjectConfiguration> Properties { get; set; }
    }

    public class WMIObjectConfiguration
    {
        public PrometheusMetricTypeEnum MetricType { get; set; }
        public bool Enabled { get; set; }
    }
}
