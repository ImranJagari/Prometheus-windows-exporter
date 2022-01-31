using Humanizer;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using WindowsExporter.Core.Enums;
using WindowsExporter.Models.Configurations;
using WindowsExporter.Models.Https;

namespace WindowsExporter.Services.Tasks.ComputerSystem
{
    public class ComputerSystemTask : BaseExporterTask<ComputerSystemConfiguration>
    {
        public ComputerSystemTask(IConfiguration configuration) : base(configuration)
        {
        }

        public override void Initialize()
        {
        }

        public override Task<List<PrometheusDataModel>> ProcessAsync()
        {
            string domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;
            string hostName = Dns.GetHostName();

            var datas = new List<PrometheusDataModel>
            {
                new PrometheusDataModel
                {
                    Description = "Labeled system hostname information as provided by ComputerSystem.DNSHostName and ComputerSystem.Domain",
                    KeyName = GetPrometheusKeyName("hostname"),
                    Type = PrometheusMetricTypeEnum.counter,
                    FiltersValue = new List<PrometheusFiltersValueModel>
                    {
                        new PrometheusFiltersValueModel
                        {
                            Filters = $"{{domain=\"{domainName}\",hostname=\"{hostName}\",fqdn=\"{domainName + "." + hostName}\"}}",
                            Value = "1"
                        }
                    }
                }
            };

            return Task.FromResult(datas);
        }
    }
}
