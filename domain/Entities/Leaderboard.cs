using System;
using System.Collections.Generic;
using System.Text;

namespace domain.Entities
{
    public class Leaderboard
    {
        public enum TimeSpan
        {
            AllTime,
            Monthly,
            Weekly
        }

        public enum Range
        {
            Global,
            Company
        }

        public enum Type
        {
            Shares,
            Distance
        }
    }
}