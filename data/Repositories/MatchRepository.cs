using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using data.Common;
using data.Common.Utils;
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
                    $"insert_user_match {match.UserId}, {match.Host.Id}, {match.HomeScore}, {match.JobScore}, {match.TimeScore}, {match.ArrivalTime.TimeOfDay}, {match.DepartureTime.TimeOfDay}, {match.Distance}",
                Connection = con
            };

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
                CommandText = $"get_user_matches {userId}",
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

        public Match GetById(int matchId)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $"get_match {matchId}",
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
                CommandText =
                    $"edit_match {match.MatchId}, {Convert.ToInt32(match.IsNew)}, {Convert.ToInt32(match.IsHidden)}",
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