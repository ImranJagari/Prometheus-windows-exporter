using WindowsExporter.Models.Configurations;
using WindowsExporter.Models.Https;
using System.ServiceProcess;
using System.Collections.Concurrent;
using System.Management;
using WindowsExporter.Core.Enums;

namespace WindowsExporter.Services.Tasks.Services
{
    public class ServicesWatcherTask : BaseExporterTask<ServicesWatcherConfiguration>
    {
        public ServicesWatcherTask(IConfiguration configuration) : base(configuration)
        {
        }

        public override void Initialize()
        {
        }

        public override Task<List<PrometheusDataModel>> ProcessAsync()
        {
            ConcurrentBag<PrometheusFiltersValueModel> filters = new ConcurrentBag<PrometheusFiltersValueModel>();

            Parallel.ForEach(_configuration.Services, service =>
            {
                var serviceController = new ServiceController(service.ServiceName);
                filters.Add(new PrometheusFiltersValueModel
                {
                    Filters = $"{{name=\"{serviceController.ServiceName},status=\"{serviceController.Status}\"\"}}",
                    Value = serviceController.Status == ServiceControllerStatus.Running ? "1" : "0"
                });

                if (service.CanRestart && serviceController.Status == ServiceControllerStatus.Stopped)
                    serviceController.Start();
            });

            return Task.FromResult(new List<PrometheusDataModel>
            {
                new PrometheusDataModel
                {
                    Description = "Windows services list with their status, can be restarted when stopped if it's setted in the configuration",
                    KeyName = GetPrometheusKeyName(),
                    Type = PrometheusMetricTypeEnum.counter,
                    FiltersValue = filters.ToList()
                }
            });
        }
    }
}
