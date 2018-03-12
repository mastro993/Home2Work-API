using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeToWork.Common;
using HomeToWork.Company;
using HomeToWork.User;
using HomeToWork.Utils;

namespace HomeToWork.Match
{
    public class Matcher
    {
        public static List<Match> GetAffineUsers(int userId)
        {

            const double dMax = 10.0; // 5km
            const double dMin = 1.5; // 1km

            var userDao = new UserDao();
            var companyDao = new CompanyDao();

            var guest = userDao.GetById(userId);
            var users = userDao.GetAll();

            var affineUsers = new List<Match>();
            var guestCompany = companyDao.GetById(guest.Company.Id);

            users = users.FindAll(user => user.Id != guest.Id);

            foreach (var host in users)
            {
                var hostCompany = companyDao.GetById(host.Company.Id);

                var homeScore = GetDistanceScore(guest.HomeLatLng, host.HomeLatLng, dMin, dMax);
                if (homeScore == 0) continue;

                var jobScore = GetDistanceScore(guestCompany.LatLng, hostCompany.LatLng, dMin, dMax);
                if (jobScore == 0) continue;

                affineUsers.Add(new Match()
                {
                    UserId = guest.Id,
                    Host = host,
                });
            }

            return affineUsers;
        }

        private static int GetDistanceScore(LatLng guest, LatLng host, double minDistance, double maxDistance)
        {


            var distance = MapUtils.Haversine(guest.Latitude, guest.Longitude, host.Latitude, host.Longitude);

            var score = (int) ((distance - maxDistance) / (minDistance - maxDistance)) * 100;

            score = score > 100 ? 100 : score;
            score = score < 0 ? 0 : score;

            return score;
        }
    }
}