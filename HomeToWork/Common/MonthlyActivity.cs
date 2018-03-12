using System;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace HomeToWork.Common
{
    [Serializable]
    public class MonthlyActivity
    {
        public int Month { get; set; }
        public int Shares { get; set; }
        public float SharesAvg { get; set; }
        public int Distance { get; set; }
        public float DistanceAvg { get; set; }

        public static MonthlyActivity Parse(SqlDataReader reader)
        {
            var month = (int) reader["month"];
            var shares = (int) reader["shares"];
            var distance = (int) reader["distance"];
            return new MonthlyActivity()
            {
                Month = month,
                Shares = shares,
                SharesAvg = 0f,
                Distance = distance,
                DistanceAvg = 0f
            };
        }
    }
}