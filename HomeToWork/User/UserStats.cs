using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeToWork.User
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


        public static UserStats Parse(SqlDataReader reader)
        {
            var monthSharedDistance = (int) reader["month_shared_distance"];
            var monthlySharedDistanceAvg = float.Parse(reader["month_shared_distance_avg"].ToString());
            var totalSharedDistance = (int) reader["total_shared_distance"];

            var totalShares = (int) reader["total_shares"];
            var totalGuestShares = (int) reader["guest_shares"];
            var totalHostShares = (int) reader["host_shares"];
            var monthShares = (int) reader["month_shares"];
            var monthlySharesAvg = float.Parse(reader["month_shares_avg"].ToString());
            var bestMonthlyShares = (int)reader["month_shares_record"];
            var longestShare = (int)reader["longest_share"]; ;


            return new UserStats()
            {
                TotalShares = totalShares,
                TotalGuestShares = totalGuestShares,
                TotalHostShares = totalHostShares,
                MonthShares = monthShares,
                MonthlySharesAvg = monthlySharesAvg,
                TotalSharedDistance = totalSharedDistance,
                MonthSharedDistance = monthSharedDistance,
                MonthlySharedDistanceAvg = monthlySharedDistanceAvg,
                BestMonthShares = bestMonthlyShares,
                LongestShare = longestShare
            };
        }
    }
}