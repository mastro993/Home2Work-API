using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeToWork.User
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