using Microsoft.AspNetCore.Mvc;
using WindowsExporter.Models.Https;
using WindowsExporter.Services;
using WindowsExporter.Services.Background;

namespace WindowsExporter.Controllers
{
    public class MetricsController : Controller
    {
        private readonly IExporterTask[] exporterTasks;
        private readonly ILogger<MetricsController> _logger;

        public MetricsController(IExporterTask[] exporterTasks, ILogger<MetricsController> logger)
        {
            this.exporterTasks = exporterTasks;
            this._logger = logger;
        }

        [Route("/metrics")]
        public async Task<string> GetMetrics()
        {
            List<PrometheusDataModel> list = new List<PrometheusDataModel>();

            foreach (var task in exporterTasks)
            {
                try
                {
                    if (task.IsInitialized)
                        list.AddRange(await task.ProcessAsync());
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }

            return string.Join('\n', list.Select(_ =>
            {
                if (_.FiltersValue.Where(_ => !string.IsNullOrWhiteSpace(_.Value)).Any())
                {
                    return $"{_.GetDescriptionKey()}\n" +
                           $"{_.GetTypeKey()}\n" +
                           $"{_.GetFiltersValue()}";
                }

                return string.Empty;
            }).Where(_ => !string.IsNullOrEmpty(_)));
        }

    }
}
