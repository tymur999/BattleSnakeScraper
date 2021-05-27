using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using BattleSnakeScraper.Core;
using BattleSnakeScraper.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BattleSnakeScraper
{
    public static class Program
    {
        private static async Task Main(string[] args)
        {
            await new HostBuilder()
                .ConfigureHostConfiguration(c =>
                {
                    c.AddJsonFile("appsettings.json");
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddLogging();
                    services.AddSingleton<GameIdFetcher>();
                    services.AddHostedService<GameIdService>();
                    services.AddHostedService<GamePingService>();
                    services.AddSingleton(new ServiceBusClient(context.Configuration["Bus:Connection"]));
                    services.AddTransient<IGameIdRepo, GameIdRepo>();
                })
                .RunConsoleAsync();
            
        }
    }
}