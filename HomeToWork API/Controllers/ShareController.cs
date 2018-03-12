using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using GoogleApi;
using GoogleApi.Entities.Maps.Directions.Request;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.WebPages;
using HomeToWork.Common;
using HomeToWork.Firebase;
using HomeToWork.Share;
using HomeToWork.User;
using HomeToWork_API.Auth;
using HomeToWork_API.Utils;

namespace HomeToWork_API.Controllers
{
    public class ShareController : ApiController
    {
        private ShareDao shareDao;
        private GuestDao guestDao;

        public ShareController()
        {
            shareDao = new ShareDao();
            guestDao = new GuestDao();
        }

        [HttpGet]
        [Route("api/share/{id}")]
        public IHttpActionResult GetShareById(int id)
        {
            if (!Session.Authorized) return Unauthorized();

            var share = shareDao.GetById(id);

            if (share == null)
                return NotFound();

            return Ok(share);
        }

        [HttpGet]
        [Route("api/share/ongoing")]
        public IHttpActionResult GetOngoingShare()
        {
            if (!Session.Authorized) return Unauthorized();

            var ongoingShare = shareDao.GetOngoinByUserId(Session.User.Id);

            if (ongoingShare.Host.Id == Session.User.Id)
            {
                ongoingShare.Type = ShareType.Driver;
            }
            else
            {
                ongoingShare.Type = ShareType.Guest;
            }

            return Ok(ongoingShare);
        }

        [HttpPost]
        [Route("api/share/new")]
        public IHttpActionResult PostNewShare()
        {
            if (!Session.Authorized) return Unauthorized();

            var share = shareDao.GetOngoinByHostId(Session.User.Id);

            if (share != null)
                return Ok(share);

            share = new Share
            {
                Host = new User {Id = Session.User.Id},
                Time = new DateTime(),
                Guests = new List<Guest>()
            };

            share.Id = shareDao.Insert(share);

            return Ok(share);
        }

        [HttpPost]
        [Route("api/share/{shareId:int}/join")]
        public IHttpActionResult PostJoinShare(int shareId, FormDataCollection data)
        {
            if (!Session.Authorized) return Unauthorized();

            var valueMap = FormDataConverter.Convert(data);
            var startLocation = LatLng.Parse(valueMap.Get("location"));

            var shareGuest = guestDao.GetById(shareId, Session.User.Id);
            if (shareGuest != null)
            {
                if (shareGuest.Status == Guest.Canceled)
                {
                    shareGuest.Status = Guest.Joined;
                    guestDao.Edit(shareGuest);
                }
            }
            else
            {
                shareGuest = new Guest()
                {
                    ShareId = shareId,
                    User = new User {Id = Session.User.Id},
                    StartLatLng = startLocation
                };

                guestDao.Insert(shareGuest);
            }

            var share = shareDao.GetById(shareId);
            var host = share.Host;

            var msgData = new Dictionary<string, string>();
            msgData.Add("TYPE", "SHARE_JOIN_REQUEST");

            FirebaseCloudMessanger.SendMessage(host.Id, msgData);

            share.Type = ShareType.Guest;

            return Ok(share);
        }

        [HttpPost]
        [Route("api/share/{shareId:int}/leave")]
        public IHttpActionResult PostLeaveShare(int shareId)
        {
            if (!Session.Authorized) return Unauthorized();

            var shareGuest = guestDao.GetById(shareId, Session.User.Id);

            if (shareGuest == null)
                return NotFound();

            if (shareGuest.Status == Guest.Canceled)
                return BadRequest();

            shareGuest.Status = Guest.Canceled;
            guestDao.Edit(shareGuest);

            var share = shareDao.GetById(shareId);
            var host = share.Host;

            var msgData = new Dictionary<string, string>();
            msgData.Add("TYPE", "SHARE_LEAVE_REQUEST");
            FirebaseCloudMessanger.SendMessage(host.Id, msgData);

            return Ok(share);
        }

        [HttpPost]
        [Route("api/share/{shareId:int}/complete")]
        public IHttpActionResult PostCompleteShare(int shareId, FormDataCollection data)
        {
            if (!Session.Authorized) return Unauthorized();

            var valueMap = FormDataConverter.Convert(data);
            var endLocation = LatLng.Parse(valueMap.Get("location"));

            var guest = guestDao.GetById(shareId, Session.User.Id);

            if (guest == null)
                return NotFound();

            if (guest.Status == Guest.Canceled)
                return BadRequest();

            var request = new DirectionsRequest()
            {
                Origin = new GoogleApi.Entities.Common.Location(guest.StartLatLng.Latitude,
                    guest.StartLatLng.Longitude),
                Destination = new GoogleApi.Entities.Common.Location(endLocation.Latitude, endLocation.Longitude)
            };
            var result = GoogleMaps.Directions.Query(request);

            guest.EndLatLng = endLocation;
            guest.Distance = result.Routes.First().Legs.First().Distance.Value;
            guestDao.Complete(guest);

            var share = shareDao.GetById(shareId);
            var host = share.Host;

            var msgData = new Dictionary<string, string>();
            msgData.Add("TYPE", "SHARE_COMPLETE_REQUEST");
            FirebaseCloudMessanger.SendMessage(host.Id, msgData);

            var expDao = new UserExpDao();
            expDao.AddExpToUser(guest.User.Id, guest.Distance / 10);

            return Ok(share);
        }


        [HttpPost]
        [Route("api/share/{shareId:int}/finish")]
        public IHttpActionResult PostFinishShare(int shareId)
        {
            if (!Session.Authorized) return Unauthorized();

            var share = shareDao.GetById(shareId);

            if (share == null)
                return NotFound();

            if (share.Host.Id != Session.User.Id)
                return BadRequest();

            share.Status = Share.Completed;
            shareDao.Edit(share);

            var totalDistance = 0;


            foreach (Guest guest in share.Guests)
            {
                totalDistance += guest.Distance;
            }

            var expDao = new UserExpDao();
            expDao.AddExpToUser(share.Host.Id, totalDistance / 10);

            return Ok(share);
        }

        [HttpDelete]
        [Route("api/share/{shareId:int}")]
        public IHttpActionResult PostCancelShare(int shareId)
        {
            if (!Session.Authorized) return Unauthorized();

            var share = shareDao.GetById(shareId);

            if (share == null)
                return NotFound();

            if (share.Status == Share.Canceled)
                return BadRequest();

            share.Status = Share.Canceled;
            shareDao.Edit(share);

            // TODO inviare messaggio ai guest dell'avvenuta cancellazione

            share.Guests.ForEach(guest =>
            {
                var msgData = new Dictionary<string, string>();
                msgData.Add("TYPE", "SHARE_DELETED_REQUEST");
                FirebaseCloudMessanger.SendMessage(guest.User.Id, msgData);
            });

           

            return Ok(share);
        }

        [HttpPost]
        [Route("api/share/{shareId:int}/ban")]
        public IHttpActionResult PostBanUserFromShare(int shareId, FormDataCollection data)
        {
            if (!Session.Authorized) return Unauthorized();

            var valueMap = FormDataConverter.Convert(data);
            var guestId = valueMap.Get("guestId").AsInt();

            var shareGuest = guestDao.GetById(shareId, guestId);

            if (shareGuest == null)
                return NotFound();

            if (shareGuest.Status == Guest.Canceled)
                return BadRequest();

            shareGuest.Status = Guest.Canceled;
            guestDao.Edit(shareGuest);

            var share = shareDao.GetById(shareId);
            var host = share.Host;

            var msgData = new Dictionary<string, string>();
            msgData.Add("TYPE", "SHARE_LEAVE_REQUEST");
            FirebaseCloudMessanger.SendMessage(host.Id, msgData);

            return Ok(share);
        }
    }
}