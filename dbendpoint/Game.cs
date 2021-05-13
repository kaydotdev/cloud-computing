using System;
using System.Collections.Generic;

namespace GameStore.Games.Game
{
    public class Game
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Origin { get; set; }
        public IEnumerable<string> Genres { get; set; }
        public IEnumerable<string> Developers { get; set; }
        public DateTime Release_Date { get; set; }
        public IDictionary<DateTime, double> Price_History { get; set; }
    }
}