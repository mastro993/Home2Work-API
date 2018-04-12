using System;
using System.Collections.Generic;
using System.Text;
using domain.Entities;
using domain.Interfaces;

namespace data.Repositories
{
    public class LeaderboardsRepository : ILeaderboardsRepository
    {
        public List<UserRanking> GetUsersLeaderboard(Leaderboard.Type type, Leaderboard.Range range,
            Leaderboard.TimeSpan timeSpan)
        {
            throw new NotImplementedException();
        }

        public Rankings GetUserRanks(long userId, Leaderboard.Type type, Leaderboard.Range range,
            Leaderboard.TimeSpan timeSpan)
        {
            throw new NotImplementedException();
        }

        public List<CompanyRanking> GetCompaniesLeaderboards(Leaderboard.Type type, Leaderboard.TimeSpan timeSpan)
        {
            throw new NotImplementedException();
        }

        public Rankings GetCompanyRanks(long companyId, Leaderboard.Type type, Leaderboard.TimeSpan timeSpan)
        {
            throw new NotImplementedException();
        }
    }
}