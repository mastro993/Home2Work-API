using System;
using System.Data.SqlClient;
using data.Common;
using data.Common.Utils;
using data.Repositories;
using domain.Entities;
using domain.Interfaces;

namespace data.Mappers
{
    class MatchSqlMapper: Mapper<SqlDataReader, Match>
    {

        private readonly IUserRepository _userRepo = new UserRepository();

        public override Match MapFrom(SqlDataReader @from)
        {
            var matchId = @from["match_id"].ToLong();
            var userId = @from["user_id"].ToLong();
            var hostId =@from["host_id"].ToLong();
            var homeScore = @from["home_score"].ToInt();
            var jobScore =@from["job_score"].ToInt();
            var timeScore = @from["time_score"].ToInt();
            var arrivalTime = DateTime.ParseExact(@from["arrival_time"].ToString(), "HH:mm:ss", null);
            var departureTime = DateTime.ParseExact(@from["departure_time"].ToString(), "HH:mm:ss", null);
            var distance = @from["distance"].ToInt();
            var isNew = (bool)@from["new"];
            var isHidden = (bool)@from["hidden"];

            return new Match()
            {
                MatchId = matchId,
                UserId = userId,
                Host = _userRepo.GetById(hostId),
                HomeScore = homeScore,
                JobScore = jobScore,
                TimeScore = timeScore,
                ArrivalTime = arrivalTime,
                DepartureTime = departureTime,
                Distance = distance,
                IsNew = isNew,
                IsHidden = isHidden
            };
        }
    }
}
