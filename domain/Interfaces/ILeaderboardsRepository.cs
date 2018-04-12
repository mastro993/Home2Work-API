using System;
using System.Collections.Generic;
using System.Text;
using domain.Entities;

namespace domain.Interfaces
{
    public interface ILeaderboardsRepository
    {
        List<UserRanking> GetUsersLeaderboard(Leaderboard.Type type, Leaderboard.Range range, Leaderboard.TimeSpan timeSpan);
        Rankings GetUserRanks(long userId, Leaderboard.Type type, Leaderboard.Range range, Leaderboard.TimeSpan timeSpan);
        List<CompanyRanking> GetCompaniesLeaderboards(Leaderboard.Type type, Leaderboard.TimeSpan timeSpan);
        Rankings GetCompanyRanks(long companyId, Leaderboard.Type type, Leaderboard.TimeSpan timeSpan);
    }
}