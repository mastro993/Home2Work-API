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
using data.Repositories;
using domain.Entities;
using domain.Interfaces;
using HomeToWork_API.Auth;
using HomeToWork_API.Firebase;
using HomeToWork_API.Utils;

namespace HomeToWork_API.Controllers
{
    public class ShareController : ApiController
    {
        private readonly IShareRepository _shareRepo;
        private readonly IUserRepository _userRepo;

        public ShareController()
        {
            _shareRepo = new ShareRepository();
            _userRepo = new UserRepository();
        }

        [HttpGet]
        [Route("api/user/share")]
        public IHttpActionResult GetShares()
        {
            if (!Session.Authorized) return Unauthorized();

            var shares = _shareRepo.GetUserShares(Session.User.Id);

            foreach (var share in shares)
            {
                var distance = 0;
                distance = share.Type == ShareType.Driver
                    ? share.Guests.Sum(guest => guest.Distance)
                    : share.Guests.FindLast(guest => guest.User.Id == Session.User.Id).Distance;

                share.SharedDistance = distance;
            }

            return Ok(shares);
        }

        [HttpGet]
        [Route("api/share/{id}")]
        public IHttpActionResult GetShareById(int id)
        {
            if (!Session.Authorized) return Unauthorized();

            var share = _shareRepo.GetShare(id);

            if (share == null)
                return NotFound();

            var distance = 0;
            distance = share.Type == ShareType.Driver
                ? share.Guests.Sum(guest => guest.Distance)
                : share.Guests.FindLast(guest => guest.User.Id == Session.User.Id).Distance;

            share.SharedDistance = distance;

            return Ok(share);
        }

        [HttpGet]
        [Route("api/share/current")]
        public IHttpActionResult GetOngoingShare()
        {
            if (!Session.Authorized) return Unauthorized();

            var ongoingShare = _shareRepo.GetUserActiveShare(Session.User.Id);

            if (ongoingShare == null)
                return Ok();

            ongoingShare.Type = ongoingShare.Host.Id == Session.User.Id ? ShareType.Driver : ShareType.Guest;

            return Ok(ongoingShare);
        }

        [HttpGet]
        [Route("api/share/{shareId}/guests")]
        public IHttpActionResult GetShareGuests(long shareId)
        {
            if (!Session.Authorized) return Unauthorized();

            var guests = _shareRepo.GetShareGuests(shareId);

            return Ok(guests);
        }

        [HttpPost]
        [Route("api/share/new")]
        public IHttpActionResult PostNewShare()
        {
            if (!Session.Authorized) return Unauthorized();

            var share = _shareRepo.GetUserActiveShare(Session.User.Id);

            if (share != null && share.Host.Id == Session.User.Id)
                return Ok(share);

            share = new Share
            {
                Host = new User {Id = Session.User.Id},
                Time = new DateTime()
            };

            share.Id = _shareRepo.Insert(share);
            share = _shareRepo.GetShare(share.Id);

            return Ok(share);
        }

        [HttpPost]
        [Route("api/share/{shareId:int}/join")]
        public IHttpActionResult PostJoinShare(int shareId, FormDataCollection data)
        {
            if (!Session.Authorized) return Unauthorized();

            var valueMap = FormDataConverter.Convert(data);
            var joinLat = double.Parse(valueMap.Get("joinLat"));
            var joinLng = double.Parse(valueMap.Get("joinLng"));

            var share = _shareRepo.GetShare(shareId);
            if (share == null)
                return NotFound();

            var shareGuest = _shareRepo.GetGuestById(shareId, Session.User.Id);
            if (shareGuest != null)
            {
                if (shareGuest.Status == Guest.Canceled)
                {
                    shareGuest.Status = Guest.Joined;
                    _shareRepo.Edit(shareGuest);
                }
            }
            else
            {
                shareGuest = new Guest()
                {
                    ShareId = shareId,
                    User = new User {Id = Session.User.Id},
                    StartLat = joinLat,
                    StartLng = joinLng
                };

                _shareRepo.Insert(shareGuest);
            }


            var host = share.Host;

            var msgData = new Dictionary<string, string>
            {
                {"TYPE", "SHARE_JOIN"}
            };
            FirebaseCloudMessanger.SendMessage(
                host.Id,
                "Nuovo ospite", Session.User + " si è unito alla condivisione in corso",
                msgData,
                "it.gruppoinfor.hometowork.SHARE_JOIN");

            share.Type = ShareType.Guest;

            return Ok(share);
        }

        [HttpPost]
        [Route("api/share/leave")]
        public IHttpActionResult PostLeaveShare()
        {
            if (!Session.Authorized)
                return Unauthorized();

            var currentShare = _shareRepo.GetUserActiveShare(Session.User.Id);

            if (currentShare == null)
                return Ok(false);

            var guests = _shareRepo.GetShareGuests(currentShare.Id);

            var shareGuest = guests.Find(guest => guest.User.Id == Session.User.Id);

            shareGuest.Status = Guest.Canceled;
            _shareRepo.Edit(shareGuest);

            var host = currentShare.Host;

            var msgData = new Dictionary<string, string>
            {
                {"TYPE", "SHARE_LEAVED"}
            };
            FirebaseCloudMessanger.SendMessage(
                host.Id,
                "Condivisione abbandonata", Session.User + " ha abbandonato la condivisione in corso",
                msgData,
                "it.gruppoinfor.hometowork.SHARE_LEAVED");

            return Ok(true);
        }

        [HttpPost]
        [Route("api/share/complete")]
        public IHttpActionResult PostCompleteShare(FormDataCollection data)
        {
            if (!Session.Authorized) return Unauthorized();

            var valueMap = FormDataConverter.Convert(data);
            var completeLat = double.Parse(valueMap.Get("completeLat"));
            var completeLng = double.Parse(valueMap.Get("completeLng"));

            var currentShare = _shareRepo.GetUserActiveShare(Session.User.Id);

            if (currentShare == null)
                return Ok(false);

            var guests = _shareRepo.GetShareGuests(currentShare.Id);

            var shareGuest = guests.Find(guest => guest.User.Id == Session.User.Id);

            if (shareGuest == null)
                return Ok(false);

            var request = new DirectionsRequest()
            {
                Origin = new GoogleApi.Entities.Common.Location(shareGuest.StartLat, shareGuest.StartLng),
                Destination = new GoogleApi.Entities.Common.Location(completeLat, completeLng)
            };
            var result = GoogleMaps.Directions.Query(request);

            shareGuest.EndLat = completeLat;
            shareGuest.EndLng = completeLng;
            shareGuest.Distance = result.Routes.First().Legs.First().Distance.Value;
            _shareRepo.Complete(shareGuest);

            var host = currentShare.Host;

            var msgData = new Dictionary<string, string>
            {
                {"TYPE", "SHARE_COMPLETED"}
            };
            FirebaseCloudMessanger.SendMessage(
                host.Id,
                "Condivisione completata",
                Session.User + " ha completato la condivisione percorrendo " + shareGuest.Distance / 1000.0 + " Km",
                msgData,
                "it.gruppoinfor.hometowork.SHARE_COMPLETED");

            _userRepo.AddExpToUser(shareGuest.User.Id, shareGuest.Distance / 10);

            return Ok(true);
        }


        [HttpPost]
        [Route("api/share/finish")]
        public IHttpActionResult PostFinishShare()
        {
            if (!Session.Authorized)
                return Unauthorized();

            var share = _shareRepo.GetUserActiveShare(Session.User.Id);

            if (share == null)
                return NotFound();

            if (share.Host.Id != Session.User.Id)
                return BadRequest();

            share.Status = ShareStatus.Completed;
            _shareRepo.Edit(share);

            var totalDistance = 0;

            var guests = _shareRepo.GetShareGuests(share.Id);

            foreach (var guest in guests)
            {
                totalDistance += guest.Distance;
            }

            _userRepo.AddExpToUser(share.Host.Id, totalDistance / 10);

            return Ok(true);
        }

        [HttpDelete]
        [Route("api/share")]
        public IHttpActionResult PostCancelShare()
        {
            if (!Session.Authorized)
                return Unauthorized();

            var share = _shareRepo.GetUserActiveShare(Session.User.Id);

            if (share == null)
                return NotFound();

            if (share.Host.Id != Session.User.Id)
                return NotFound();

            if (share.Status == ShareStatus.Canceled)
                return NotFound();

            share.Status = ShareStatus.Canceled;
            _shareRepo.Edit(share);

            var guests = _shareRepo.GetShareGuests(share.Id);


            guests.ForEach(guest =>
            {
                var msgData = new Dictionary<string, string>
                {
                    {"TYPE", "SHARE_CANCELED"}
                };
                FirebaseCloudMessanger.SendMessage(
                    guest.User.Id,
                    "Condivisione annullata", Session.User + " ha annullato la condivisione in corso",
                    msgData,
                    "it.gruppoinfor.hometowork.SHARE_CANCELED");
            });


            return Ok(true);
        }

        [HttpPost]
        [Route("api/share/ban")]
        public IHttpActionResult PostBanUserFromShare(FormDataCollection data)
        {
            if (!Session.Authorized) return Unauthorized();

            var valueMap = FormDataConverter.Convert(data);
            var guestId = valueMap.Get("guestId").AsInt();

            var share = _shareRepo.GetUserActiveShare(Session.User.Id);

            if (share.Host.Id != Session.User.Id)
                return NotFound();

            var shareGuest = _shareRepo.GetGuestById(share.Id, guestId);

            if (shareGuest == null)
                return NotFound();

            if (shareGuest.Status == Guest.Canceled)
                return NotFound();

            shareGuest.Status = Guest.Canceled;
            _shareRepo.Edit(shareGuest);

            var msgData = new Dictionary<string, string>
            {
                {"TYPE", "SHARE_BAN"}
            };
            FirebaseCloudMessanger.SendMessage(
                guestId,
                "Sei stato espulso", Session.User + " ti ha espulso dalla condivisione in corso",
                msgData,
                "it.gruppoinfor.hometowork.SHARE_BAN");

            return Ok(true);
        }
    }
}