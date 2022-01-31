using Humanizer;
using Microsoft.Extensions.Configuration;
using Microsoft.Web.Administration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WindowsExporter.Core.Enums;
using WindowsExporter.Models.Configurations;
using WindowsExporter.Models.Https;

namespace WindowsExporter.Services.Tasks.IIS
{
    public class IISTask : BaseExporterTask<IISConfiguration>
    {
        public IISTask(IConfiguration configuration) : base(configuration)
        {
        }

        public override void Initialize()
        {
        }

        public override Task<List<PrometheusDataModel>> ProcessAsync()
        {
            var iisManager = new ServerManager();
            SiteCollection sites = iisManager.Sites;

            var datas = new List<PrometheusDataModel>
            {
                new PrometheusDataModel
                {
                    Description = "IIS Websites mapping site id with site name",
                    KeyName = GetPrometheusKeyName("sites"),
                    Type = PrometheusMetricTypeEnum.counter,
                    FiltersValue = sites?.Select(_ => new PrometheusFiltersValueModel
                    {
                        Filters = $"{{site_id=\"W3SVC{_.Id}\",site_name=\"{_.Name}\",state=\"{_.State}\"}}",
                        Value = _.State == ObjectState.Started ? "1" : "0"
                    }) ?? new List<PrometheusFiltersValueModel>()
                }
            };

            return Task.FromResult(datas);
        }
    }
}
