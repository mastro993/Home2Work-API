using System;
using System.Data.SqlClient;
using data.Common;
using data.Common.Utils;
using domain.Entities;

namespace data.Mappers
{
    class SclLocationSqlMapper : Mapper<SqlDataReader, UserLocation>

    {
        public override UserLocation MapFrom(SqlDataReader @from)
        {
            var locationId = @from["id"].ToLong();
            var userId = @from["user_id"].ToLong();
            var lat = @from["latitude"].ToDouble();
            var lng = @from["longitude"].ToDouble();
            var locationTime = DateTime.Parse(@from["time"].ToString());
            var type = (UserLocation.LocationType) @from["type"].ToInt();

            return new UserLocation()
            {
                LocationId = locationId,
                UserId = userId,
                Latitude = lat,
                Longitude = lng,
                Date = locationTime,
                Type = type
            };
        }
    }
}