using System.Data.SqlClient;
using data.Common;
using data.Common.Utils;
using domain.Entities;

namespace data.Mappers
{
    class UserStatsSqlMapper: Mapper<SqlDataReader, UserStats>
    {
        public override UserStats MapFrom(SqlDataReader @from)
        {
            var monthSharedDistance = @from["monthly_shared_distance"].ToInt();
            var monthlySharedDistanceAvg =@from["monthly_shared_distance_avg"].ToFloat();
            var totalSharedDistance = @from["total_shared_distance"].ToInt();

            var totalShares =@from["total_shares"].ToInt();
            var totalGuestShares = @from["guest_shares"].ToInt();
            var totalHostShares = @from["host_shares"].ToInt();
            var monthShares = @from["monthly_shares"].ToInt();
            var monthlySharesAvg = @from["monthly_shares_avg"].ToFloat();
            var bestMonthlyShares = @from["monthly_shares_record"].ToInt();
            var longestShare = @from["longest_share"].ToInt(); 


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
