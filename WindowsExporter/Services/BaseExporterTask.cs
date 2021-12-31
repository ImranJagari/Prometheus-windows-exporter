using Humanizer;
using WindowsExporter.Models.Configurations;
using WindowsExporter.Models.Https;

namespace WindowsExporter.Services
{
    internal interface IExporterTask
    {
        bool CanExecute();
        Task<List<PrometheusDataModel>> ProcessAsync();
        void Initialize();
    }
    internal abstract class BaseExporterTask<TConfiguration> : IExporterTask
        where TConfiguration : BaseConfiguration
    {
        protected TConfiguration _configuration;

        protected BaseExporterTask(IConfiguration configuration)
        {
            _configuration = configuration.GetSection(this.GetType().Name).Get<TConfiguration>();
        }

        public abstract Task<List<PrometheusDataModel>> ProcessAsync();
        public abstract void Initialize();

        protected virtual string PrefixKeyName => _configuration.PrefixKeyName;

        public virtual bool CanExecute()
        {
            return _configuration.Enabled;
        }

        public virtual IEnumerable<PrometheusFiltersValueModel> GetDatas(string key)
        {
            return null;
        }
        protected virtual string GetPrometheusKeyName(string name)
        {
            return PrefixKeyName + this.GetType().Name.Underscore();
        }
    }
}
