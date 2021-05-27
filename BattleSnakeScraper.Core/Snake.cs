using System.Collections.Generic;

namespace BattleSnakeScraper.Core
{
    public class Snake
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public IList<SnakeBody> Body { get; set; }
        public int Health { get; set; }
        public DeathEvent Death { get; set; }
        public string Color { get; set; }
        public string HeadType { get; set; }
        public string TailType { get; set; }
        public string Latency { get; set; }
        public string Shout { get; set; }
        public string Squad { get; set; }
        public string ApiVersion { get; set; }
        public string Author { get; set; }
    }
}