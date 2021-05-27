using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BattleSnakeScraper
{
    public class GamePingService : BackgroundService
    {
        private readonly GameIdFetcher _fetcher;
        private readonly ILogger<GamePingService> _logger;
        private readonly ServiceBusSender _sender;
        private Timer _timer;

        public GamePingService(GameIdFetcher fetcher, ServiceBusClient client, ILogger<GamePingService> logger)
        {
            _fetcher = fetcher;
            _logger = logger;
            _sender = client.CreateSender("gameIdQueue");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer =  new 
                Timer(async obj => await CollectSnakeGamesAsync(), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            await _timer.DisposeAsync();
        }

        private async Task CollectSnakeGamesAsync()
        {
            Console.WriteLine("Collecting snake info");
            var ids = await _fetcher.FetchGames();

            var listT = new List<Task>(ids.Count);
            
            foreach (var id in ids)
            {
                listT.Add(InspectGame(id));
            }

            await Task.WhenAll(listT);
        }

        private async Task InspectGame(string id)
        {
            var data = await _fetcher.GetGameData(id);

            var winner = data.LastFrame.Snakes.FirstOrDefault(s => s.Death == null);
            if (winner == null)
                return;
            
            var strategy = await _fetcher.IsMinMaxSnake(data.Game.Id, winner);
            if (!strategy)
                return;

            await _sender.SendMessageAsync(new ServiceBusMessage{Body = new BinaryData(id)});
            _logger.LogInformation($"Added game id {id}");
        }
    }
}