using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsExporter.Models.Https;
using WindowsExporter.Services.IIS;
using WindowsExporter.Services.Performance;

namespace WindowsExporter.Services.Background
{
    public class PrometheusScrapperService : BackgroundService
    {
        public static ConcurrentBag<PrometheusDataModel> PrometheusDatas = new ConcurrentBag<PrometheusDataModel>();
        private readonly IExporterTask[] exporterTasks;
        private readonly ILogger<PrometheusScrapperService> _logger;

        public PrometheusScrapperService(IExporterTask[] tasks, ILogger<PrometheusScrapperService> logger)
        {
            this.exporterTasks = tasks;
            this._logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    List<PrometheusDataModel> logs = new List<PrometheusDataModel>();

                    foreach(var task in exporterTasks)
                    {
                        try
                        {
                            logs.AddRange(await task.ProcessAsync());
                        }
                        catch(Exception ex)
                        {
                            _logger.LogError(ex, ex.Message);
                        }
                    }

                    foreach (var log in logs)
                    {
                        var existingLog = PrometheusDatas.FirstOrDefault(_ => _.KeyName == log.KeyName);
                        if (existingLog is not null)
                        {
                            existingLog.Type = log.Type;
                            existingLog.Description = log.Description;
                            existingLog.FiltersValue = log.FiltersValue;
                        }
                        else
                        {
                            PrometheusDatas.Add(log);
                        }
                    }

                }
            });

            return Task.CompletedTask;
        }
    }
}
