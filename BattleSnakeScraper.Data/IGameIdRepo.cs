using System.Threading.Tasks;

namespace BattleSnakeScraper.Data
{
    public interface IGameIdRepo
    {
        public Task<string> GetByGameId(string gameId);
        public Task<bool> AddGameId(string gameId);
    }
}