using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeToWork.Database;

namespace HomeToWork.Share
{
    public class ShareDao
    {
        public List<Share> GetByUserID(int userId)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"SELECT * FROM share s
                                WHERE (s.host_id = {userId} AND s.status != 2)
                                    OR (s.id IN 
                                        (SELECT g.share_id FROM guest g 
                                        WHERE g.user_id = {userId} AND g.status != 2)    
                                    AND s.status != 2)
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
                        var share = Share.Parse(reader);
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

        public Share GetOngoinByHostId(int hostId)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"SELECT * FROM share 
                                WHERE host_id = {hostId} AND status = 0",
                Connection = con
            };


            con.Open();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return Share.Parse(reader);
                    }
                }
            }
            finally
            {
                con.Close();
            }

            return null;
        }

        public Share GetOngoinByUserId(int userId)
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
                                    WHERE g.user_id = {userId} AND g.status = {Share.Created})
                                 ) AND s.status = {Share.Created}",
                Connection = con
            };


            con.Open();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return Share.Parse(reader);
                    }
                }
            }
            finally
            {
                con.Close();
            }

            return null;
        }


        public Share GetById(int id)
        {
            var con = new SqlConnection(Config.ConnectionString);
            var cmd = new SqlCommand
            {
                CommandText = $@"SELECT * FROM share 
                                WHERE id = {id}",
                Connection = con
            };


            con.Open();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return Share.Parse(reader);
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

        public bool Delete(int shareId)
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
    }
}