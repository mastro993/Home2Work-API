using System.Collections.Generic;
using System.Data.SqlClient;
using data.Common;
using data.Database;
using data.Mappers;
using domain.Entities;
using domain.Interfaces;

namespace data.Repositories
{
    public class ShareRepository : IShareRepository
    {
        private readonly Mapper<SqlDataReader, Share> _shareMapper = new ShareSqlMapper();
        private readonly Mapper<SqlDataReader, Guest> _guestMapper = new GuestSqlMapper();

        public List<Share> GetUserShares(long userId, int? page, int? limit)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $"get_user_shares {userId}, {limit}, {page}",
                Connection = con
            };


            con.Open();

            var shares = new List<Share>();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var share = _shareMapper.MapFrom(reader);
                        share.Guests = GetShareGuests(share.Id);
                        share.Type = share.Host.Id == userId ? Type.Driver : Type.Guest;

                        shares.Add(share);
                    }
                }
            }
            finally
            {
                con.Close();
            }

            return shares;
        }

        public Share GetUserShare(long userId, long shareId)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var cmd = new SqlCommand
            {
                CommandText = $"get_share {shareId}",
                Connection = con
            };


            con.Open();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var share = _shareMapper.MapFrom(reader);
                        var guests = GetShareGuests(share.Id);
                        share.Guests = guests;
                        share.Type = share.Host.Id == userId ? Type.Driver : Type.Guest;
                        return share;
                    }
                }
            }
            finally
            {
                con.Close();
            }

            return null;
        }

        public Share GetUserActiveShare(long userId)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var cmd = new SqlCommand
            {
                CommandText = $@"get_user_current_share {userId}",
                Connection = con
            };


            con.Open();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var share = _shareMapper.MapFrom(reader);
                        var guests = GetShareGuests(share.Id);
                        share.Guests = guests;
                        share.Type = share.Host.Id == userId ? Type.Driver : Type.Guest;
                        return share;
                    }
                }
            }
            finally
            {
                con.Close();
            }

            return null;
        }

        public long CreateShare(long hostId, double latitude, double longitude)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"create_share {hostId}, '{latitude.ToString().Replace(",",".")}', '{longitude.ToString().Replace(",", ".")}'",
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

        public bool FinishShare(long shareId, double latitude, double longitude, int distance)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"finish_share {shareId}, '{latitude.ToString().Replace(",", ".")}', '{longitude.ToString().Replace(",", ".")}', {distance}",
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

        public bool CancelShare(long shareId)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"cancel_share {shareId}",
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

        public bool Delete(long shareId)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"DELETE FROM share WHERE id = {shareId}",
                Connection = con
            };

            con.Open();

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

        public Guest GetGuest(long shareId, long userId)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"get_share_guest {shareId}, {userId}",
                Connection = con
            };

            con.Open();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return _guestMapper.MapFrom(reader);
                    }
                }
            }
            finally
            {
                con.Close();
            }

            return null;
        }

        public bool JoinShare(long shareId, long guestId, double latitude, double longitude)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText =
                    $@"insert_share_guest {shareId}, {guestId}, '{latitude.ToString().Replace(",", ".")}', '{longitude.ToString().Replace(",", ".")}'",
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

        public bool CompleteShare(long shareId, long userId, double latitude, double longitude, int distance)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"complete_share {shareId}, {userId}, '{latitude.ToString().Replace(",", ".")}', '{longitude.ToString().Replace(",", ".")}', {distance}",
                Connection = con
            };

            cmd.Connection = con;
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

        public bool LeaveShare(long shareId, long guestId)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"leave_share {shareId}, {guestId}",
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

        public List<Guest> GetShareGuests(long shareId)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $"get_share_guests {shareId}",
                Connection = con
            };

            con.Open();

            var users = new List<Guest>();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(_guestMapper.MapFrom(reader));
                    }
                }
            }
            finally
            {
                con.Close();
            }

            return users;
        }
    }
}