using Humanizer;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading;

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
