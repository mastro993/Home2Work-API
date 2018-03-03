using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeToWork.Database;
using HomeToWork.Utils;

namespace HomeToWork.User
{
    public class UserDao
    {
        public User GetById(int userId)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"SELECT * FROM users WHERE id = {userId}",
                Connection = con
            };


            con.Open();

            User user = null;

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        user = User.Parse(reader);
                    }
                }

                return user;
            }
            finally
            {
                con.Close();
            }
        }

        public List<User> GetAll()
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = @"SELECT * FROM users",
                Connection = con
            };


            con.Open();

            var users = new List<User>();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(User.Parse(reader));
                    }
                }
            }
            finally
            {
                con.Close();
            }

            return users;
        }

        public User Edit(User user)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"UPDATE users SET 
                                    email = {user.Email}, 
                                    name = {user.Name}, 
                                    surname = {user.Surname}, 
                                    location = {user.HomeLatLng}, 
                                    cap = {user.Address.Cap}, 
                                    city = {user.Address.City}, 
                                    address = {user.Address.AddressLine}, 
                                    company_id = {user.Company.Id}
                                WHERE id = @UserId",
                Connection = con
            };

            con.Open();

            int rows;

            try
            {
                rows = cmd.ExecuteNonQuery();
            }
            finally
            {
                con.Close();
            }

            return rows > 0 ? user : null;
        }

        public User Configure(User user)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"UPDATE users SET 
                                    name = {user.Name}, 
                                    surname = {user.Surname}, 
                                    location = {user.HomeLatLng}, 
                                    cap = {user.Address.Cap}, 
                                    city = {user.Address.City}, 
                                    address = {user.Address.AddressLine}, 
                                    company_id = {user.Company.Id},
                                    configured = 1,
                                WHERE id = @UserId",
                Connection = con
            };

            con.Open();

            int rows;

            try
            {
                rows = cmd.ExecuteNonQuery();
            }
            finally
            {
                con.Close();
            }

            return rows > 0 ? user : null;
        }

        public bool EditPassword(int userId, string newPassword, string salt)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"UPDATE users SET 
                                    password = '{newPassword}', 
                                    salt = '{salt}' 
                                WHERE id = {userId}",
                Connection = con
            };


            cmd.Connection = con;

            var rows = 0;

            try
            {
                rows = cmd.ExecuteNonQuery();
            }
            finally
            {
                con.Close();
            }

            return rows > 0;
        }

        public string NewAccessToken(int userId)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var randomString = StringUtils.RandomString();
            var newToken = HashingUtils.Sha256(randomString);

            var cmd = new SqlCommand
            {
                CommandText = $@"UPDATE users SET access_token = '{newToken}' WHERE id = {userId}",
                Connection = con
            };

            con.Open();

            try
            {
                cmd.ExecuteNonQuery();
                return newToken;
            }
            finally
            {
                con.Close();
            }
        }

        public string NewSessionToken(int userId)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var randomString = StringUtils.RandomString();
            var newToken = HashingUtils.Sha256(randomString);

            var cmd = new SqlCommand
            {
                CommandText = $@"UPDATE users SET session_token = '{newToken}' WHERE id = {userId}",
                Connection = con
            };

            con.Open();

            try
            {
                cmd.ExecuteNonQuery();
                return newToken;
            }
            finally
            {
                con.Close();
            }
        }

        public User GetBySessionToken(string sessionToken)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"SELECT * FROM users WHERE session_token = '{sessionToken}'",
                Connection = con
            };


            con.Open();

            User user = null;

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        user = User.Parse(reader);
                    }
                }

                return user;
            }
            finally
            {
                con.Close();
            }
        }
    }
}