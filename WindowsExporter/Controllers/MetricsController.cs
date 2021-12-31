using Microsoft.AspNetCore.Mvc;
using WindowsExporter.Services.Background;

namespace WindowsExporter.Controllers
{
    public class MetricsController : Controller
    {
        [Route("/metrics")]
        public string GetMetrics()
        {
            return string.Join('\n', PrometheusScrapperService.PrometheusDatas.Select(_ =>
            {
                if (_.FiltersValue.Any() && _.FiltersValue.All(filter => !string.IsNullOrWhiteSpace(filter.Value)))
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
