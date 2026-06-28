using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BigClubDebate.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Use Startup for service configuration (keeps existing Startup logic intact)
            var startup = new Startup(builder.Configuration, builder.Environment);
            startup.ConfigureServices(builder.Services);

            // Add Aspire service defaults (telemetry, health checks, service discovery, resilience)
            builder.AddServiceDefaults();

            var app = builder.Build();

            // Map Aspire default endpoints (health checks) before the Startup pipeline
            app.MapDefaultEndpoints();

            startup.Configure(app, app.Environment);

            app.Run();
        }
    }
}
