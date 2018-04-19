using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using data.Common;
using data.Common.Utils;
using data.Repositories;
using domain.Entities;
using domain.Interfaces;
using HomeToWork_API.Auth;
using HomeToWork_API.Utils;
using Microsoft.Ajax.Utilities;
using StringUtils = HomeToWork_API.Utils.StringUtils;

namespace HomeToWork_API.Controllers
{
    public class UserController : ApiController
    {
        private readonly IUserRepository _userRepo;

        public UserController()
        {
            _userRepo = new UserRepository();
        }

        [HttpPost]
        [Route("api/user/login")]
        [ResponseType(typeof(User))]
        public HttpResponseMessage Login(FormDataCollection data)
        {
            var valueMap = FormDataConverter.Convert(data);

            var email = valueMap.Get("email");

            if (email.IsNullOrWhiteSpace())
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Email non presente");
            }

            if (!StringUtils.ValidateEmail(email))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Email non valida. Formato non corretto");
            }

            var password = valueMap.Get("password");

            if (password.IsNullOrWhiteSpace())
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Password non presente");
            }


            var salt = _userRepo.GetUserSalt(email);
            var passwordHash = HashingUtils.Sha256(password + salt);

            var user = _userRepo.Login(email, passwordHash);

            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized,
                    "Nessun utente trovato con le credenziali immesse");
            }

            var sessionToken = _userRepo.NewSessionToken(user.Id);

            var response = Request.CreateResponse(HttpStatusCode.OK, user);
            response.Headers.Add("X-User-Session-Token", sessionToken);
            return response;
        }

        [HttpGet]
        [Route("api/user")]
        public IHttpActionResult Get([FromUri] long? id = null)
        {
            if (!Session.Authorized)
            {
                return Unauthorized();
            }

            if (!id.HasValue)
            {
                return Ok(Session.User);
            }

            var user = _userRepo.GetById(id.Value);
            return Ok(user);
        }

        [HttpPost]
        [Route("api/user/status")]
        public IHttpActionResult Get(FormDataCollection data)
        {
            if (!Session.Authorized)
            {
                return Unauthorized();
            }

            var valueMap = FormDataConverter.Convert(data);

            var status = valueMap.Get("status");

            if (status.IsNullOrWhiteSpace())
            {
                return BadRequest("Stato non inserito o non valido");
            }

            var updated = _userRepo.UpdateUserStatus(Session.User.Id, status);

            return Ok(updated);
        }

        [HttpGet]
        [Route("api/user/profile")]
        public IHttpActionResult GetProfile([FromUri] long? id = null)
        {
            if (!Session.Authorized)
            {
                return Unauthorized();
            }

            var profile = _userRepo.GetProfileById(id ?? Session.User.Id);

            return Ok(profile);
        }

        [HttpPost]
        [Route("api/user/avatar")]
        public IHttpActionResult PostAvatar()
        {
            if (!Session.Authorized)
            {
                return Unauthorized();
            }

            var httpRequest = HttpContext.Current.Request;
            var files = httpRequest.Files;

            if (files.Count == 0)
            {
                return BadRequest("Nessun file caricato");
            }

            const int maxContentLength = 1024 * 1024 * 1; //Size = 1 MB
            var postedFile = files[0];

            if (postedFile.ContentLength <= 0)
            {
                return BadRequest("File non valido");
            }

            try
            {
                IList<string> allowedFileExtensions = new List<string> {".jpg", ".gif", ".png"};
                var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                var extension = ext.ToLower();

                if (!allowedFileExtensions.Contains(extension))
                {
                    return BadRequest("Formato non supportato. Formati supportati: .jpg, .gif, .png");
                }

                if (postedFile.ContentLength > maxContentLength)
                {
                    return BadRequest("Dimensione file eccessiva. Dimensione massima 1 mb");
                }

                var filePath = HttpContext.Current.Server.MapPath("~/Images/Avatar/" + Session.User.Id + extension);
                postedFile.SaveAs(filePath);

                return Created<string>($"~/Images/Avatar/{Session.User.Id}{extension}", null);
            }

            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }
    }
}