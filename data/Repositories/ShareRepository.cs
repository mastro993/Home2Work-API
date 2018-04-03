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

        public List<Share> GetUserShares(long userId)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"SELECT * FROM share s
                                WHERE (s.host_id = {userId} AND s.status = 1)
                                    OR (s.id IN 
                                        (SELECT g.share_id FROM guest g 
                                        WHERE g.user_id = {userId} AND g.status = 1)    
                                    AND s.status = 1)
                                ORDER BY s.time DESC",
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
                        share.Type = share.Host.Id == userId ? ShareType.Driver : ShareType.Guest;

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

        public Share GetShare(long id)
        {
            var con = new SqlConnection(Config.ConnectionString);

            var cmd = new SqlCommand
            {
                CommandText = $"SELECT * FROM share WHERE id = {id}",
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
                CommandText = $@"SELECT *
                                 FROM share s
                                 WHERE (s.host_id = {userId}
                                 OR s.id IN (
                                    SELECT g.share_id
                                    FROM guest g
                                    WHERE g.user_id = {userId} AND g.status = {(int) ShareStatus.Created})
                                 ) AND s.status = {(int) ShareStatus.Created}",
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

        public int Insert(Share share)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"INSERT INTO share (host_id) 
                                OUTPUT Inserted.Id
                                VALUES ({share.Host.Id})",
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

        public Share Edit(Share share)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"UPDATE share SET 
                                    status = {share.Status}
                                WHERE id = {share.Id}",
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

            return rows > 0 ? share : null;
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

        public Guest GetGuestById(long shareId, long userId)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"SELECT * FROM guest 
                                WHERE share_id = {shareId} AND user_id = {userId}",
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

        public void Insert(Guest guest)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"INSERT INTO guest (share_id, user_id, start_latitude, start_longitude) 
                                VALUES ({guest.ShareId}, {guest.User.Id}, {guest.StartLat}, {guest.StartLng})",
                Connection = con
            };

            con.Open();

            try
            {
                cmd.ExecuteNonQuery();
            }
            finally
            {
                con.Close();
            }
        }

        public Guest Complete(Guest guest)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"UPDATE guest SET 
                                    end_latitude = {guest.EndLat},
                                    end_longitude = {guest.EndLng},
                                    status = 1,
                                    distance = {guest.Distance}
                                WHERE share_id = {guest.ShareId} AND user_id = {guest.User.Id}",
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

            return rows > 0 ? guest : null;
        }

        public Guest Edit(Guest guest)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"UPDATE guest SET 
                                    status = {guest.Status}
                                WHERE share_id = {guest.ShareId} AND user_id = {guest.User.Id}",
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

            return rows > 0 ? guest : null;
        }

        public List<Guest> GetShareGuests(long shareId)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"SELECT * FROM guest WHERE share_id = {shareId}",
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