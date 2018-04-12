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
            Weekly, 
            Daily
        }

        public enum Range
        {
            Global,
            Company,
            Regional
        }

        public enum Type
        {
            Shares,
            SharesAvg,
            SharedDistance,
            SharedDistanceAvg,
            LongestShare
        }

    }
}