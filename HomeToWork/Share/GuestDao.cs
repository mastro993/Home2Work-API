using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeToWork.Database;

namespace HomeToWork.Share
{
    public class GuestDao 
    {
        public Guest GetById(int shareId, int userId)
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
                        return Guest.Parse(reader);
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
                CommandText = $@"INSERT INTO guest (share_id, user_id, start_location) 
                                VALUES ({guest.ShareId}, {guest.User.Id}, '{guest.StartLatLng}')",
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
                                    end_location = '{guest.EndLatLng}',
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

        public List<Guest> GetAllByShareId(int shareId)
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
                        users.Add(Guest.Parse(reader));
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

