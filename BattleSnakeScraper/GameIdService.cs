using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using BattleSnakeScraper.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BattleSnakeScraper
{
    public class GameIdService : BackgroundService
    {
        private readonly IGameIdRepo _repo;
        private readonly ILogger<GameIdService> _logger;
        private readonly ServiceBusProcessor _receiver;

        public GameIdService(ServiceBusClient client, IGameIdRepo repo, ILogger<GameIdService> logger)
        {
            _repo = repo;
            _logger = logger;
            _receiver = client.CreateProcessor("gameIdQueue");;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _receiver.ProcessMessageAsync += ReceiverOnProcessMessageAsync;
            _receiver.ProcessErrorAsync += args =>
            {
                _logger.LogError(args.Exception, "Error in receiver", args);
                return Task.CompletedTask;
            };

            await _receiver.StartProcessingAsync(stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
                await Task.Delay(1000, stoppingToken);
        }

        private async Task ReceiverOnProcessMessageAsync(ProcessMessageEventArgs arg)
        {
            var gameId = arg.Message.Body.ToString();
            if (await _repo.GetByGameId(gameId) != null)
                return;

            await _repo.AddGameId(gameId);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.WhenAll(base.StopAsync(cancellationToken), _receiver.DisposeAsync().AsTask());
        }
    }
}