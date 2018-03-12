using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeToWork.Database;

namespace HomeToWork.Location
{
    public class LocationDao
    {
        public List<Location> GetAllLocations()
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = @"SELECT * FROM location",
                Connection = con
            };

            con.Open();

            var locations = new List<Location>();

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    locations.Add(Location.Parse(reader));
                }
            }

            con.Close();

            return locations;
        }

        public List<Location> GetAllUserLocations(long userId, bool byDate)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $"SELECT * FROM location WHERE user_id = {userId}",
                Connection = con
            };

            if (byDate)
                cmd.CommandText += " ORDER BY time ASC";

            con.Open();

            var locations = new List<Location>();

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    locations.Add(Location.Parse(reader));
                }
            }

            con.Close();

            return locations;
        }

        public List<Location> GetAllUserLocations(int userId, DayOfWeek weekday, bool byDate)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"SELECT * FROM location WHERE user_id = {userId} AND DATEPART(dw,time) = {weekday}",
                Connection = con
            };

            if (byDate)
                cmd.CommandText += " ORDER BY time ASC";

            try
            {
                con.Open();

                var locations = new List<Location>();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        locations.Add(Location.Parse(reader));
                    }
                }

                return locations;
            }
            finally
            {
                con.Close();
            }
        }

        public int InsertUserLocation(int userId, Location location)
        {
            //if (!isLocationRelevant(userId,location)) return 0;

            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"INSERT INTO location (user_id, latitude, longitude, time)
                                OUTPUT Inserted.Id
                                VALUES ({userId}, '{location.LatLng.Latitude}', '{location.LatLng.Longitude}', @Time)",
                Connection = con
            };

            cmd.Parameters.Add("@Time", SqlDbType.DateTime).Value = location.Timestamp;

            try
            {
                con.Open();
                return (int) cmd.ExecuteScalar();
            }
            finally
            {
                con.Close();
            }
        }

        //public static bool IsLocationRelevant(int userId, HomeLatLng location)
        //{
        //    foreach (var loc in GetAllUserLocations(userId, false))
        //    {
        //        var sameDay = (loc.Timestamp.DayOfWeek == location.Timestamp.DayOfWeek);
        //        if (!sameDay) return true;
        //        var sameTime = (Math.Abs(loc.Timestamp.TimeOfDay.Subtract(loc.Timestamp.TimeOfDay).TotalMinutes) < 10);
        //        if (!sameTime) return true;
        //        var fromCoord = location.LatLng.ToGeoCoordinate();
        //        var toCoord = loc.LatLng.ToGeoCoordinate();
        //        var distance = fromCoord.GetDistanceTo(toCoord);
        //        var sameLocation = distance < 500.0;
        //        if (!sameLocation) return true;
        //    }

        //    return false;
        //}
    }
}