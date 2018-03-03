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
    [Serializable]
    public class Match
    {
        public int MatchId { get; set; }
        public User.User Host { get; set; }
        public User.User Guest { get; set; }
        public List<DayOfWeek> Weekdays { get; set; }
        public int Score { get; set; }
        public int Distance { get; set; }
        public LatLng StartLocation { get; set; }
        public DateTime StartTime { get; set; }
        public LatLng EndLocation { get; set; }
        public DateTime EndTime { get; set; }
        public bool New { get; set; }
        public bool Hidden { get; set; }

        public static Match Parse(SqlDataReader reader)
        {
            var weekdays = reader["weekdays"]
                .ToString().Split(',').ToList()
                .Select(w => (DayOfWeek) int.Parse(w)).ToList();

            var matchId = int.Parse(reader["id"].ToString());
            var hostId = (int) reader["host_id"];
            var guestId = (int) reader["guest_id"];
            var score = (int) reader["score"];
            var distance = int.Parse(reader["distance"].ToString());
            var startLatLng = LatLng.Parse(reader["start_location"].ToString());
            var startTime = DateTime.ParseExact(reader["start_time"].ToString(), "HH:mm:ss", null);
            var endLatLng = LatLng.Parse(reader["end_location"].ToString());
            var endTime = DateTime.ParseExact(reader["end_time"].ToString(), "HH:mm:ss", null);
            var isNew = (bool) reader["new"];
            var isHidden = (bool) reader["hidden"];

            var userDao = new UserDao();

            return new Match()
            {
                MatchId = matchId,
                Host = userDao.GetById(hostId),
                Guest = userDao.GetById(guestId),
                Weekdays = weekdays,
                Score = score,
                Distance = distance,
                StartLocation = startLatLng,
                StartTime = startTime,
                EndLocation = endLatLng,
                EndTime = endTime,
                New = isNew,
                Hidden = isHidden
            };
        }
    }
}