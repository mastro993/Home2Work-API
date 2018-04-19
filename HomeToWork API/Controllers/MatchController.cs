using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using data.Common;
using data.Repositories;
using domain.Entities;
using domain.Interfaces;
using HomeToWork_API.Auth;

namespace HomeToWork_API.Controllers
{
    public class MatchController : ApiController
    {
        private readonly IMatchRepository _matchRepo;

        public MatchController()
        {
            _matchRepo = new MatchRepository();
        }

        [HttpGet]
        [Route("api/match/list")]
        public IHttpActionResult GetMatches(
            [FromUri] int page = 1,
            [FromUri] int limit = int.MaxValue
        )
        {
            if (!Session.Authorized)
            {
                return Unauthorized();
            }

            var matches = _matchRepo.GetByUserId(Session.User.Id, limit, page);

            if (matches.Count != 0)
            {
                return Ok(matches);
            }

            var matcher = new Matcher();
            matches = matcher.GetAffineUsers(Session.User.Id);

            return Ok(matches);
        }

        [HttpGet]
        [Route("api/match/{matchId:int}")]
        public IHttpActionResult GetMatchById(int matchId)
        {
            if (!Session.Authorized)
            {
                return Unauthorized();
            }

            var match = _matchRepo.GetById(matchId);

            if (match == null)
            {
                return NotFound();
            }    

            match.IsNew = false;
            _matchRepo.EditMatch(match);

            return Ok(match);
        }

        [HttpPut]
        [Route("api/match")]
        public IHttpActionResult PutMatch(Match match)
        {
            if (!Session.Authorized)
            {
                return Unauthorized();
            }

            if (match == null)
            {
                return BadRequest("Oggetto match non presente");
            }

            match = _matchRepo.EditMatch(match);

            if (match != null) return Ok(match);

            return InternalServerError();
        }
    }
}