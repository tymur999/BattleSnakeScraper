namespace BattleSnakeScraper.Core
{
    public class Game
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Ruleset Ruleset { get; set; }
        public double SnakeTimeout { get; set; }
    }
}