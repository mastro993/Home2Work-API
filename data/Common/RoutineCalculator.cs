using System;
using System.Collections.Generic;
using System.Linq;
using data.Common.Utils;
using data.Repositories;
using domain.Entities;

namespace data.Common
{
    public class RoutineCalculator
    {
        private readonly UserRepository _userRepo;
        private readonly LocationRepository _locationRepo;

        private readonly int RANGE = 250;

        public RoutineCalculator(UserRepository userRepo, LocationRepository locationRepo)
        {
            _userRepo = userRepo;
            _locationRepo = locationRepo;
        }

        public void Execute()
        {
            var users = _userRepo.GetAll();


            foreach (var user in users)
            {
                var routines = GetUserRoutines(user);
            }
        }


        private IEnumerable<UserRoutine> GetUserRoutines(User user)
        {
            var jobStartTime = user.JobStartTime.GetValueOrDefault();
            var jobEndTime = user.JobEndTime.GetValueOrDefault();

            var homeLocations = _locationRepo.GetUserLocationsInRange(
                    user.Id,
                    user.Address.Latitude.GetValueOrDefault(),
                    user.Address.Longitude.GetValueOrDefault(),
                    RANGE)
                .Where(l => l.Date.DayOfWeek != DayOfWeek.Saturday && l.Date.DayOfWeek != DayOfWeek.Sunday)
                .ToList();

            var jobLocations = _locationRepo.GetUserLocationsInRange(
                    user.Id,
                    user.JobAddress.Latitude.GetValueOrDefault(),
                    user.JobAddress.Longitude.GetValueOrDefault(),
                    RANGE)
                .Where(l => l.Date.DayOfWeek != DayOfWeek.Saturday && l.Date.DayOfWeek != DayOfWeek.Sunday)
                .ToList();

            var homeLocationsBeforeJob = homeLocations
                .Where(l => l.Date.TimeOfDay.CompareTo(jobStartTime) < 0)
                .Where(l => l.Date.TimeOfDay.CompareTo(jobStartTime.Subtract(new TimeSpan(3,0,0))) > 0)
                .ToList();

            var jobLocationsBeforeJob = jobLocations
                .Where(l => l.Date.TimeOfDay.CompareTo(jobStartTime) < 0)
                .Where(l => l.Date.TimeOfDay.CompareTo(jobStartTime.Subtract(new TimeSpan(3, 0, 0))) > 0)
                .ToList();

            var fromHomeTime = new TimeSpan();
            if (homeLocationsBeforeJob.Count > 0)
            {
                var fromHomeTimeAvg = homeLocationsBeforeJob.Average(l => l.Date.TimeOfDay.TotalSeconds);
                fromHomeTime = TimeSpan.FromSeconds(fromHomeTimeAvg);
            }

            var toJobTime = new TimeSpan();
            if (jobLocationsBeforeJob.Count > 0)
            {
                var toJobTimeAvg = jobLocationsBeforeJob.Average(l => l.Date.TimeOfDay.TotalSeconds);
                toJobTime = TimeSpan.FromSeconds(toJobTimeAvg);
            }

            var toJobRoutine = new UserRoutine
            {
                StartLat = user.Address.Latitude.GetValueOrDefault(),
                StartLng = user.Address.Longitude.GetValueOrDefault(),
                EndLat = user.JobAddress.Latitude.GetValueOrDefault(),
                EndLng = user.JobAddress.Longitude.GetValueOrDefault(),
                StartTime = fromHomeTime,
                EndTime = toJobTime,
                Type = UserRoutine.RoutineType.Job
            };


            var jobLocationsAfterJob = jobLocations
                .Where(l => l.Date.TimeOfDay.CompareTo(jobEndTime) > 0)
                .Where(l => l.Date.TimeOfDay.CompareTo(jobEndTime.Add(new TimeSpan(3, 0, 0))) < 0)
                .ToList();

            var homeLocationsAfterJob = homeLocations
                .Where(l => l.Date.TimeOfDay.CompareTo(jobEndTime) > 0)
                .Where(l => l.Date.TimeOfDay.CompareTo(jobEndTime.Add(new TimeSpan(3, 0, 0))) < 0)
                .ToList();

            var toHomeTime = new TimeSpan();
            if (homeLocationsAfterJob.Count > 0)
            {
                var toHomeTimeAvg = homeLocationsAfterJob.Average(l => l.Date.TimeOfDay.TotalSeconds);
                toHomeTime = TimeSpan.FromSeconds(toHomeTimeAvg);
            }

            var fromJobTime = new TimeSpan();
            if (jobLocationsAfterJob.Count > 0)
            {
                var fromJobTimeAvg = jobLocationsAfterJob.Average(l => l.Date.TimeOfDay.TotalSeconds);
                fromJobTime = TimeSpan.FromSeconds(fromJobTimeAvg);
            }

            var toHomeRoutine = new UserRoutine
            {
                StartLat = user.JobAddress.Latitude.GetValueOrDefault(),
                StartLng = user.JobAddress.Longitude.GetValueOrDefault(),
                EndLat = user.Address.Latitude.GetValueOrDefault(),
                EndLng = user.Address.Longitude.GetValueOrDefault(),
                StartTime = fromJobTime,
                EndTime = toHomeTime,
                Type = UserRoutine.RoutineType.Home
            };

            return new List<UserRoutine> {toJobRoutine, toHomeRoutine};
        }
    }
}