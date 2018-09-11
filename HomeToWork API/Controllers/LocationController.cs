using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using data.Repositories;
using domain.Entities;
using domain.Interfaces;
using HomeToWork_API.Auth;

namespace HomeToWork_API.Controllers
{
    public class LocationController : ApiController
    {
        private readonly ILocationRespository _locationRepo;

        public LocationController()
        {
            _locationRepo = new LocationRepository();
        }

        [HttpPost]
        [Route("user/location")]
        public IHttpActionResult PostUserLocations(IEnumerable<UserLocation> locations)
        {
            if (!Session.Authorized)
            {
                return Unauthorized();
            }

            var locationList = locations.ToList();

            foreach (var location in locationList)
            {
                _locationRepo.InsertUserSCLLocation(
                    Session.User.Id, 
                    location.Latitude, 
                    location.Longitude,
                    location.Date, 
                    location.Type);
            }

            return Ok(true);
        }

        [HttpPost]
        [Route("user/lastlocation")]
        public IHttpActionResult PostUserLastLocation(UserLocation location)
        {
            if (!Session.Authorized)
            {
                return Unauthorized();
            }

            var inserted = _locationRepo.InsertUserLastLocation(Session.User.Id, location.Latitude, location.Longitude,
                location.Date);

            return Ok(inserted);
        }
    }
}