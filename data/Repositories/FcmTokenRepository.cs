using System.Data.SqlClient;
using data.Database;
using domain.Interfaces;

namespace data.Repositories
{
    public class FcmTokenRepository : IFirebaseRepository
    {
        public string GetUserToken(long userId)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $"get_user_fcm_token {userId}",
                Connection = con
            };


            con.Open();

            string token = null;

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    token = reader["token"].ToString();
                }
            }

            con.Close();

            return token;
        }

        public bool SetUserToken(long userId, string token)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $"set_user_fcm_token {userId}, '{token}'",
                Connection = con
            };


            con.Open();

            try
            {
                return cmd.ExecuteNonQuery() > 0;
            }
            finally
            {
                con.Close();
            }
        }

        public bool UpdateUserToken(long userId, string token)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $"update_user_fcm_token {userId}, '{token}'",
                Connection = con
            };


            con.Open();

            try
            {
                return cmd.ExecuteNonQuery() > 0;
            }
            finally
            {
                con.Close();
            }
        }
    }
}