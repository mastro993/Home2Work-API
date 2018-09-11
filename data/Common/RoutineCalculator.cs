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
        private readonly int TIME_DIFF = 60 * 60;

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
            var locations = _locationRepo
                .GetUserLocationsFromDate(user.Id, DateTime.Now.Subtract(new TimeSpan(30, 0, 0, 0)).Date)
                .OrderBy(l => l.Date);


            var routines = new List<Routine>();
            var routine = new Routine();

            foreach (var loc in locations)
            {
                if (routine.IsEmpty())
                {
                    if (loc.Type == UserLocation.LocationType.Start)
                    {
                        routine.StartLocations.Add(loc);
                    }
                }
                else
                {
                    if (loc.Type == UserLocation.LocationType.End)
                    {
                        routine.EndLocations.Add(loc);
                        routines.Add(routine);
                        routine = new Routine();
                    }
                }
            }


            return null;
        }


        private class Routine
        {
            public List<UserLocation> StartLocations { get; set; }
            public List<UserLocation> EndLocations { get; set; }
            int Frequency { get; set; }

            public Routine()
            {
                StartLocations = new List<UserLocation>();
                EndLocations = new List<UserLocation>();
            }

            public bool IsEmpty()
            {
                return StartLocations.Count == 0 && EndLocations.Count == 0;
            }

            public void Add(UserLocation userLocation)
            {
                if (userLocation.Type == UserLocation.LocationType.Start)
                {
                    StartLocations.Add(userLocation);
                }
                else
                {
                    EndLocations.Add(userLocation);
                }
            }
        }
    }
}