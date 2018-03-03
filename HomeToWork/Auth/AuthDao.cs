using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeToWork.Database;
using HomeToWork.User;

namespace HomeToWork.Auth
{
    public class AuthDao
    {
        public int Register(string email, string password, string salt)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var cmd = new SqlCommand
            {
                CommandText = $@"INSERT INTO users (email, password, salt) 
                                OUTPUT Inserted.Id
                                VALUES ('{email}', '{password}', '{salt}')",
                Connection = con
            };

            con.Open();

            try
            {
                return (int) cmd.ExecuteScalar();
            }
            finally
            {
                con.Close();
            }
        }

        public AuthUser Login(string email, string password)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var cmd = new SqlCommand
            {
                CommandText =
                    $@"SELECT * FROM users WHERE email = '{email}' AND password = '{password}'",
                Connection = con
            };

            con.Open();

            AuthUser user = null;

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        user = AuthUser.Parse(reader);
                    }
                }
            }
            finally
            {
                con.Close();
            }

            return user;
        }

        public AuthUser Login(string token)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var cmd = new SqlCommand
            {
                CommandText = $@"SELECT * FROM users WHERE access_token = '{token}'",
                Connection = con
            };

            con.Open();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return AuthUser.Parse(reader);
                    }
                }
            }
            finally
            {
                con.Close();
            }

            return null;
        }

        public string GetUserSalt(string email)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var cmd = new SqlCommand
            {
                CommandText = $@"SELECT salt FROM users WHERE email = '{email}'",
                Connection = con
            };

            con.Open();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return reader["salt"].ToString();
                    }
                }
            }
            finally
            {
                con.Close();
            }

            return null;
        }
    }
}