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
        [Route("api/user/location")]
        public IHttpActionResult PostUserLocations(IEnumerable<Location> locations)
        {
            if (!Session.Authorized) return Unauthorized();

            var locationList = locations.ToList();

            foreach (var location in locationList)
            {
                _locationRepo.InsertUserLocation(Session.User.Id, location.Latitude, location.Longitude, location.Date);
            }

            return Ok(true);
        }

        [HttpGet]
        [Route("api/user/{userId:int}/location")]
        public IHttpActionResult GetUserLocations(int userId)
        {
            if (!Session.Authorized) return Unauthorized();


            var locations = _locationRepo.GetAllUserLocations(userId, false);
            return Ok(locations);
        }
    }
}