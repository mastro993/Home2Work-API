using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeToWork.Common;
using HomeToWork.Company;
using HomeToWork.User;

namespace HomeToWork.Match
{
    public class MatchCalculator
    {
        private static readonly double D_MAX = 5000; // 5km
        private static readonly double D_MIN = 1500; // 1km

        public static List<Match> GetRelatedUsers(int userId)
        {
            var userDao = new UserDao();
            var companyDao = new CompanyDao();

            var guest = userDao.GetById(userId);
            var users = userDao.GetAll();

            var probableMatches = new List<Match>();
            var guestCompany = companyDao.GetById(guest.Company.Id);

            foreach (var host in users)
            {
                if (guest.Id == host.Id) continue;

                var hostCompany = companyDao.GetById(host.Company.Id);

                var homeScore = GetDistanceScore(guest.HomeLatLng, host.HomeLatLng);
                if (Math.Abs(homeScore) < 1) continue;

                var jobScore = GetDistanceScore(guestCompany.LatLng, hostCompany.LatLng);
                if (Math.Abs(jobScore) < 1) continue;

                probableMatches.Add(new Match()
                {
                    Guest = guest,
                    Host = host,
                });
            }

            return probableMatches;
        }

        private static double GetDistanceScore(LatLng guestHome, LatLng hostHome)
        {
            var distanceDelta = D_MAX - D_MIN;
            var distance = guestHome.GetDistanceTo(hostHome);
            var x = distanceDelta - Math.Min(distanceDelta, Math.Max(0, distance - D_MIN));
            var homeScore = (100 / distanceDelta) * x;

            return homeScore;
        }
    }
}