using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeToWork.Database;

namespace HomeToWork.Match
{
    public class MatchDao
    {
        public void Insert(Match match)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText =
                    @"INSERT INTO match (host_id, guest_id, weekdays, score, distance, start_location, start_time, end_location, end_time) 
                                VALUES (@HostID, @GuestID, @Weekdays, @Score, @Distance, @StartLatLng, @StartTime, @EndLatLng, @EndTime)"
            };


            cmd.Parameters.Add("@HostID", SqlDbType.Int).Value = match.Host.Id;
            cmd.Parameters.Add("@GuestID", SqlDbType.Int).Value = match.Guest.Id;
            cmd.Parameters.Add("@Weekdays", SqlDbType.VarChar).Value =
                string.Join(",", match.Weekdays.Select(w => ((int) w).ToString()).ToArray());
            cmd.Parameters.Add("@Score", SqlDbType.Int).Value = match.Score;
            cmd.Parameters.Add("@Distance", SqlDbType.Int).Value = match.Distance;
            cmd.Parameters.Add("@StartLatLng", SqlDbType.VarChar).Value = match.StartLocation.ToString();
            cmd.Parameters.Add("@StartTime", SqlDbType.Time).Value = match.StartTime.TimeOfDay;
            cmd.Parameters.Add("@EndLatLng", SqlDbType.VarChar).Value = match.EndLocation.ToString();
            cmd.Parameters.Add("@EndTime", SqlDbType.Time).Value = match.EndTime.TimeOfDay;
            cmd.Parameters.Add("@New", SqlDbType.Bit).Value = 1;

            cmd.Connection = con;

            con.Open();

            cmd.ExecuteNonQuery();

            con.Close();
        }

        public List<Match> GetByUserId(int userId)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"SELECT * FROM match WHERE guest_id = {userId} ORDER BY score DESC",
                Connection = con
            };

            cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

            con.Open();

            var matches = new List<Match>();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        matches.Add(Match.Parse(reader));
                    }
                }
            }
            finally
            {
                con.Close();
            }


            return matches;
        }

        public Match GetById(int matchId)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"SELECT * FROM match WHERE id = {matchId}",
                Connection = con
            };

            con.Open();

            Match match = null;

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        match = Match.Parse(reader);
                    }
                }
            }
            finally
            {
                con.Close();
            }


            return match;
        }


        public Match EditMatch(Match match)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"UPDATE match SET 
                                    new = {Convert.ToInt32(match.New)}, 
                                    hidden = {Convert.ToInt32(match.Hidden)}
                                WHERE id = {match.MatchId}",
                Connection = con
            };

            con.Open();

            int rows;

            try
            {
                rows = cmd.ExecuteNonQuery();
            }
            finally
            {
                con.Close();
            }

            return rows > 0 ? match : null;
        }
    }
}