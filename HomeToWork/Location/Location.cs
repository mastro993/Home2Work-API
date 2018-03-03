using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeToWork.Common;

namespace HomeToWork.Location
{
    [Serializable]
    public class Location
    {
        public int LocationId { get; set; }
        public LatLng LatLng { get; set; }
        public DateTime Timestamp { get; set; }

        public static Location Parse(SqlDataReader reader)
        {
            var locationId = (int) reader["id"];
            var lat = double.Parse(reader["latitude"].ToString());
            var lng = double.Parse(reader["longitude"].ToString());
            var locationTime = DateTime.Parse(reader["time"].ToString());

            return new Location()
            {
                LocationId = locationId,
                LatLng = new LatLng(lat, lng),
                Timestamp = locationTime
            };
        }
    }
}