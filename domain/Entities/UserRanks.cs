using System;

namespace domain.Entities
{
    [Serializable]
    public class UserRanks
    {
        public int Shares { get; set; }
        public int MonthShares { get; set; }
        public int MonthSharesAvg { get; set; }
        public int SharedDistance { get; set; }
        public int MonthSharedDistance { get; set; }
        public int MonthSharedDistanceAvg { get; set; }
    }
}