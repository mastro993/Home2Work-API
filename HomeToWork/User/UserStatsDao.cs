using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeToWork.Common;
using HomeToWork.Database;

namespace HomeToWork.User
{
    public class UserStatsDao
    {
        public UserStats GetUserStats(int userId)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"SELECT *
                                FROM user_statistics US
                                INNER JOIN user_ranks UR ON UR.user_id = US.user_id
                                WHERE UR.user_id = {userId}",
                Connection = con
            };


            con.Open();

            UserStats stats = null;

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stats = UserStats.Parse(reader);
                    }
                }
            }
            finally
            {
                con.Close();
            }

            return stats;
        }

        public List<MonthlyActivity> GetUserMonthlyActivity(int userId)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"WITH months AS
                                (
                                  SELECT 1 AS Month
                                  UNION ALL
                                  SELECT Month + 1
                                  FROM months
                                  WHERE Month < 12
                                )

                                SELECT
                                  M.Month,
                                  COALESCE(A.shares, 0) as shares,
                                  COALESCE(A.distance, 0) as distance
                                FROM (SELECT
                                        MONTH(time)            AS month,
                                        SUM(distance)          AS distance,
                                        Count(DISTINCT (S.id)) AS shares
                                      FROM share S
                                        LEFT JOIN guest G ON S.id = G.share_id
                                      WHERE (G.user_id = {userId} OR S.host_id = {userId}) 
                                        AND G.status = 1 AND S.status = 1 AND
                                            dbo.FullMonthsSeparation(S.time, getdate()) <= 6
                                      GROUP BY MONTH(time), YEAR(time)) A
                                  RIGHT JOIN months m ON A.month = M.month",
                Connection = con
            };


            con.Open();

            var rawActivity = new List<MonthlyActivity>();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rawActivity.Add(MonthlyActivity.Parse(reader));
                    }
                }
            }
            finally
            {
                con.Close();
            }

            var activity = new List<MonthlyActivity>();
            var thisMonth = DateTime.Now.Month;

            for (var i = 0; i <= 5; i++)
            {
                var selectedActivity = rawActivity.Find(e => e.Month == thisMonth);
                activity.Add(selectedActivity);

                thisMonth -= 1;
                if (thisMonth == 0) thisMonth = 12;
            }

            activity.Reverse();
            return activity;
        }
    }
}