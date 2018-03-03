using System;
using System.Data.SqlClient;

namespace HomeToWork.Common
{
    [Serializable]
    public class MonthlyActivity
    {
        public int Month { get; set; }
        public int Shares { get; set; }
        public int Distance { get; set; }

        public static MonthlyActivity Parse(SqlDataReader reader)
        {
            var month = (int) reader["month"];
            var shares = (int) reader["shares"];
            var distance = (int) reader["distance"];
            return new MonthlyActivity()
            {
                Month = month,
                Shares = shares,
                Distance = distance,
            };
        }
    }
}