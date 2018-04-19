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
        private readonly Mapper<SqlDataReader, ProfileStatus> _userStatusMapper = new ProfileStatusSqlMapper();

        public User Login(string email, string passwordHash)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var cmd = new SqlCommand
            {
                CommandText = $"user_login '{email}', '{passwordHash}'",
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
                CommandText = $@"get_user_salt '{email}'",
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
                CommandText = $"get_user {userId}",
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

        public UserProfile GetProfileById(long userId)
        {
            var status = GetUserStatus(userId);
            var user = GetById(userId);
            var karma = GetUserKarma(userId);
            var stats = GetUserStats(userId);
            var activity = GetUserMonthlyActivity(userId);

            var profile = new UserProfile()
            {
                Status = status,
                Karma = karma,
                Stats = stats,
                Activity = activity,
                Regdate = user.Regdate
            };

            return profile;
        }

        public List<User> GetAll()
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = "SELECT * FROM users",
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

        public string NewSessionToken(long userId)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var newToken = PasswordGenerator.Generate(length: 64);

            var cmd = new SqlCommand
            {
                CommandText = $"set_session_token {userId}, '{newToken}'",
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
                CommandText = $"get_user_from_session '{sessionToken}'",
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

        public UserKarma GetUserKarma(long userId)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var cmd = new SqlCommand
            {
                CommandText = $"get_user_karma {userId}",
                Connection = con
            };

            con.Open();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var amount = reader["total_karma"].ToInt();
                        var monthAmount = reader["month_karma"].ToInt();
                        var karma = new UserKarma(amount) {MonthKarma = monthAmount};
                        return karma;
                    }
                }

                return null;
            }
            finally
            {
                con.Close();
            }
        }

        public bool AddExpToUser(long userId, long exp)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var cmd = new SqlCommand
            {
                CommandText = $"add_user_exp {userId}, {exp}",
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
                CommandText = $"get_user_statistics {userId}",
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
                CommandText = $"get_user_monthly_activity {userId}",
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

        public bool UpdateUserStatus(long userId, string status)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $"update_user_status {userId}, '{status}'",
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

            return rows > 0;
        }

        public ProfileStatus GetUserStatus(long userId)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var cmd = new SqlCommand
            {
                CommandText = $"get_user_status {userId}",
                Connection = con
            };

            con.Open();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return _userStatusMapper.MapFrom(reader);
                    }
                }

                return null;
            }
            finally
            {
                con.Close();
            }
        }
    }
}