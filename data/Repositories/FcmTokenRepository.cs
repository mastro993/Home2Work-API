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
                CommandText = $@"SELECT token FROM fcm_token WHERE user_id = {userId}",
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
                CommandText = $@"INSERT INTO fcm_token (user_id, token) 
                                    VALUES ({userId}, '{token}')",
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
                CommandText = $@"UPDATE fcm_token 
                                 SET token = '{token}' 
                                WHERE user_id = {userId}",
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