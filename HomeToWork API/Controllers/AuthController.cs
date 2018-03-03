using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Description;
using HomeToWork.Auth;
using HomeToWork.User;
using HomeToWork.Utils;
using HomeToWork_API.Utils;
using Microsoft.Ajax.Utilities;

namespace HomeToWork_API.Controllers
{
    public class AuthController : ApiController
    {
        [HttpPost]
        [Route("api/auth/register")]
        public IHttpActionResult Register(FormDataCollection data)
        {
            var valueMap = FormDataConverter.Convert(data);
            var email = valueMap.Get("email");
            var password = valueMap.Get("password");

            var randomString = StringUtils.RandomString();
            var salt = HashingUtils.Sha256(randomString);
            var saltedPassword = HashingUtils.Sha256(password + salt);


            var authDao = new AuthDao();
            var userId = authDao.Register(email, saltedPassword, salt);

            if (userId != 0)
            {
                return Created(new Uri("api/user/" + userId), User);
            }

            return InternalServerError();
        }


        [HttpPost]
        [Route("api/auth/login")]
        [ResponseType(typeof(AuthUser))]
        public HttpResponseMessage Login(FormDataCollection data)
        {
            var valueMap = FormDataConverter.Convert(data);

            var email = valueMap.Get("email");
            var password = valueMap.Get("password");

            if (email.IsNullOrWhiteSpace() || password.IsNullOrWhiteSpace())
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var authDao = new AuthDao();

            var salt = authDao.GetUserSalt(email);
            var saltedPassword = HashingUtils.Sha256(password + salt);
            var user = authDao.Login(email, saltedPassword);

            if (user == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            var userDao = new UserDao();

            // Dopo al login tramite email e password creo un nuovo access token
            var accessToken = userDao.NewAccessToken(user.Id);
            user.AccessToken = accessToken;

            // Una volta autenticato l'utente creo un token per la sessione
            // e lo restituisco nell'header della risposta
            var sessionToken = userDao.NewSessionToken(user.Id);

            var response = Request.CreateResponse(HttpStatusCode.OK, user);
            response.Headers.Add("X-User-Session-Token", sessionToken);
            return response;
        }


        [HttpPost]
        [Route("api/auth/login/token")]
        [ResponseType(typeof(AuthUser))]
        public HttpResponseMessage TokenLogin(FormDataCollection data)
        {
            var valueMap = FormDataConverter.Convert(data);
            var token = valueMap.Get("token");

            if (token.IsNullOrWhiteSpace())
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var authDao = new AuthDao();
            var user = authDao.Login(token);

            if (user == null)
                return Request.CreateResponse(HttpStatusCode.NotFound, "Incorrect password");

            // Una volta autenticato l'utente creo un token per la sessione
            // e lo restituisco nell'header della risposta
            var userDao = new UserDao();
            var sessionToken = userDao.NewSessionToken(user.Id);
            var response = Request.CreateResponse(HttpStatusCode.OK, user);
            response.Headers.Add("X-User-Session-Token", sessionToken);
            return response;
        }
    }
}