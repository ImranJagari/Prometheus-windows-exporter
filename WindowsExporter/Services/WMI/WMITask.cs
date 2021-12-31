using WindowsExporter.Models.Configurations;
using WindowsExporter.Models.Https;
using System.Management;
using Humanizer;
using WindowsExporter.Core.Enums;

namespace WindowsExporter.Services.OS
{
    internal class WMITask : BaseExporterTask<WMIConfiguration>
    {
        private readonly CimType[] _authorizedCimTypes = new CimType[]
        {
            CimType.Real32,
            CimType.Real64,
            CimType.SInt8,
            CimType.SInt16,
            CimType.SInt32,
            CimType.SInt64,
            CimType.UInt8,
            CimType.UInt16,
            CimType.UInt32,
            CimType.UInt64
        };
        Dictionary<string, PropertyData> _osProperties = new Dictionary<string, PropertyData>();
        public WMITask(IConfiguration configuration) : base(configuration)
        {
        }

        public override void Initialize()
        {
            foreach (var searcherConfig in _configuration.Searchers)
            {
                if (searcherConfig.Value.Enabled)
                {
                    var searcher = new ManagementObjectSearcher($"SELECT * FROM {searcherConfig.Key}");
                    foreach (var obj in searcher.Get())
                    {
                        if (searcherConfig.Value.Properties is null)
                        {
                            foreach (var prop in obj.Properties)
                            {
                                _osProperties.TryAdd(prop.Name, prop);
                            }
                        }
                        else
                        {
                            foreach (var prop in obj.Properties)
                            {
                                if (searcherConfig.Value.Properties.ContainsKey(prop.Name))
                                {
                                    _osProperties.TryAdd(prop.Name, prop);
                                }
                            }

                        }
                    }
                }
            }
        }

        public override Task<List<PrometheusDataModel>> ProcessAsync()
        {
            List<PrometheusDataModel> models = new List<PrometheusDataModel>();
            foreach(var prop in _osProperties)
            {
                models.Add(new PrometheusDataModel
                {
                    Description = $"Value for {prop.Key} from {prop.Value.Origin}.",
                    Type = PrometheusMetricTypeEnum.counter.ToString(),
                    KeyName = GetPrometheusKeyName((prop.Value.Origin + prop.Key).Underscore()),
                    FiltersValue = GetDatas(prop.Key)
                });
            }

            return Task.FromResult(models);
        }

        public override IEnumerable<PrometheusFiltersValueModel> GetDatas(string key)
        {
            if (_osProperties.ContainsKey(key) && _authorizedCimTypes.Contains(_osProperties[key].Type))
            {
                return new[]
                {
                    new PrometheusFiltersValueModel
                    {
                        Filters = string.Empty,
                        Value = $"{_osProperties[key].Value}"
                    }
                };
            }

            return Array.Empty<PrometheusFiltersValueModel>();
        }

        protected override string GetPrometheusKeyName(string name)
        {
            return $"windows_management_{name}";
        }
    }
}
