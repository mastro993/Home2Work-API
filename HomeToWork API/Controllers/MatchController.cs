using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HomeToWork.Match;
using HomeToWork_API.Auth;

namespace HomeToWork_API.Controllers
{
    public class MatchController : ApiController
    {
        private MatchDao matchDao;

        public MatchController()
        {
            matchDao = new MatchDao();
        }

        [HttpGet]
        [Route("api/match/{matchId:int}")]
        public IHttpActionResult GetMatchById(int matchId)
        {
            if (!Session.Authorized) return Unauthorized();

            var match = matchDao.GetById(matchId);

            if (match == null)
                return NotFound();

            match.New = false;
            matchDao.EditMatch(match);

            return Ok(match);
        }

        [HttpPut]
        [Route("api/match")]
        public IHttpActionResult PutMatch(Match match)
        {
            if (!Session.Authorized) return Unauthorized();

            match = matchDao.EditMatch(match);

            if (match != null) return Ok(match);

            return InternalServerError();
        }
    }
}