using System;

namespace domain.Entities
{
    [Serializable]
    public class SharingActivity
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Shares { get; set; }
        public float SharesAvg { get; set; }
        public int Distance { get; set; }
        public float DistanceAvg { get; set; }
    }
}