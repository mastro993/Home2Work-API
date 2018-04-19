using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using data.Repositories;
using domain.Interfaces;
using HomeToWork_API.Auth;
using HomeToWork_API.Utils;
using Microsoft.Ajax.Utilities;

namespace HomeToWork_API.Controllers
{
    public class FirebaseController : ApiController
    {


        private readonly IFirebaseRepository _firebaseRepo;

        public FirebaseController()
        {

            _firebaseRepo = new FcmTokenRepository();
        }

        [HttpPost]
        [Route("api/firebase/token")]
        public IHttpActionResult PostFcmToken(FormDataCollection data)
        {
            if (!Session.Authorized)
            {
                return Unauthorized();
            }

            var valueMap = FormDataConverter.Convert(data);
            var newToken = valueMap.Get("token");

            if (newToken.IsNullOrWhiteSpace())
            {
                return BadRequest("Token Firebase Cloud Messaging mancante");
            }

            var token = _firebaseRepo.GetUserToken(Session.User.Id);

            if (token.IsNullOrWhiteSpace())
            {
                var inserted = _firebaseRepo.SetUserToken(Session.User.Id, newToken);
                return Ok(inserted);
            }

            if (token.Equals(newToken))
            {
                return Ok(true);
            }

            var updated = _firebaseRepo.UpdateUserToken(Session.User.Id, newToken);

            return Ok(updated);
        }
    }
}
