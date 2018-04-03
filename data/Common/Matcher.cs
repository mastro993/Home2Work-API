using System.Collections.Generic;
using data.Common.Utils;
using data.Repositories;
using domain.Entities;
using domain.Interfaces;

namespace data.Common
{
    public class Matcher
    {
        private readonly IUserRepository _userRepo;
        private readonly ICompanyRepository _companyRepo;

        public Matcher()
        {
            _userRepo = new UserRepository();
            _companyRepo = new CompanyRepository();
        }

        public List<Match> GetAffineUsers(long userId)
        {
            const double dMax = 10.0; // 10km
            const double dMin = 1.5; // 1km


            var guest = _userRepo.GetById(userId);
            var users = _userRepo.GetAll();

            var affineUsers = new List<Match>();
            var guestCompany = _companyRepo.GetById(guest.Company.Id);

            users = users.FindAll(user => user.Id != guest.Id);

            foreach (var host in users)
            {
                var hostCompany = _companyRepo.GetById(host.Company.Id);

                var homeScore = GetDistanceScore(
                    guest.Address.Latitude, guest.Address.Longitude,
                    host.Address.Latitude, host.Address.Longitude,
                    dMin, dMax);

                if (homeScore == 0) continue;

                var jobScore = GetDistanceScore(
                    guestCompany.Address.Latitude, guestCompany.Address.Longitude,
                    hostCompany.Address.Latitude, hostCompany.Address.Longitude,
                    dMin, dMax);
                if (jobScore == 0) continue;

                affineUsers.Add(new Match()
                {
                    UserId = guest.Id,
                    Host = host,
                    HomeScore = null,
                    TimeScore = null,
                    JobScore = null,
                });
            }

            return affineUsers;
        }

        private static int GetDistanceScore(double guestLat, double guestLng, double hostLat, double hostLng,
            double minDistance, double maxDistance)
        {
            var distance = MapUtils.Haversine(guestLat, guestLng, hostLat, hostLng);

            var score = (int) ((distance - maxDistance) / (minDistance - maxDistance)) * 100;

            score = score > 100 ? 100 : score;
            score = score < 0 ? 0 : score;

            return score;
        }
    }
}