using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using LibreCards.WebApp.Hubs;
using Microsoft.AspNetCore.ResponseCompression;
using LibreCards.Core;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace LibreCards.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddControllersWithViews();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            services.AddSingleton<IGame, Game>(s =>
            {
                var gameStatus = new GameStatus();
                var lobby = new Lobby(3, gameStatus);
                var dataStorage = new DataStorage();
                var cardRepository = new CardRepository(dataStorage);
                return new Game(gameStatus, cardRepository, lobby);
            });
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();

            app.Map("/signalr", map =>
            {
#if DEBUG
                map.UseCors(builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(origin => true)
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .Build());
#endif
                map.UseRouting();
                map.UseAuthorization();
                map.UseEndpoints(endpoints =>
                {
                    endpoints.MapHub<GameHub>("/cardsgame");
                });
            });
        }
    }
}
