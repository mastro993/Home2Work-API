using System;
using System.Data.SqlClient;
using HomeToWork.Database;

namespace HomeToWork.User
{
    public class UserExpDao
    {
        public UserExp GetUserExp(long userId)
        {
            // TODO exp utente

            var random = new Random().Next(1, 250000);
            var amount = (int) Math.Round(random / 10.0) * 10;

            return new UserExp(amount);
        }

        public bool AddExpToUser(int userId, int exp)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var cmd = new SqlCommand
            {
                CommandText = $@"UPDATE users SET 
                                    exp = {exp}
                                WHERE id = {userId}",
                Connection = con
            };

            con.Open();

            try
            {
                var rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
            finally
            {
                con.Close();
            }
        }
    }
}