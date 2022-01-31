using System.Collections.Generic;
using System.Linq;
using WindowsExporter.Core.Enums;

namespace WindowsExporter.Models.Https
{
    public class PrometheusDataModel
    {
        public string KeyName { get; set; }
        public PrometheusMetricTypeEnum Type { get; set; }
        public string Description { get; set; }
        public IEnumerable<PrometheusFiltersValueModel> FiltersValue { get; set; }



        public string GetDescriptionKey()
        {
            return $"# HELP {KeyName} {Description}";
        }
        public virtual string GetTypeKey()
        {
            return $"# TYPE {KeyName} {Type}";
        }
        public string GetFiltersValue()
        {
            return string.Join('\n', FiltersValue.Select(_ => $"{KeyName}{_.Filters} {_.Value}"));
        }
    }
}
