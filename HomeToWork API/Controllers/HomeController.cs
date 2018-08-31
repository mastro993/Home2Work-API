using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using data.Common.Utils;
using data.Repositories;
using domain.Entities;
using GoogleApi;
using GoogleApi.Entities.Maps.Directions.Request;

namespace HomeToWork_API.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            
            /*var userRepo = new UserRepository();
            var locationRepo = new LocationRepository();

            var users = userRepo.GetAll();

            foreach (var user in users)
            {
                if(user.Id == 3) continue;

                var date = new DateTime(2018, 3, 1);

                var request = new DirectionsRequest()
                {
                    Origin = new GoogleApi.Entities.Common.Location(user.Address.Latitude.Value,
                        user.Address.Longitude.Value),
                    Destination =
                        new GoogleApi.Entities.Common.Location(user.JobAddress.Latitude.Value,
                            user.JobAddress.Longitude.Value),
                    Key = "AIzaSyDrnpGbytKl9jFKNTmI3B2vXh_68x3DG2Y"
                };
                var result = GoogleMaps.Directions.Query(request);
                var duration = result.Routes.First().Legs.First().Duration.Value;

                var routeDuration = new TimeSpan(0, 0, duration);

                for (var i = 0; i < 30; i++)
                {

                    if (date.DayOfWeek != DayOfWeek.Sunday)
                    {
                        var rnd = new Random();

                        var latDelta = rnd.Next(-50, 50).ToDouble() / 10000;
                        var homeLat = user.Address.Latitude.Value + latDelta;
                        var lngDelta = rnd.Next(-50, 50).ToDouble() / 10000;
                        var homeLng = user.Address.Longitude.Value + lngDelta;
                        var timeDelta = new TimeSpan(0, rnd.Next(-20, 10), 0);
                        var homeDeparture = user.DepartureTime.Value.Add(timeDelta);

                        locationRepo.InsertUserSCLLocation(user.Id, homeLat, homeLng, date.Add(homeDeparture));

                        latDelta = rnd.Next(-50, 50).ToDouble() / 10000;
                        var jobLat = user.JobAddress.Latitude.Value + latDelta;
                        lngDelta = rnd.Next(-50, 50).ToDouble() / 10000;
                        var jobLng = user.JobAddress.Longitude.Value + lngDelta;
                        timeDelta = new TimeSpan(0, rnd.Next(0, 10), 0);
                        var jobArrival = homeDeparture.Add(routeDuration).Add(timeDelta);

                        locationRepo.InsertUserSCLLocation(user.Id, jobLat, jobLng, date.Add(jobArrival));

                        latDelta = rnd.Next(-50, 50).ToDouble() / 10000;
                        jobLat = user.JobAddress.Latitude.Value + latDelta;
                        lngDelta = rnd.Next(-50, 50).ToDouble() / 10000;
                        jobLng = user.JobAddress.Longitude.Value + lngDelta;
                        timeDelta = new TimeSpan(0, rnd.Next(-10, 15), 0);
                        var jobDeparture = user.JobDepartureTime.Value.Add(timeDelta);

                        locationRepo.InsertUserSCLLocation(user.Id, jobLat, jobLng, date.Add(jobDeparture));

                        latDelta = rnd.Next(-50, 50).ToDouble() / 10000;
                        homeLat = user.JobAddress.Latitude.Value + latDelta;
                        lngDelta = rnd.Next(-50, 50).ToDouble() / 10000;
                        homeLng = user.JobAddress.Longitude.Value + lngDelta;
                        timeDelta = new TimeSpan(0, rnd.Next(0, 20), 0);
                        var homeArrival = jobDeparture.Add(routeDuration).Add(timeDelta);

                        locationRepo.InsertUserSCLLocation(user.Id, homeLat, homeLng, date.Add(homeArrival));
                    }


                    date = date.AddDays(1);
                }
            }*/


            ViewBag.Title = "Home Page";

            return View();
        }
    }
}