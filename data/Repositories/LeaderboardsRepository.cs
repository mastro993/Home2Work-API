using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using data.Common;
using data.Database;
using data.Mappers;
using domain.Entities;
using domain.Interfaces;

namespace data.Repositories
{
    public class LeaderboardsRepository : ILeaderboardsRepository
    {
        private readonly Mapper<SqlDataReader, UserRanking> _userRanksMapper = new UserRankingSqlMapper();

        public List<UserRanking> GetUsersGlobalLeaderboard(Leaderboard.Type type, Leaderboard.TimeSpan timeSpan,
            int page, int limit)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $"get_users_leaderboard_global_{timeSpan}_{type} {limit}, {page}",
                Connection = con
            };

            con.Open();

            var ranks = new List<UserRanking>();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ranks.Add(_userRanksMapper.MapFrom(reader));
                    }
                }
            }
            finally
            {
                con.Close();
            }

            con.Close();

            return ranks;
        }

        public List<UserRanking> GetUsersCompanyLeaderboard(long? companyId, Leaderboard.Type type,
            Leaderboard.TimeSpan timeSpan, int page, int limit)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $"get_users_leaderboard_company_{timeSpan}_{type} {companyId}, {limit}, {page}",
                Connection = con
            };

            con.Open();

            var ranks = new List<UserRanking>();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ranks.Add(_userRanksMapper.MapFrom(reader));
                    }
                }
            }
            finally
            {
                con.Close();
            }

            return ranks;
        }


        public Rankings GetUserRanks(
            long userId,
            Leaderboard.Type type,
            Leaderboard.Range range,
            Leaderboard.TimeSpan timeSpan,
            int page,
            int limit)
        {
            throw new NotImplementedException();
        }

        public List<CompanyRanking> GetCompaniesLeaderboards(
            Leaderboard.Type type,
            Leaderboard.TimeSpan timeSpan,
            int page,
            int limit)
        {
            throw new NotImplementedException();
        }

        public Rankings GetCompanyRanks(
            long companyId,
            Leaderboard.Type type,
            Leaderboard.TimeSpan timeSpan,
            int page,
            int limit)
        {
            throw new NotImplementedException();
        }
    }
}