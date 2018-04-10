using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using data.Common;
using data.Repositories;
using domain.Entities;
using domain.Interfaces;
using HomeToWork_API.Auth;
using HomeToWork_API.Utils;
using Microsoft.Ajax.Utilities;

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
        [Route("api/user/register")]
        public IHttpActionResult Register(FormDataCollection data)
        {
            return NotFound();

            //var valueMap = FormDataConverter.Convert(data);
            //var email = valueMap.Get("email");
            //var password = valueMap.Get("password");

            //var userId = _userRepo.Register(email, password);

            //if (userId != 0)
            //{
            //    return Created(new Uri("api/user/" + userId), User);
            //}

           // return InternalServerError();
        }


        [HttpPost]
        [Route("api/user/login")]
        [ResponseType(typeof(User))]
        public HttpResponseMessage Login(FormDataCollection data)
        {
            var valueMap = FormDataConverter.Convert(data);

            var email = valueMap.Get("email");
            var password = valueMap.Get("password");

            if (email.IsNullOrWhiteSpace() || password.IsNullOrWhiteSpace())
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var user = _userRepo.Login(email, password);

            if (user == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            var sessionToken = _userRepo.NewSessionToken(user.Id);

            var response = Request.CreateResponse(HttpStatusCode.OK, user);
            response.Headers.Add("X-User-Session-Token", sessionToken);
            return response;
        }

        [HttpGet]
        [Route("api/user")]
        public IHttpActionResult Get()
        {
            if (Session.Authorized) return Unauthorized();

            return Ok(Session.User);
        }

        [HttpPut]
        [Route("api/user")]
        public IHttpActionResult Put(User user)
        {

            return NotFound();

            //if (!Session.Authorized) return Unauthorized();
            //user = _userRepo.Edit(user);

            //return Ok(user);
        }

        [HttpPost]
        [Route("api/user/avatar")]
        public IHttpActionResult PostAvatar()
        {
            if (!Session.Authorized) return Unauthorized();

            var dict = new Dictionary<string, object>();
            const int maxContentLength = 1024 * 1024 * 1; //Size = 1 MB
            try
            {
                var httpRequest = HttpContext.Current.Request;

                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    if (postedFile == null || postedFile.ContentLength <= 0)
                        return BadRequest();

                    IList<string> allowedFileExtensions = new List<string> {".jpg", ".gif", ".png"};
                    var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                    var extension = ext.ToLower();

                    if (!allowedFileExtensions.Contains(extension))
                    {
                        dict.Add("error", "Please Upload image of type .jpg,.gif,.png.");
                        return BadRequest();
                    }

                    if (postedFile.ContentLength > maxContentLength)
                    {
                        dict.Add("error", "Please Upload a file upto 1 mb.");
                        return BadRequest();
                    }

                    var filePath = HttpContext.Current.Server.MapPath("~/Images/Avatar/" + Session.User.Id + extension);
                    postedFile.SaveAs(filePath);

                    return Created<string>($"~/Images/Avatar/{Session.User.Id}{extension}", null);
                }

                dict.Add("error", "Please Upload a image.");
                return NotFound();
            }

            catch (Exception ex)
            {
                var res = string.Format(ex.ToString());
                dict.Add("error", res);
                return NotFound();
            }
        }


        [HttpPost]
        [Route("api/user/fcmtoken")]
        public IHttpActionResult PostFcmToken(FormDataCollection data)
        {
            if (!Session.Authorized) return Unauthorized();

            var fcmTokenDao = new FcmTokenRepository();

            var valueMap = FormDataConverter.Convert(data);
            var newToken = valueMap.Get("token");

            var token = fcmTokenDao.GetUserToken(Session.User.Id);

            if (token.IsNullOrWhiteSpace())
            {
                var inserted = fcmTokenDao.SetUserToken(Session.User.Id, newToken);
                return Ok(inserted);
            }

            if (token.Equals(newToken)) return Ok(true);

            var updated = fcmTokenDao.UpdateUserToken(Session.User.Id, newToken);
            return Ok(updated);

        }

        [HttpGet]
        [Route("api/user/profile")]
        public IHttpActionResult GetProfile()
        {
            if (!Session.Authorized) return Unauthorized();

            var exp = _userRepo.GetUserExp(Session.User.Id);
            var stats = _userRepo.GetUserStats(Session.User.Id);
            var activity = _userRepo.GetUserMonthlyActivity(Session.User.Id);

            var profile = new UserProfile()
            {
                Exp = exp,
                Stats = stats,
                Activity = activity,
                Regdate = Session.User.Regdate
            };

            return Ok(profile);
        }


        [HttpGet]
        [Route("api/user/{userId:int}")]
        public IHttpActionResult GetUserById(int userId)
        {
            if (!Session.Authorized) return Unauthorized();

            var user = _userRepo.GetById(userId);

            if (user != null) return Ok(user);

            return NotFound();
        }

        [HttpGet]
        [Route("api/user/{userId:int}/profile")]
        public IHttpActionResult GetUserProfileById(int userId)
        {
            if (!Session.Authorized) return Unauthorized();

            var user = _userRepo.GetById(userId);
            if (user == null)
                return NotFound();

            var exp = _userRepo.GetUserExp(user.Id);
            var stats = _userRepo.GetUserStats(user.Id);
            var activity = _userRepo.GetUserMonthlyActivity(user.Id);

            var profile = new UserProfile()
            {
                Exp = exp,
                Stats = stats,
                Activity = activity,
                Regdate = user.Regdate
            };

            return Ok(profile);
        }

        [HttpGet]
        [Route("api/user/list")]
        public IHttpActionResult GetUserList()
        {
            if (!Session.Authorized) return Unauthorized();

            var userList = _userRepo.GetAll();
            return Ok(userList);
        }
    }
}