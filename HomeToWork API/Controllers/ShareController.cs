using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using GoogleApi;
using GoogleApi.Entities.Maps.Directions.Request;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web.Http;
using System.Web.WebPages;
using data.Repositories;
using domain.Entities;
using domain.Interfaces;
using HomeToWork_API.Auth;
using HomeToWork_API.Firebase;
using HomeToWork_API.Utils;
using Microsoft.Ajax.Utilities;

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
        [Route("api/share/list")]
        public IHttpActionResult GetShares(
            [FromUri] int page = 1,
            [FromUri] int limit = int.MaxValue)
        {
            if (!Session.Authorized)
            {
                return Unauthorized();
            }

            var shares = _shareRepo.GetUserShares(Session.User.Id, page, limit);

            return Ok(shares);
        }

        [HttpPost]
        [Route("api/share/new")]
        public IHttpActionResult PostNewShare(FormDataCollection data)
        {
            if (!Session.Authorized)
            {
                return Unauthorized();
            }

            // Controllo se l'utente non ha già una condivisione in corso, nel caso la restituisco
            var share = _shareRepo.GetUserActiveShare(Session.User.Id);

            if (share != null && share.Host.Id == Session.User.Id)
            {
                return Ok(share);
            }
                
            // Altrimenti ne creo una nuova
            var valueMap = FormDataConverter.Convert(data);

            var lat = valueMap.Get("startLat");
            var lng = valueMap.Get("startLng");

            if (lat.IsNullOrWhiteSpace())
            {
                return BadRequest("Latitudine iniziale mancante");
            }


            if (lng.IsNullOrWhiteSpace())
            {
                return BadRequest("Longitudine iniziale mancante");
            }

            double startLat;
            double startLng;

            try
            {
                startLat = double.Parse(lat);
            }
            catch (Exception e)
            {
                return BadRequest("Formato latitudine non corretto");
            }

            try
            {
                startLng = double.Parse(lng);
            }
            catch (Exception e)
            {
                return BadRequest("Formato longitudine non corretto");
            }
           

            var shareId = _shareRepo.CreateShare(Session.User.Id, startLat, startLng);

            share = _shareRepo.GetUserShare(Session.User.Id, shareId);

            return Ok(share);
        }

        [HttpGet]
        [Route("api/share/{id}")]
        public IHttpActionResult GetShare(int id)
        {
            if (!Session.Authorized)
            {
                return Unauthorized();
            }

            var share = _shareRepo.GetUserShare(Session.User.Id, id);

            if (share == null)
            {
                return NotFound();
            }  

            return Ok(share);


        }

        [HttpGet]
        [Route("api/share/current")]
        public IHttpActionResult GetCurrentShare()
        {
            if (!Session.Authorized)
            {
                return Unauthorized();
            }

            var ongoingShare = _shareRepo.GetUserActiveShare(Session.User.Id);

            if (ongoingShare == null)
            {
                return NotFound();
            }
               

            return Ok(ongoingShare);
        }

        [HttpGet]
        [Route("api/share/{shareId}/guests")]
        public IHttpActionResult GetShareGuests(long shareId)
        {
            if (!Session.Authorized)
            {
                return Unauthorized();
            }

            var guests = _shareRepo.GetShareGuests(shareId);

            return Ok(guests);
        }


        [HttpPost]
        [Route("api/share/{shareId:int}/join")]
        public IHttpActionResult PostJoinShare(int shareId, FormDataCollection data)
        {
            if (!Session.Authorized)
            {
                return Unauthorized();
            }

            var valueMap = FormDataConverter.Convert(data);

            var lat = valueMap.Get("joinLat");
            var lng = valueMap.Get("joinLng");

            if (lat.IsNullOrWhiteSpace())
            {
                return BadRequest("Latitudine iniziale mancante");
            }


            if (lng.IsNullOrWhiteSpace())
            {
                return BadRequest("Longitudine iniziale mancante");
            }

            double joinLat;
            double joinLng;

            try
            {
                joinLat = double.Parse(lat);
            }
            catch (Exception e)
            {
                return BadRequest("Formato latitutudine non corretto");
            }

            try
            {
                joinLng = double.Parse(lng);
            }
            catch (Exception e)
            {
                return BadRequest("Formato longitudine non corretto");
            }

            var share = _shareRepo.GetUserShare(Session.User.Id, shareId);
            if (share == null)
            {
                return NotFound();
            }
                

            var shareGuest = _shareRepo.GetGuest(shareId, Session.User.Id);
            if (shareGuest != null)
            {
                if (shareGuest.CurrentStatus == Guest.Status.Leaved)
                {
                    shareGuest.CurrentStatus = Guest.Status.Joined;
                    _shareRepo.JoinShare(shareId, shareGuest.User.Id, joinLat, joinLng);
                }
            }
            else
            {
                _shareRepo.JoinShare(shareId, Session.User.Id, joinLat, joinLng);
            }

            var msgData = new Dictionary<string, string>
            {
                {"TYPE", "SHARE_JOIN"}
            };

            #pragma warning disable 4014
            FirebaseCloudMessanger.SendMessage(
                share.Host.Id,
                "Nuovo ospite", Session.User + " si è unito alla condivisione in corso",
                msgData,
                "it.gruppoinfor.hometowork.SHARE_JOIN");
            #pragma warning restore 4014

            share = _shareRepo.GetUserShare(Session.User.Id, shareId);

            return Ok(share);
        }

        [HttpPost]
        [Route("api/share/leave")]
        public IHttpActionResult PostLeaveShare()
        {
            if (!Session.Authorized)
            {
                return Unauthorized();
            }

            var share = _shareRepo.GetUserActiveShare(Session.User.Id);

            if (share == null)
            {
                return NotFound();
            }
                

            var shareGuest = _shareRepo.GetGuest(share.Id, Session.User.Id);

            if (shareGuest == null)
            {
                return NotFound();
            }
               

            var leaved = _shareRepo.LeaveShare(share.Id, shareGuest.User.Id);

            if (!leaved)
            {
                return Ok(false);
            }

            var msgData = new Dictionary<string, string>
            {
                {"TYPE", "SHARE_LEAVED"}
            };

#pragma warning disable 4014
            FirebaseCloudMessanger.SendMessage(
                share.Host.Id,
                "Condivisione abbandonata", Session.User + " ha abbandonato la condivisione in corso",
                msgData,
                "it.gruppoinfor.hometowork.SHARE_LEAVED");
#pragma warning restore 4014

            return Ok(true);
        }

        [HttpPost]
        [Route("api/share/complete")]
        public IHttpActionResult PostCompleteShare(FormDataCollection data)
        {
            if (!Session.Authorized) return Unauthorized();

            var currentShare = _shareRepo.GetUserActiveShare(Session.User.Id);

            if (currentShare == null)
            {
                return NotFound();
            }

            var guest = _shareRepo.GetGuest(currentShare.Id, Session.User.Id);

            if (guest == null)
            {
                return NotFound();
            }


            var valueMap = FormDataConverter.Convert(data);

            var lat = valueMap.Get("completeLat");
            var lng = valueMap.Get("completaLng");

            if (lat.IsNullOrWhiteSpace())
            {
                return BadRequest("Latitudine finale mancante");
            }


            if (lng.IsNullOrWhiteSpace())
            {
                return BadRequest("Longitudine finale mancante");
            }

            double completeLat;
            double completeLng;

            try
            {
                completeLat = double.Parse(lat);
            }
            catch (Exception e)
            {
                return BadRequest("Formato latitutudine non corretto");
            }

            try
            {
                completeLng = double.Parse(lng);
            }
            catch (Exception e)
            {
                return BadRequest("Formato longitudine non corretto");
            }

            var request = new DirectionsRequest()
            {
                Origin = new GoogleApi.Entities.Common.Location(guest.StartLat, guest.StartLng),
                Destination = new GoogleApi.Entities.Common.Location(completeLat, completeLng)
            };
            var result = GoogleMaps.Directions.Query(request);
            var distance = result.Routes.First().Legs.First().Distance.Value;

            _shareRepo.CompleteShare(currentShare.Id, Session.User.Id, completeLat, completeLng, distance);

            var host = currentShare.Host;

            var msgData = new Dictionary<string, string>
            {
                {"TYPE", "SHARE_COMPLETED"}
            };

            #pragma warning disable 4014
            FirebaseCloudMessanger.SendMessage(
                host.Id,
                "Condivisione completata",
                Session.User + " ha completato la condivisione percorrendo " + distance / 1000.0 + " Km",
                msgData,
                "it.gruppoinfor.hometowork.SHARE_COMPLETED");
            #pragma warning restore 4014


            _userRepo.AddExpToUser(guest.User.Id, guest.Distance / 100);

            var completedShare = _shareRepo.GetGuest(currentShare.Id, Session.User.Id);

            return Ok(completedShare);
        }

        [HttpPost]
        [Route("api/share/finish")]
        public IHttpActionResult PostFinishShare(FormDataCollection data)
        {
            if (!Session.Authorized)
            {
                return Unauthorized();
            }

            var share = _shareRepo.GetUserActiveShare(Session.User.Id);

            if (share == null)
            {

            }

            var valueMap = FormDataConverter.Convert(data);

            var lat = valueMap.Get("finishLat");
            var lng = valueMap.Get("finishLng");

            if (lat.IsNullOrWhiteSpace())
            {
                return BadRequest("Latitudine finale mancante");
            }


            if (lng.IsNullOrWhiteSpace())
            {
                return BadRequest("Longitudine finale mancante");
            }

            double finishLat;
            double finishLng;

            try
            {
                finishLat = double.Parse(lat);
            }
            catch (Exception e)
            {
                return BadRequest("Formato latitutudine non corretto");
            }

            try
            {
                finishLng = double.Parse(lng);
            }
            catch (Exception e)
            {
                return BadRequest("Formato longitudine non corretto");
            }



            if (share.Host.Id != Session.User.Id)
                return BadRequest();


            var request = new DirectionsRequest()
            {
                Origin = new GoogleApi.Entities.Common.Location(share.StartLat, share.StartLng),
                Destination = new GoogleApi.Entities.Common.Location(finishLat, finishLng)
            };
            var result = GoogleMaps.Directions.Query(request);
            var distance = result.Routes.First().Legs.First().Distance.Value;

            _shareRepo.FinishShare(share.Id, finishLat, finishLng, distance);

            var totalDistance = 0;

            var guests = _shareRepo.GetShareGuests(share.Id);

            foreach (var guest in guests)
            {
                totalDistance += guest.Distance;
            }

            _userRepo.AddExpToUser(share.Host.Id, totalDistance / 100);

            return Ok(share);
        }

        [HttpDelete]
        [Route("api/share")]
        public IHttpActionResult PostCancelShare()
        {
            if (!Session.Authorized)
            {
                return Unauthorized();
            }

            var share = _shareRepo.GetUserActiveShare(Session.User.Id);

            if (share == null)
            {
                return NotFound();
            }

            if (share.Host.Id != Session.User.Id)
            {
                return NotFound();
            }

            if (share.Status == Status.Canceled)
            {
                return NotFound();
            }

            _shareRepo.CancelShare(share.Id);

            var guests = _shareRepo.GetShareGuests(share.Id);

            guests.ForEach(guest =>
            {
                var msgData = new Dictionary<string, string>
                {
                    {"TYPE", "SHARE_CANCELED"}
                };
                #pragma warning disable 4014
                FirebaseCloudMessanger.SendMessage(
                    guest.User.Id,
                    "Condivisione annullata", Session.User + " ha annullato la condivisione in corso",
                    msgData,
                    "it.gruppoinfor.hometowork.SHARE_CANCELED");
                #pragma warning restore 4014
            });


            return Ok(true);
        }

        [HttpPost]
        [Route("api/share/ban")]
        public IHttpActionResult PostBanUserFromShare(FormDataCollection data)
        {
            if (!Session.Authorized)
            {
                return Unauthorized();
            }

            var valueMap = FormDataConverter.Convert(data);
            var guestId = valueMap.Get("guestId").AsInt();

            var share = _shareRepo.GetUserActiveShare(Session.User.Id);

            if (share.Host.Id != Session.User.Id)
            {
                return NotFound();
            }

            var shareGuest = _shareRepo.GetGuest(share.Id, guestId);

            if (shareGuest == null)
            {
                return NotFound();
            }

            if (shareGuest.CurrentStatus == Guest.Status.Leaved)
            {
                return NotFound();
            }

            shareGuest.CurrentStatus = Guest.Status.Leaved;

            _shareRepo.LeaveShare(share.Id, shareGuest.User.Id);

            var msgData = new Dictionary<string, string>
            {
                {"TYPE", "SHARE_BAN"}
            };

            #pragma warning disable 4014
            FirebaseCloudMessanger.SendMessage(
                guestId,
                "Sei stato espulso", Session.User + " ti ha espulso dalla condivisione in corso",
                msgData,
                "it.gruppoinfor.hometowork.SHARE_BAN");
            #pragma warning restore 4014

            return Ok(true);
        }
    }
}