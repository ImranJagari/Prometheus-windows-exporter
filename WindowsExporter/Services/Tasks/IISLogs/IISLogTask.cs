using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WindowsExporter.Core.Enums;
using WindowsExporter.Models.Configurations;
using WindowsExporter.Models.Https;
using WindowsExporter.Models.Internal;

namespace WindowsExporter.Services.Tasks.IISLogs
{
    internal class IISLogTask : BaseExporterTask<IISLogConfiguration>
    {
        private static string RequestStatusCountKeyName = "requests_status_count";
        private static string RequestMethodCountKeyName = "requests_method_count";

        private static ConcurrentDictionary<string, List<IISLogEvent>> _logs = new ConcurrentDictionary<string, List<IISLogEvent>>();

        public IISLogTask(IConfiguration configuration) : base(configuration)
        {
        }

        public override void Initialize()
        {
        }

        public override Task<List<PrometheusDataModel>> ProcessAsync()
        {
            var files = Directory.EnumerateFiles(_configuration.PathToLogs, $"u_ex{DateTime.Now:yyMMdd}*.log", SearchOption.AllDirectories);
            Parallel.ForEach(files, file =>
            {
                ReadFile(file);
            });

            List<PrometheusDataModel> datas = new List<PrometheusDataModel>();

            datas.Add(new PrometheusDataModel
            {
                Type = PrometheusMetricTypeEnum.counter,
                Description = "Total request on the day based on site and on the method",
                KeyName = GetPrometheusKeyName(RequestMethodCountKeyName),
                FiltersValue = GetDatas(RequestMethodCountKeyName)
            });

            datas.Add(new PrometheusDataModel
            {
                Type = PrometheusMetricTypeEnum.counter,
                Description = "Total request on the day based on site and on the response status code",
                KeyName = GetPrometheusKeyName(RequestStatusCountKeyName),
                FiltersValue = GetDatas(RequestStatusCountKeyName)
            });

            return Task.FromResult(datas);
        }

        private void ReadFile(string filePath)
        {
            using (IISLogEngine parser = new IISLogEngine(filePath))
            {
                IEnumerable<IISLogEvent> collection = parser.ParseLog();
                if (_logs.ContainsKey(filePath))
                {
                    _logs[filePath] = collection.ToList();
                }
                else
                {
                    _logs.TryAdd(filePath, collection.ToList());
                }
            }
        }

        public override IEnumerable<PrometheusFiltersValueModel> GetDatas(string key)
        {
            foreach (var siteGroup in _logs.Where(_ => _.Key.Contains($"u_ex{DateTime.Now:yyMMdd}")).SelectMany(_ => _.Value).GroupBy(_ => _.sSitename))
            {
                if (key == RequestStatusCountKeyName)
                {
                    foreach (var group in siteGroup.GroupBy(_ => _.scStatus))
                    {
                        yield return new PrometheusFiltersValueModel
                        {
                            Filters = $"{{site=\"{siteGroup.Key}\",status=\"{group.Key}\"}}",
                            Value = $"{group.Count()}"
                        };
                    }
                }
                else if (key == RequestMethodCountKeyName)
                {
                    foreach (var group in siteGroup.GroupBy(_ => _.csMethod))
                    {
                        yield return new PrometheusFiltersValueModel
                        {
                            Filters = $"{{site=\"{siteGroup.Key}\",method=\"{group.Key}\"}}",
                            Value = $"{group.Count()}"
                        };
                    }
                }
            }
        }
    }
}
