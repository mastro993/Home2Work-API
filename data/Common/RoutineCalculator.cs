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
        private readonly LocationRepository _locationRepo;

        private readonly int DAYS_RANGE = 60;
        private readonly int RANGE = 250;
        private readonly double MIN_DISTANCE = 2.5;
        private readonly int TIME_DIFF = 15 * 60;

        public RoutineCalculator(LocationRepository locationRepo)
        {
            _locationRepo = locationRepo;
        }

        public IEnumerable<UserRoutine> GetRoutines(User user)
        {
            var routines = new List<Routine>();
            var routine = new Routine();

            Route route = null;
            var routes = new List<Route>();

            var locations = _locationRepo
                // Getting last DAYS_RANGE days locations
                .GetUserLocationsFromDate(user.Id, DateTime.Now.Subtract(new TimeSpan(DAYS_RANGE, 0, 0, 0)).Date)
                // Ordered by date, avoiding eventual db inconsistency
                .OrderBy(l => l.Date);


            foreach (var loc in locations)
            {
                if (route == null)
                {
                    // If the route is empty checks if it must be initialized with a new "start" location
                    if (loc.Type == UserLocation.LocationType.Start)
                    {
                        route = new Route{StartLocation = loc};
                    }
                    // If the location is not a "start" location it is skipped (route cannot start with an "end" location
                    continue;
                } 

                // Else if the route is already been initialized checks the type of the next location
                if (loc.Type == UserLocation.LocationType.Start)
                {
                    // Case when there is a "start" location

                    // In the case where the route is initialized checks if is present an "end" location
                    if (route.EndLocation != null)
                    {
                        // If present, checks the time difference
                        var timediff = loc.Date.Subtract(route.EndLocation.Date).TotalSeconds;
                        // If the time difference is less than TIME_DIFF, end locations is removed and current location skipped.
                        // This is the case where an user takes a stop driving and it must not be counted as end of the route.
                        if (timediff < TIME_DIFF)
                        {
                            route.EndLocation = null;
                            continue;
                        }
                        // Else if the stop is long enough, the route is closed and initialized a new one with te current "start" location
                        // But first is checked if the route is not "short" or "circular"

                        var startEndDistance = MapUtils.Haversine(route.StartLocation.Latitude,
                            route.StartLocation.Longitude, route.EndLocation.Latitude, route.EndLocation.Longitude);

                        // If the distance between the start and the end is more than MIN_DISTANCE, the route is completed
                        if (startEndDistance >= MIN_DISTANCE)
                        {
                            routes.Add(route);
                            route = new Route { StartLocation = loc };
                            continue;
                        }

                        // Else, if the start-end distance is less than MIN_DISTANCE, the route is discarded
                        route = null;
                        continue;

                    }

                }

                // Else if the location is an "end" location, simply add the location in the current route
                route.EndLocation = loc;
                
            }

            // In the case the process leaves an unclosed route, it will be discarded
            if (route?.EndLocation != null)
            {
                routes.Add(route);
            }


            return null;
        }


        private class Routine
        {
            public IEnumerable<UserLocation> StartLocations { get; set; }
            public IEnumerable<UserLocation> EndLocations { get; set; }
            int Frequency { get; set; }

            public Routine()
            {
                StartLocations = new List<UserLocation>();
                EndLocations = new List<UserLocation>();
            }

            public bool IsEmpty()
            {
                return !StartLocations.Any() && !EndLocations.Any();
            }

            public void Add(UserLocation userLocation)
            {
                if (userLocation.Type == UserLocation.LocationType.Start)
                {
                    StartLocations.Append(userLocation);
                }
                else
                {
                    EndLocations.Append(userLocation);
                }
            }
        }

        private class Route
        {
            public UserLocation StartLocation { get; set; }
            public UserLocation EndLocation { get; set; }

        }
    }
}