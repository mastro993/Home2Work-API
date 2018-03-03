using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeToWork.Common;
using HomeToWork.User;

namespace HomeToWork.Share
{
    [Serializable]
    public class Guest
    {
        public static readonly int Joined = 0;
        public static readonly int Completed = 1;
        public static readonly int Canceled = 2;

        public int ShareId { get; set; }
        public User.User User { get; set; }
        public LatLng StartLatLng { get; set; }
        public LatLng EndLatLng { get; set; }
        public int Status { get; set; }
        public int Distance { get; set; }

        public static Guest Parse(SqlDataReader reader)
        {
            var shareId = int.Parse(reader["share_id"].ToString());
            var guestId = int.Parse(reader["user_id"].ToString());
            var startLatLng = LatLng.Parse(reader["start_location"].ToString());
            var endLatLng = LatLng.Parse(reader["end_location"].ToString());
            var status = int.Parse(reader["status"].ToString());
            var distance = int.Parse(reader["distance"].ToString());

            var userDao = new UserDao();

            return new Guest()
            {
                ShareId = shareId,
                User = userDao.GetById(guestId),
                StartLatLng = startLatLng,
                EndLatLng = endLatLng,
                Status = status,
                Distance = distance,
            };
        }
    }
}