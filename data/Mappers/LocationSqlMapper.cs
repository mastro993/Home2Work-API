using System;
using System.Data.SqlClient;
using data.Common;
using data.Common.Utils;
using domain.Entities;

namespace data.Mappers
{
    class LocationSqlMapper: Mapper<SqlDataReader, Location>

    {
        public override Location MapFrom(SqlDataReader @from)
        {
            var locationId = @from["id"].ToLong();
            var userId = @from["user_id"].ToLong();
            var lat = @from["latitude"].ToDouble();
            var lng =@from["longitude"].ToDouble();
            var locationTime = LocalDateTime.Parse(@from["time"].ToString());

            return new Location()
            {
                LocationId = locationId,
                UserId = userId,
                Latitude = lat,
                Longitude = lng,
                Date = locationTime
            };
        }
    }
}
