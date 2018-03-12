using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeToWork.Location;
using HomeToWork.User;

namespace HomeToWork.Routine
{
    public class RoutineCalculator
    {
        public static void execute()
        {
            var users = new UserDao().GetAll();

            foreach (var user in users)
            {
                var routes = getUserRoutes(user.Id);
            }
        }

        private static List<Route> getUserRoutes(long userId)
        {
            
            var locations = new LocationDao().GetAllUserLocations(userId, byDate: true);

            var routes = new List<Route>();
            var route = new Route();

            foreach (var location in locations)
            {
                if (route.Size == 0)
                {
                    route.Add(location);
                    continue;
                }

                var timeDeltaMinutes = (route.Last.Timestamp - location.Timestamp).TotalMinutes;
                if (Math.Abs(timeDeltaMinutes) > 30.0)
                {
                    routes.Add(route);
                    route = new Route();
                }

                route.Add(location);

            }


            return routes;
        }
    }
}