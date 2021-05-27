namespace BattleSnakeScraper.Core
{
    public class DeathEvent
    {
        public string Cause { get; set; }
        public int Turn { get; set; }
        public string EliminatedBy { get; set; }
    }
}