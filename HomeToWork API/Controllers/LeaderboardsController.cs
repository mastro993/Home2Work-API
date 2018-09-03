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
        private readonly ILeaderboardsRepository _leaderboardsRepo;

        public LeaderboardsController()
        {
            _leaderboardsRepo = new LeaderboardsRepository();
        }

        [HttpGet]
        [Route("user/leaderboard")]
        public IHttpActionResult GetUsersLeaderboard(
            [FromUri] Leaderboard.Type type = Leaderboard.Type.Shares,
            [FromUri] Leaderboard.Range range = Leaderboard.Range.Global,
            [FromUri] Leaderboard.TimeSpan timeSpan = Leaderboard.TimeSpan.AllTime,
            [FromUri] int? companyId = null,
            [FromUri] int page = 1,
            [FromUri] int limit = 20)
        {
            if (!Session.Authorized)
                return Unauthorized();

            var leaderboard = range == Leaderboard.Range.Company
                ? _leaderboardsRepo.GetUsersCompanyLeaderboard(companyId, type, timeSpan, page, limit)
                : _leaderboardsRepo.GetUsersGlobalLeaderboard(type, timeSpan, page, limit);


            return Ok(leaderboard);
        }

        [HttpGet]
        [Route("company/leaderboard")]
        public IHttpActionResult GetCompaniesLeaderboard(
            [FromUri] Leaderboard.Type? type = null,
            [FromUri] Leaderboard.Range range = Leaderboard.Range.Global,
            [FromUri] Leaderboard.TimeSpan timeSpan = Leaderboard.TimeSpan.AllTime,
            [FromUri] int page = 1,
            [FromUri] int limit = 20)
        {
            if (!Session.Authorized)
                return Unauthorized();

            if (type == null)
            {
                return BadRequest("Campo Type mancante");
            }

            var data = type + " " + range + " " + timeSpan;

            return Ok();
        }

        [HttpGet]
        [Route("user/{userId}/leaderboard")]
        public IHttpActionResult GetUserRankings(long userId)
        {
            if (!Session.Authorized)
                return Unauthorized();

            //var rankings = _leaderboardsRepo.GetUserRanks(userId);
        

            return Ok();
        }
    }
}