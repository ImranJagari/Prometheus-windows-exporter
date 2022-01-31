using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using WindowsExporter.Core.Helper;
using WindowsExporter.Services;
using WindowsExporter.Services.Background;

namespace WindowsExporter
{
    public class Startup
    {
        private IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            var types = TypeHelper.GetTypesFromBase<IExporterTask>();

            foreach (var type in types)
                services.AddSingleton(type);

            services
                .AddSingleton<IExporterTask[]>(provider =>
                {
                    List<IExporterTask> tasks = new List<IExporterTask>();

                    foreach (var type in types)
                    {
                        var service = provider.GetService(type) as IExporterTask;
                        if (service.CanExecute())
                        {
                            service.Initialize();
                            service.IsInitialized = true;
                            tasks.Add(service);
                        }
                    }

                    return tasks.ToArray();
                })
                .AddHostedService<TaskUpdaterBackgroundService>();

            services.AddControllers();

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
