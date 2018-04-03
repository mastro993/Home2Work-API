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
    public class MatchRepository : IMatchRepository
    {
        private readonly Mapper<SqlDataReader, Match> _matchMapper = new MatchSqlMapper();

        public void Insert(Match match)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText =
                    @"INSERT INTO match (user_id, host_id, home_score, job_score, time_score, arrival_time, departure_time, new, hidden, distance) 
                                VALUES (@UserId, @HostId, @HomeScore, @JobScore, @TimeScore, @ArrivalTime, @DepartureTime, @New, @Hidden, @Distance)"
            };


            cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = match.UserId;
            cmd.Parameters.Add("@HostId", SqlDbType.Int).Value = match.Host.Id;
            cmd.Parameters.Add("@HomeScore", SqlDbType.Int).Value = match.HomeScore;
            cmd.Parameters.Add("@JobScore", SqlDbType.Int).Value = match.JobScore;
            cmd.Parameters.Add("@TimeScore", SqlDbType.Int).Value = match.TimeScore;
            cmd.Parameters.Add("@ArrivalTime", SqlDbType.Time).Value = match.ArrivalTime.TimeOfDay;
            cmd.Parameters.Add("@DepartureTime", SqlDbType.Time).Value = match.DepartureTime.TimeOfDay;
            cmd.Parameters.Add("@New", SqlDbType.Bit).Value = 1;
            cmd.Parameters.Add("@Hidden", SqlDbType.Bit).Value = 0;
            cmd.Parameters.Add("@Distance", SqlDbType.Int).Value = match.Distance;

            cmd.Connection = con;

            con.Open();

            cmd.ExecuteNonQuery();

            con.Close();
        }

        public List<Match> GetAll()
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"SELECT * FROM match",
                Connection = con
            };

            con.Open();

            var matches = new List<Match>();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        matches.Add(_matchMapper.MapFrom(reader));
                    }
                }
            }
            finally
            {
                con.Close();
            }


            return matches;
        }

        public List<Match> GetByUserId(long userId)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"SELECT * FROM match WHERE user_id = {userId} AND hidden=0",
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
                        matches.Add(_matchMapper.MapFrom(reader));
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
                CommandText = $@"SELECT * FROM match WHERE match_id = {matchId}",
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
                        match = _matchMapper.MapFrom(reader);
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
                                    new = {Convert.ToInt32(match.IsNew)}, 
                                    hidden = {Convert.ToInt32(match.IsHidden)}
                                WHERE match_id = {match.MatchId}",
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