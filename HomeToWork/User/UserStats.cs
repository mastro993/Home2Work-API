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
        public int Shares { get; set; }
        public int GuestShares { get; set; }
        public int HostShares { get; set; }
        public int MonthShares { get; set; }
        public float MonthSharesAvg { get; set; }
        public int SharedDistance { get; set; }
        public int MonthSharedDistance { get; set; }
        public float MonthSharedDistanceAvg { get; set; }
        public UserRanks GlobalRanks { get; set; }
        public UserRanks CompanyRanks { get; set; }

        public static UserStats Parse(SqlDataReader reader)
        {
            var totalShares = (int) reader["total_shares"];
            var guestShares = (int) reader["guest_shares"];
            var hostShares = (int) reader["host_shares"];
            var monthShares = (int) reader["month_shares"];
            var monthSharesAvg = Single.Parse(reader["month_shares_avg"].ToString());
            var sharedDistance = (int) reader["total_shared_distance"];
            var monthSharedDistance = (int) reader["month_shared_distance"];
            var monthSharedDistanceAvg = Single.Parse(reader["month_shared_distance_avg"].ToString());

            var totalSharesRank = Int32.Parse(reader["total_shares_rank"].ToString());
            var monthSharesRank = Int32.Parse(reader["month_shares_rank"].ToString());
            var monthSharesAvgRank = Int32.Parse(reader["month_shares_avg_rank"].ToString());
            var totalSharedDistanceRank = Int32.Parse(reader["total_shared_distance_rank"].ToString());
            var monthSharedDistanceRank = Int32.Parse(reader["month_shared_distance_rank"].ToString());
            var monthSharedDistanceAvgRank = Int32.Parse(reader["month_shared_distance_avg_rank"].ToString());


            var totalSharesRankCompany = Int32.Parse(reader["total_shares_rank_company"].ToString());
            var monthSharesRankCompany = Int32.Parse(reader["month_shares_rank_company"].ToString());
            var monthSharesAvgRankCompany = Int32.Parse(reader["month_shares_avg_rank_company"].ToString());
            var totalSharedDistanceRankCompany = Int32.Parse(reader["total_shared_distance_rank_company"].ToString());
            var monthSharedDistanceRankCompany = Int32.Parse(reader["month_shared_distance_rank_company"].ToString());
            var monthSharedDistanceAvgRankCompany =
                Int32.Parse(reader["month_shared_distance_avg_rank_company"].ToString());

            return new UserStats()
            {
                Shares = totalShares,
                GuestShares = guestShares,
                HostShares = hostShares,
                MonthShares = monthShares,
                MonthSharesAvg = monthSharesAvg,
                SharedDistance = sharedDistance,
                MonthSharedDistance = monthSharedDistance,
                MonthSharedDistanceAvg = monthSharedDistanceAvg,
                GlobalRanks = new UserRanks()
                {
                    Shares = totalSharesRank,
                    MonthShares = monthSharesRank,
                    MonthSharesAvg = monthSharesAvgRank,
                    SharedDistance = totalSharedDistanceRank,
                    MonthSharedDistance = monthSharedDistanceRank,
                    MonthSharedDistanceAvg = monthSharedDistanceAvgRank
                },
                CompanyRanks = new UserRanks()
                {
                    Shares = totalSharesRankCompany,
                    MonthShares = monthSharesRankCompany,
                    MonthSharesAvg = monthSharesAvgRankCompany,
                    SharedDistance = totalSharedDistanceRankCompany,
                    MonthSharedDistance = monthSharedDistanceRankCompany,
                    MonthSharedDistanceAvg = monthSharedDistanceAvgRankCompany
                }
            };
        }
    }
}