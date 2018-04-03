using System;

namespace domain.Entities
{
    public class Match
    {
        public long MatchId { get; set; }
        public long UserId { get; set; }
        public User Host { get; set; }
        public int? HomeScore { get; set; }
        public int? JobScore { get; set; }
        public int? TimeScore { get; set; }
        public DateTime ArrivalTime { get; set; }
        public DateTime DepartureTime { get; set; }
        public int Distance { get; set; }
        public bool IsNew { get; set; }
        public bool IsHidden { get; set; }

    }
}