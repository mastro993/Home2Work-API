using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using data.Common;
using data.Database;
using data.Mappers;
using domain.Entities;
using domain.Interfaces;

namespace data.Repositories
{
    public class LocationRepository : ILocationRespository
    {
        private readonly Mapper<SqlDataReader, Location> _mapper = new LocationSqlMapper();

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

        public bool InsertUserSCLLocation(long userId, double latitude, double longitude, DateTime date)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var cmd = new SqlCommand
            {
                CommandText =
                    $@"insert_user_scl_location {userId}, {latitude.ToString(CultureInfo.InvariantCulture)}, {
                            longitude.ToString(CultureInfo.InvariantCulture)
                        }, @Date",
                Connection = con
            };

            cmd.Parameters.Add("@Date", SqlDbType.DateTime).Value = date;

            try
            {
                con.Open();
                var rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
            catch (Exception e)
            {
                return false;
            }
            finally
            {
                con.Close();
            }
        }

        public bool InsertUserLastLocation(long userId, double latitude, double longitude, DateTime date)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var cmd = new SqlCommand
            {
                CommandText =
                    $@"insert_user_last_location {userId}, {latitude.ToString(CultureInfo.InvariantCulture)}, {
                            longitude.ToString(CultureInfo.InvariantCulture)
                        }, @Date",
                Connection = con
            };

            cmd.Parameters.Add("@Date", SqlDbType.DateTime).Value = date;

            try
            {
                con.Open();
                var rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
            catch (Exception e)
            {
                return false;
            }
            finally
            {
                con.Close();
            }
        }
    }
}