using WindowsExporter.Core.Helper;
using WindowsExporter.Services;
using WindowsExporter.Services.Background;
using WindowsExporter.Services.IIS;
using WindowsExporter.Services.OS;
using WindowsExporter.Services.Performance;

namespace WindowsExporter
{
    internal class Startup
    {
        private IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services
                .AddSingleton<PerformanceTask>()
                .AddSingleton<IISLogTask>()
                .AddSingleton<WMITask>()
                .AddHostedService<PrometheusScrapperService>(provider =>
                {
                    List<IExporterTask> services = new List<IExporterTask>();

                    var types = TypeHelper.GetTypesFromBase<IExporterTask>();
                    foreach (var type in types)
                    {
                        var service = provider.GetService(type) as IExporterTask;
                        if (service.CanExecute())
                        {
                            service.Initialize();
                            services.Add(service);
                        }
                    }

                    return new PrometheusScrapperService(services.ToArray());
                });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseCors(builder =>
            {
                builder.AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowAnyOrigin();
            });

            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }


    }
}
