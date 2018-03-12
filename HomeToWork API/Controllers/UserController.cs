using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using HomeToWork.Chat;
using HomeToWork.Firebase;
using HomeToWork.Location;
using HomeToWork.Match;
using HomeToWork.Share;
using HomeToWork.User;
using HomeToWork_API.Auth;
using HomeToWork_API.Utils;
using Microsoft.Ajax.Utilities;

namespace HomeToWork_API.Controllers
{
    public class UserController : ApiController
    {
        private readonly UserDao userDao;

        public UserController()
        {
            userDao = new UserDao();
        }

        [HttpGet]
        [Route("api/user")]
        public IHttpActionResult Get()
        {
            if (!Session.Authorized) return Unauthorized();

            return Ok(Session.User);
        }

        [HttpPut]
        [Route("api/user")]
        public IHttpActionResult Put(User user)
        {
            if (!Session.Authorized) return Unauthorized();
            user = userDao.Edit(user);

            return Ok(user);
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

            var fcmTokenDao = new FcmTokenDao();

            var valueMap = FormDataConverter.Convert(data);
            var newToken = valueMap.Get("token");

            var token = fcmTokenDao.GetUserToken(Session.User.Id);

            if (token.IsNullOrWhiteSpace())
            {
                var inserted = fcmTokenDao.SetUserToken(Session.User.Id, newToken);
                if (inserted) return Ok();
            }
            else if (!token.Equals(newToken))
            {
                var updated = fcmTokenDao.UpdateUserToken(Session.User.Id, newToken);
                if (updated) return Ok();
            }
            else
            {
                return Ok();
            }


            return InternalServerError();
        }

        [HttpGet]
        [Route("api/user/profile")]
        public IHttpActionResult GetProfile()
        {
            if (!Session.Authorized) return Unauthorized();

            var expDao = new UserExpDao();
            var statsDao = new UserStatsDao();

            var exp = expDao.GetUserExp(Session.User.Id);
            var stats = statsDao.GetUserStats(Session.User.Id);
            var activity = statsDao.GetUserMonthlyActivity(Session.User.Id);

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
        [Route("api/user/chat")]
        public IHttpActionResult GetChatList()
        {
            if (!Session.Authorized) return Unauthorized();

            var chatDao = new ChatDao();

            var chats = chatDao.GetUserChatList(Session.User.Id);
            return Ok(chats);
        }

        [HttpPost]
        [Route("api/user/location")]
        public IHttpActionResult PostUserLocations(IEnumerable<Location> locations)
        {
            if (!Session.Authorized) return Unauthorized();

            var locationDao = new LocationDao();

            var locationList = locations.ToList();
            try
            {
                foreach (var location in locationList)
                {
                    location.LocationId = locationDao.InsertUserLocation(Session.User.Id, location);
                }
            }
            catch (Exception e)
            {
                return InternalServerError();
            }

            return Ok(locationList);
        }

        [HttpGet]
        [Route("api/user/match")]
        public IHttpActionResult GetMatches()
        {
            if (!Session.Authorized) return Unauthorized();

            var matchDao = new MatchDao();

            var matches = matchDao.GetByUserId(Session.User.Id);

            if (matches.Count == 0)
                matches = Matcher.GetAffineUsers(Session.User.Id);

            return Ok(matches);
        }

        [HttpGet]
        [Route("api/user/share")]
        public IHttpActionResult GetShares()
        {
            if (!Session.Authorized) return Unauthorized();

            var shareDao = new ShareDao();

            var shares = shareDao.GetByUserID(Session.User.Id);

            return Ok(shares);
        }


        /// ########################################################################################
        [HttpGet]
        [Route("api/user/{userId:int}")]
        public IHttpActionResult GetUserById(int userId)
        {
            if (!Session.Authorized) return Unauthorized();

            var user = userDao.GetById(userId);

            if (user != null) return Ok(user);

            return NotFound();
        }

        [HttpGet]
        [Route("api/user/{userId:int}/profile")]
        public IHttpActionResult GetUserProfileById(int userId)
        {
            if (!Session.Authorized) return Unauthorized();

            var user = userDao.GetById(userId);
            if (user == null)
                return NotFound();

            var expDao = new UserExpDao();
            var statsDao = new UserStatsDao();

            var exp = expDao.GetUserExp(user.Id);
            var stats = statsDao.GetUserStats(user.Id);
            var activity = statsDao.GetUserMonthlyActivity(user.Id);

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

            var userList = userDao.GetAll();
            return Ok(userList);
        }
    }
}