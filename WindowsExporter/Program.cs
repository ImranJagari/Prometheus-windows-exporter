using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Net;


namespace WindowsExporter
{
    public class Program
    {
        static void Main(string[] args)
        {
            WebHost.CreateDefaultBuilder(args)
                .UseUrls($"http://{Dns.GetHostName()}:9182")
                .UseStartup<Startup>()
                .Build()
                .Run();
        }
    }
}
