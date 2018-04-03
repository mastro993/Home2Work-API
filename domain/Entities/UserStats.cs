using System;

namespace domain.Entities
{
    [Serializable]
    public class UserStats
    {
        public int MonthSharedDistance { get; set; }
        public float MonthlySharedDistanceAvg { get; set; }
        public int TotalSharedDistance { get; set; }
        public int TotalShares { get; set; }
        public int TotalGuestShares { get; set; }
        public int TotalHostShares { get; set; }
        public int MonthShares { get; set; }
        public float MonthlySharesAvg { get; set; }
        public int BestMonthShares { get; set; }
        public int LongestShare { get; set; }

    }
}