using LibreCards.Core;
using LibreCards.Core.Persistence;
using LibreCards.WebApp.Hubs;
using Microsoft.AspNetCore.ResponseCompression;

namespace LibreCards.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

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
                var lobby = new Lobby(gameStatus);
                var dataStorage = new DataStorage();
                var cardRepository = new CardRepository(dataStorage);
                var cardState = new CardState(cardRepository);
                var judgePicker = new JudgePicker();
                return new Game(gameStatus, cardState, lobby, judgePicker);
            });

            services.AddSingleton<IPlayerConnectionStorage, InMemoryPlayerConnectionStorage>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
