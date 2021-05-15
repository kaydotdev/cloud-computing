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
        public DateTime ReleaseDate { get; set; }
        public PriceHistory[] PriceHistory { get; set; }
    }
    
    public class GamePreview
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Origin { get; set; }
        public IEnumerable<string> Genres { get; set; }
        public IEnumerable<string> Developers { get; set; }
        public DateTime ReleaseDate { get; set; }
    }

    public class PriceHistory
    {
        public DateTime DatePrice { get; set; }
        public double InitialPrice { get; set; }
        public double FinalPrice { get; set; }
        public int DiscountOnPrice { get; set; }
    }
}