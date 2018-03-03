using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeToWork.User;

namespace HomeToWork.Share
{
    [Serializable]
    public class Share
    {
        public static readonly int Created = 0;
        public static readonly int Completed = 1;
        public static readonly int Canceled = 2;

        public int Id { get; set; }
        public User.User Host { get; set; }
        public int Status { get; set; }
        public DateTime Time { get; set; }
        public ShareType Type { get; set; }
        public List<Guest> Guests { get; set; }

        public static Share Parse(SqlDataReader reader)
        {
            var userDao = new UserDao();

            var shareId = int.Parse(reader["id"].ToString());
            var hostId = int.Parse(reader["host_id"].ToString());
            var status = int.Parse(reader["status"].ToString());
            var time = DateTime.Parse(reader["time"].ToString());

            var guestDao = new GuestDao();
            var guests = guestDao.GetAllByShareId(shareId);

            return new Share()
            {
                Id = shareId,
                Host = userDao.GetById(hostId),
                Status = status,
                Time = time,
                Guests = guests
            };
        }
    }

    public enum ShareType
    {
        Driver,
        Guest
    }
}