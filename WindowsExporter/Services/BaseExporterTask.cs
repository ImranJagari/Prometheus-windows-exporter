using Humanizer;
using WindowsExporter.Models.Configurations;
using WindowsExporter.Models.Https;

namespace WindowsExporter.Services
{
    public interface IExporterTask
    {
        bool CanExecute();
        Task<List<PrometheusDataModel>> ProcessAsync();
        void Update();
        void Initialize();
        bool IsInitialized { get; set; }
    }
    public abstract class BaseExporterTask<TConfiguration> : IExporterTask
        where TConfiguration : BaseConfiguration
    {
        protected TConfiguration _configuration;

        protected BaseExporterTask(IConfiguration configuration)
        {
            _configuration = configuration.GetSection(this.GetType().Name)?.Get<TConfiguration>();
        }

        public abstract Task<List<PrometheusDataModel>> ProcessAsync();
        public void Update()
        {
            IsInitialized = false;
            Initialize();
            IsInitialized = true;
        }
        public abstract void Initialize();
        public bool IsInitialized { get; set; }
        protected virtual string PrefixKeyName => _configuration.PrefixKeyName;
        public virtual bool CanExecute()
        {
            return _configuration?.Enabled ?? false;
        }

        public virtual IEnumerable<PrometheusFiltersValueModel> GetDatas(string key)
        {
            return null;
        }
        protected virtual string GetPrometheusKeyName(string name = null)
        {
            if (!string.IsNullOrWhiteSpace(name))
                return PrefixKeyName + "_" + name.Underscore();

            return PrefixKeyName;
        }
    }
}
