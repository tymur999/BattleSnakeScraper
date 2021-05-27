using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace BattleSnakeScraper.Data
{
    public class GameIdRepo : IGameIdRepo
    {
        private readonly string _cs;

        public GameIdRepo(IConfiguration config)
        {
            _cs = config["SnakeDb"];
        }

        public async Task<string> GetByGameId(string gameId)
        {
            await using var conn = new SqlConnection(_cs);
            return await conn.QueryFirstOrDefaultAsync<string>
                ("select top(1) gameId from Game.GameIds where GameId = @gameId", new { gameId });
        }

        public async Task<bool> AddGameId(string gameId)
        {
            await using var conn = new SqlConnection(_cs);
            return await conn.ExecuteAsync("insert into Game.GameIds(gameId) values(@gameId)", new {gameId}) != 0;
        }
    }
}