using System;
using System.Collections.Generic;
using System.Text;
using domain.Entities;

namespace domain.Interfaces
{
    public interface ILeaderboardsRepository
    {
        List<UserRanking> GetUsersGlobalLeaderboard(
            Leaderboard.Type type,
            Leaderboard.TimeSpan timeSpan,
            int page,
            int limit
        );

        List<UserRanking> GetUsersCompanyLeaderboard(
            long? companyId,
            Leaderboard.Type type,
            Leaderboard.TimeSpan timeSpan,
            int page, 
            int limit
        );

        Rankings GetUserRanks(
            long userId,
            Leaderboard.Type type,
            Leaderboard.Range range,
            Leaderboard.TimeSpan timeSpan,
            int page,
            int limit
        );

        List<CompanyRanking> GetCompaniesLeaderboards(
            Leaderboard.Type type,
            Leaderboard.TimeSpan timeSpan,
            int page,
            int limit
        );

        Rankings GetCompanyRanks(
            long companyId,
            Leaderboard.Type type,
            Leaderboard.TimeSpan timeSpan,
            int page,
            int limit
        );
    }
}