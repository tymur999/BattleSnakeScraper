using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using BattleSnakeScraper.Core;
using Newtonsoft.Json;

namespace BattleSnakeScraper
{
    public class GameIdFetcher
    {
        private readonly HttpClient _client;

        public GameIdFetcher()
        {
            _client = new HttpClient();
        }
        public async Task<IList<string>> FetchGames()
        {
            var html = await _client.GetAsync("https://play.battlesnake.com/arena/global/");
            var data = await html.Content.ReadAsStringAsync();
            using var doc = await new HtmlParser().ParseDocumentAsync(data);
            var regex = new Regex("/g/");

            var links = doc.QuerySelectorAll("a").Cast<IHtmlAnchorElement>();
            return links.AsParallel()
                .Where(a => regex.IsMatch(a.Href))
                .Select(x => x.Href[11..47])
                .ToList();
        }

        public static IEnumerable<string> RemoveDuplicates(IList<string> gameIds)
        {
            if (gameIds == null) throw new ArgumentNullException(nameof(gameIds));
            return gameIds.Distinct();
        }

        public async Task<GameData> GetGameData(string id)
        {
            var response = await _client.GetStringAsync(
                $"https://engine.battlesnake.com/games/{id}");
            return JsonConvert.DeserializeObject<GameData>(response);
        }

        public async Task<bool> IsMinMaxSnake(string gameId, Snake snake)
        {
            string html;
            try
            {
                html = await _client.GetStringAsync($"https://play.battlesnake.com/g/{gameId}/");
            }
            catch(HttpRequestException e)
            {
                Debug.WriteLine(e);
                return false;
            }
            
            using var doc = await new HtmlParser().ParseDocumentAsync(html);
            
            var badges = doc.QuerySelector($"span[class='d-snake {snake.Id} d-snake-tail']");

            var row = badges.Parent.Parent as IHtmlTableRowElement;
            var cell = row!.QuerySelector("td[class='text-right']");
            var minMaxElement  = cell.ChildNodes.FirstOrDefault(n => n.TextContent == "MinMax");
            return minMaxElement != null;
        }

        ~GameIdFetcher()
        {
            _client.Dispose();
        }
    }
}