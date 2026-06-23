using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BigClubDebate.Data;
using BigClubDebate.Data.Model.DataSources;
using BigClubDebate.Data.Model.Reader;
using BigClubDebate.Web.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BigClubDebate.Web
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor()
                .AddCircuitOptions(options => options.DetailedErrors = _env.IsDevelopment());
            services.AddSingleton(s => FootballDataFolderConfig.FromEntryAssemblyPath());
            services.AddSingleton<FootyDataReader>();
            services.AddSingleton<IGameDataProvider>(s => s.GetRequiredService<FootyDataReader>());
            services.AddSingleton(s => new TransfermarktCsvReader(
                s.GetRequiredService<FootballDataFolderConfig>().TransfermarktGamesCsvPath));
            services.AddSingleton(s => new ChampsCsvReader(
                s.GetRequiredService<FootballDataFolderConfig>().EngSoccerDataChampsCsvPath));
            services.AddSingleton<EuropeanCupGames>();
            services.AddSingleton<Teams>();
            services.AddSingleton<WittyTagLineGenerator>();
            services.AddSingleton<LeagueGames>();
            services.AddSingleton<CupGames>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
