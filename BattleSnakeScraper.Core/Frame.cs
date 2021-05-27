using System.Collections.Generic;

namespace BattleSnakeScraper.Core
{
    public class Frame
    {
        public int Turn { get; set; }
        public IList<Snake> Snakes { get; set; }
        public IList<Food> Food { get; set; }
        //Object as we won't be really using this, we don't know what it is
        public object[] Hazards { get; set; }
    }
}