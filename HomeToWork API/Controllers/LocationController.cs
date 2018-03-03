using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HomeToWork.Location;
using HomeToWork_API.Auth;

namespace HomeToWork_API.Controllers
{
    public class LocationController : ApiController
    {
        private LocationDao locationDao;

        public LocationController()
        {
            locationDao = new LocationDao();
        }

        [HttpGet]
        [Route("api/user/{userId:int}/location")]
        public IHttpActionResult GetUserLocations(int userId)
        {
            if (!Session.Authorized) return Unauthorized();


            var locations = locationDao.GetAllUserLocations(userId, false);
            return Ok(locations);
        }
    }
}