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
    public class LeaderboardsController : ApiController
    {
        private ILeaderboardsRepository _leaderboardsRepo;

        public LeaderboardsController()
        {
            _leaderboardsRepo = new LeaderboardsRepository();
        }

        [HttpGet]
        [Route("api/user/leaderboard")]
        public IHttpActionResult GetUsersLeaderboard(
            [FromUri] Leaderboard.Type? type = null,
            [FromUri] Leaderboard.Range range = Leaderboard.Range.Global,
            [FromUri] Leaderboard.TimeSpan timeSpan = Leaderboard.TimeSpan.AllTime,
            [FromUri] int page = 0, [FromUri] int limit = 0)
        {
            if (!Session.Authorized)
                return Unauthorized();

            if (type == null)
            {
                return BadRequest("Manca il campo type");
            }

            var data = type + " " + range + " " + timeSpan + " " + page + " " + limit;

            return Ok(data);
            //return Ok(Session.User);
        }

        [HttpGet]
        [Route("api/company/leaderboard")]
        public IHttpActionResult GetCompaniesLeaderboard(
            [FromUri] Leaderboard.Type? type = null,
            [FromUri] Leaderboard.Range range = Leaderboard.Range.Global,
            [FromUri] Leaderboard.TimeSpan timeSpan = Leaderboard.TimeSpan.AllTime,
            [FromUri] int page = 0, [FromUri] int limit = 0)
        {
            if (!Session.Authorized)
                return Unauthorized();

            if (type == null)
            {
                return BadRequest("Manca il campo type");
            }

            var data = type + " " + range + " " + timeSpan;

            return Ok(data);
            //return Ok(Session.User);
        }
    }
}