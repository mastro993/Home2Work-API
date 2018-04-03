using data.Common;
using data.Common.Utils;
using data.Database;
using data.Mappers;
using domain.Entities;
using domain.Interfaces;
using MlkPwgen;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Random = System.Random;

namespace data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly Mapper<SqlDataReader, User> _userMapper = new UserSqlMapper();
        private readonly Mapper<SqlDataReader, UserStats> _statsMapper = new UserStatsSqlMapper();
        private readonly Mapper<SqlDataReader, SharingActivity> _sharingActivityMapper = new SharingActivitySqlMapper();

        public int Register(string email, string password)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var salt = HashingUtils.Sha256(StringUtils.RandomString());
            var saltedPassword = HashingUtils.Sha256(password + salt);

            var cmd = new SqlCommand
            {
                CommandText = $@"INSERT INTO users (email, password, salt) 
                                OUTPUT Inserted.Id
                                VALUES ('{email}', '{saltedPassword}', '{salt}')",
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

        public User Login(string email, string password)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var salt = GetUserSalt(email);
            var saltedPassword = HashingUtils.Sha256(password + salt);

            var cmd = new SqlCommand
            {
                CommandText =
                    $@"SELECT * FROM users
                        LEFT JOIN users_data ON users.id = users_data.user_id  
                        WHERE email = '{email}' AND password = '{saltedPassword}'",
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
                        user = _userMapper.MapFrom(reader);
                    }
                }
            }
            finally
            {
                con.Close();
            }

            return user;
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

        public User GetById(long userId)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"SELECT * FROM users
                                 LEFT JOIN users_data ON users.id = users_data.user_id    
                            WHERE user_id = {userId}",
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
                        user = _userMapper.MapFrom(reader);
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
                CommandText = @"SELECT * FROM users LEFT JOIN users_data ON users.id = users_data.user_id",
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
                        users.Add(_userMapper.MapFrom(reader));
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
                CommandText = $@"UPDATE users_data SET 
                                    email = '{user.Email}', 
                                    name = '{user.Name}', 
                                    surname = '{user.Surname}', 
                                    home_lat = {user.Address.Latitude}, 
                                    home_lng = {user.Address.Longitude},
                                    street = '{user.Address.Street}',
                                    region = {user.Address.Region},
                                    city = '{user.Address.City}',
                                    cap = '{user.Address.PostalCode}',
                                    district = '{user.Address.District}',
                                    company_id = {user.Company.Id}
                                WHERE id = {user.Id}",
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

        public string NewSessionToken(long userId)
        {
            var con = new SqlConnection(Config.ConnectionString);
            //var randomString = StringUtils.RandomString();
            //var newToken = HashingUtils.Sha256(randomString);

            var newToken = PasswordGenerator.Generate(length: 64);

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
                CommandText = $@"SELECT * FROM users
                                LEFT JOIN users_data ON users.id = users_data.user_id 
                                WHERE session_token = '{sessionToken}'",
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
                        user = _userMapper.MapFrom(reader);
                    }
                }

                return user;
            }
            finally
            {
                con.Close();
            }
        }

        public UserExp GetUserExp(long userId)
        {
            // TODO exp utente

            var random = new Random().Next(1, 250000);
            var amount = (int) Math.Round(random / 10.0) * 10;

            return new UserExp(amount);
        }

        public bool AddExpToUser(long userId, long exp)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var cmd = new SqlCommand
            {
                CommandText = $@"UPDATE users_data SET 
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

        public UserStats GetUserStats(long userId)
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
                        stats = _statsMapper.MapFrom(reader);
                    }
                }
            }
            finally
            {
                con.Close();
            }

            return stats;
        }

        public Dictionary<int, SharingActivity> GetUserMonthlyActivity(long userId)
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

            var rawActivity = new Dictionary<int, SharingActivity>();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var sharingActivity = _sharingActivityMapper.MapFrom(reader);
                        rawActivity.Add(sharingActivity.Month, sharingActivity);
                    }
                }
            }
            finally
            {
                con.Close();
            }

            var activity = new Dictionary<int, SharingActivity>();
            var month = DateTime.Now.Month;

            for (var i = 0; i <= 5; i++)
            {
                var selectedActivity = rawActivity[month];
                activity.Add(month, selectedActivity);

                month -= 1;
                if (month == 0) month = 12;
            }

            activity.Reverse();
            return activity;
        }
    }
}