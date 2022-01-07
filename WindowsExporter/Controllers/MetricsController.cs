using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WindowsExporter.Models.Https;
using WindowsExporter.Services;

namespace WindowsExporter.Controllers
{
    public class MetricsController : Controller
    {
        private readonly IExporterTask[] exporterTasks;
        private readonly ILogger<MetricsController> _logger;
        private readonly IMemoryCache _cache;

        public MetricsController(IExporterTask[] exporterTasks, ILogger<MetricsController> logger, IMemoryCache cache)
        {
            this.exporterTasks = exporterTasks;
            this._logger = logger;
            this._cache = cache;
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
                    {
                        List<PrometheusDataModel> collection = await task.ProcessAsync();
                        _cache.Set(task.GetType().Name, collection);
                        list.AddRange(collection);
                    }
                    else
                    {
                        list.AddRange(_cache.Get<List<PrometheusDataModel>>(task.GetType().Name) ?? new List<PrometheusDataModel>());
                    }
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
