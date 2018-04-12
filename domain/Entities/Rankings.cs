using System;

namespace domain.Entities
{
    [Serializable]
    public class Rankings
    {
        public int Shares { get; set; }
        public int SharesAvg { get; set; }
        public int SharedDistance { get; set; }
        public int SharedDistanceAvg { get; set; }
        public int LongestShare { get; set; }
    }
}