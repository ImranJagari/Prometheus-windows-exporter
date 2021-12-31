﻿using Humanizer;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsExporter.Core.Enums;
using WindowsExporter.Models.Configurations;
using WindowsExporter.Models.Https;

namespace WindowsExporter.Services.Performance
{
    internal class PerformanceTask : BaseExporterTask<PerformanceConfiguration>
    {
        private IEnumerable<PerformanceCounter> _counters;
        private static readonly Dictionary<string, string> unicodeCharactersName = new()
        {
            { "%", "percent" },
            { "/sec", " per seconds" },
            { "(s)", "per seconds" },
            { "&", "and" },
            { ".", string.Empty }
        };
        private ConcurrentBag<PrometheusDataModel> _models = new ConcurrentBag<PrometheusDataModel>();

        public PerformanceTask(IConfiguration configuration) : base(configuration)
        {
        }

        public override void Initialize()
        {
            _counters = _configuration.Categories.Select(_ => new PerformanceCounterCategory(_.Key)).SelectMany(_ =>
            {
                if (!PerformanceCounterCategory.Exists(_.CategoryName) || !_configuration.Categories[_.CategoryName].Enabled)
                    return new List<PerformanceCounter>();

                List<PerformanceCounter> countersCategories = new List<PerformanceCounter>();
                if (_.CategoryType == PerformanceCounterCategoryType.MultiInstance)
                {
                    foreach (var instance in _.GetInstanceNames())
                    {
                        var configCounters = _configuration.Categories[_.CategoryName].Counters;
                        PerformanceCounter[] counters = _.GetCounters(instance);

                        if (configCounters is not null)
                            counters = counters.Where(_ => configCounters.ContainsKey(_.CounterName) && configCounters[_.CounterName].Enabled).ToArray();

                        countersCategories.AddRange(counters);
                    }
                }
                else
                {
                    countersCategories.AddRange(_.GetCounters());
                }


                return countersCategories;
            }).ToList();
        }

        public override Task<List<PrometheusDataModel>> ProcessAsync()
        {
            _models.Clear();
            
            foreach(var counter in _counters)
            {
                var key = GetPrometheusKeyName($"{counter.CategoryName} {counter.CounterName}");
                var model = _models.FirstOrDefault(_ => _.KeyName == key);

                if (model is null)
                {
                    _models.Add(new PrometheusDataModel
                    {
                        KeyName = key,
                        Description = string.Concat(counter.CounterHelp.TakeWhile(_ => !_.Equals('.'))),
                        Type = GetMetricType(counter).ToString(),
                        FiltersValue = GetDatas(counter.CounterName)
                    });
                }
                else
                {
                    List<PrometheusFiltersValueModel> datas = model.FiltersValue.ToList();
                    datas.AddRange(GetDatas(counter.CounterName));
                    model.FiltersValue = datas;
                }
            }

            return Task.FromResult(_models.ToList());
        }


        public override IEnumerable<PrometheusFiltersValueModel> GetDatas(string key)
        {
            var counter = _counters.FirstOrDefault(_ => _.CounterName == key);
            if(counter is not null)
            {
                float value = counter.NextValue();

                if (string.IsNullOrWhiteSpace(counter.InstanceName))
                {
                    yield return new PrometheusFiltersValueModel
                    {
                        Filters = string.Empty,
                        Value = value.ToString(CultureInfo.InvariantCulture)
                    };
                }
                else
                {
                    yield return new PrometheusFiltersValueModel
                    {
                        Filters = $"{{counter_instance=\"{counter.InstanceName}\"}}",
                        Value = value.ToString(CultureInfo.InvariantCulture)
                    };
                }
            }
        }

        protected override string GetPrometheusKeyName(string name)
        {
            foreach (var character in unicodeCharactersName)
                name = name.Replace(character.Key, character.Value);

            return $"windows_{name.Trim().Underscore()}";
        }

        private PrometheusMetricTypeEnum GetMetricType(PerformanceCounter counter)
        {
            if (_configuration.Categories.ContainsKey(counter.CategoryName) &&  (_configuration.Categories[counter.CategoryName].Counters?.ContainsKey(counter.CounterName) ?? false))
            {
                return _configuration.Categories[counter.CategoryName].Counters[counter.CounterName].MetricType;
            }
            return PrometheusMetricTypeEnum.counter;
        }
    }
}
