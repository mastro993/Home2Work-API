using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeToWork.Common;
using HomeToWork.User;

namespace HomeToWork.Match
{
    public class Match
    {
        public long MatchId { get; set; }
        public long UserId { get; set; }
        public User.User Host { get; set; }
        public int HomeScore { get; set; }
        public int JobScore { get; set; }
        public int TimeScore { get; set; }
        public DateTime ArrivalTime { get; set; }
        public DateTime DepartureTime { get; set; }
        public int Distance { get; set; }
        public bool IsNew { get; set; }
        public bool IsHidden { get; set; }

        public static Match Parse(SqlDataReader reader)
        {
            var matchId = int.Parse(reader["match_id"].ToString());
            var userId = int.Parse(reader["user_id"].ToString());
            var hostId = (int) reader["host_id"];
            var homeScore = (int) reader["home_score"];
            var jobScore = (int) reader["job_score"];
            var timeScore = (int) reader["time_score"];
            var arrivalTime = DateTime.ParseExact(reader["arrival_time"].ToString(), "HH:mm:ss", null);
            var departureTime = DateTime.ParseExact(reader["departure_time"].ToString(), "HH:mm:ss", null);
            var distance = (int) reader["distance"];
            var isNew = (bool) reader["new"];
            var isHidden = (bool) reader["hidden"];

            var userDao = new UserDao();

            return new Match()
            {
                MatchId = matchId,
                UserId = userId,
                Host = userDao.GetById(hostId),
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