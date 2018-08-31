using System;
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
                    guest.Address.Latitude.Value, guest.Address.Longitude.Value,
                    host.Address.Latitude.Value, host.Address.Longitude.Value,
                    dMin, dMax);

                if (homeScore == 0) continue;

                var jobScore = GetDistanceScore(
                    guestCompany.Address.Latitude.Value, guestCompany.Address.Longitude.Value,
                    hostCompany.Address.Latitude.Value, hostCompany.Address.Longitude.Value,
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

        private static int GetDistanceScore(
            double guestLat, double guestLng,
            double hostLat, double hostLng,
            double minDistance, double maxDistance)
        {
            var distance = MapUtils.Haversine(guestLat, guestLng, hostLat, hostLng);

            var score = (int) ((distance - maxDistance) / (minDistance - maxDistance)) * 100;

            score = score > 100 ? 100 : score;
            score = score < 0 ? 0 : score;

            return score;
        }

        private static int GetTimeScore(
            TimeSpan guestStartTime, TimeSpan guestEndTime,
            TimeSpan hostStartTime, TimeSpan hostEndTime,
            TimeSpan minTimeDelta, TimeSpan maxTimeDelta)
        {
            var startTimeDelta = guestStartTime.Subtract(hostStartTime).Duration();
            var endTimeDelta = guestEndTime.Subtract(hostEndTime).Duration();

            var startScore = (startTimeDelta.Subtract(maxTimeDelta).Milliseconds /
                              minTimeDelta.Subtract(maxTimeDelta).Milliseconds) * 100;
            var endScore = (endTimeDelta.Subtract(maxTimeDelta).Milliseconds /
                            minTimeDelta.Subtract(maxTimeDelta).Milliseconds) * 100;

            startScore = startScore > 100 ? 100 : startScore;
            startScore = startScore < 0 ? 0 : startScore;

            endScore = endScore > 100 ? 100 : endScore;
            endScore = endScore < 0 ? 0 : endScore;

            if (startScore == 0 || endScore == 0) return 0;

            return (startScore + endScore) / 2;
        }
    }
}