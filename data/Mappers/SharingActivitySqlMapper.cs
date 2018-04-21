using System.Data.SqlClient;
using data.Common;
using data.Common.Utils;
using domain.Entities;

namespace data.Mappers
{
    class SharingActivitySqlMapper : Mapper<SqlDataReader, SharingActivity>
    {
        public override SharingActivity MapFrom(SqlDataReader @from)
        {
            var year = @from["year"].ToInt();
            var month = @from["month"].ToInt();
            var shares = @from["shares"].ToInt();
            var distance = @from["distance"].ToInt();
            return new SharingActivity()
            {
                Year = year,
                Month = month,
                Shares = shares,
                SharesAvg = 0f,
                Distance = distance,
                DistanceAvg = 0f
            };
        }
    }
}