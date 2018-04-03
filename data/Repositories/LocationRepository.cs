using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using data.Common;
using data.Database;
using data.Mappers;
using domain.Entities;
using domain.Interfaces;

namespace data.Repositories
{
    public class LocationRepository: ILocationRespository
    {
        private readonly Mapper<SqlDataReader, Location> _mapper = new LocationSqlMapper();

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
                    locations.Add(_mapper.MapFrom(reader));
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
                    locations.Add(_mapper.MapFrom(reader));
                }
            }

            con.Close();

            return locations;
        }

        public List<Location> GetAllUserLocations(long userId, DayOfWeek weekday, bool byDate)
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
                        locations.Add(_mapper.MapFrom(reader));
                    }
                }

                return locations;
            }
            finally
            {
                con.Close();
            }
        }

        public int InsertUserLocation(long userId, Location location)
        {
            //if (!isLocationRelevant(userId,location)) return 0;

            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"INSERT INTO location (user_id, latitude, longitude, time)
                                OUTPUT Inserted.Id
                                VALUES ({userId}, '{location.Latitude}', '{location.Longitude}', @Time)",
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