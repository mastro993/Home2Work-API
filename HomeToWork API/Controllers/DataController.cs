using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Mvc;
using data.Repositories;
using HomeToWork_API.Auth;

namespace HomeToWork_API.Controllers
{
    public class DataController : ApiController
    {

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/data/user_locations")]
        public HttpResponseMessage GetUserLocations(
            [FromUri] long? userId = null,
            [FromUri] int? year = null,
            [FromUri] int? month = null
            )
        {
            if (!Session.Authorized)
            {
                var response = Request.CreateResponse(HttpStatusCode.Unauthorized);
                return response;
            }

            if (userId.HasValue)
            {
                var locationRepo = new LocationRepository();
                var locations = locationRepo.GetUserLocations(userId.Value);

                string csv = string.Empty;

                csv += "Latitude" + ",";
                csv += "Longitude" + ",";
                csv += "Time";

                csv += "\r\n";


                foreach (var location in locations)
                {

                    csv += location.Latitude.ToString(CultureInfo.InvariantCulture) + ",";
                    csv += location.Longitude.ToString(CultureInfo.InvariantCulture) + ",";
                    csv += location.Date.ToString(CultureInfo.InvariantCulture);

                    //Add new line.
                    csv += "\r\n";
                }

                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                writer.Write(csv);
                writer.Flush();
                stream.Position = 0;

                var result = new HttpResponseMessage(HttpStatusCode.OK) {Content = new StreamContent(stream)};
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "Export.csv" };
                return result;



            }
            else
            {
                var response = Request.CreateResponse(HttpStatusCode.BadRequest);
                return response;
            }

            
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/data/company_locations")]
        public HttpResponseMessage GetCompanyLocations(
           [FromUri] long? companyId = null,
           [FromUri] int? year = null,
           [FromUri] int? month = null
           )
        {
            if (!Session.Authorized)
            {
                var response = Request.CreateResponse(HttpStatusCode.Unauthorized);
                return response;
            }

            if (companyId.HasValue)
            {
                var locationRepo = new LocationRepository();
                var locations = locationRepo.GetCompanyLocations(companyId.Value);

                string csv = string.Empty;

                csv += "Latitude" + ",";
                csv += "Longitude" + ",";
                csv += "Time";

                csv += "\r\n";


                foreach (var location in locations)
                {

                    csv += location.Latitude.ToString(CultureInfo.InvariantCulture) + ",";
                    csv += location.Longitude.ToString(CultureInfo.InvariantCulture) + ",";
                    csv += location.Date.ToString(CultureInfo.InvariantCulture);

                    //Add new line.
                    csv += "\r\n";
                }

                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                writer.Write(csv);
                writer.Flush();
                stream.Position = 0;

                var result = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(stream) };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "Export.csv" };
                return result;



            }
            else
            {
                var response = Request.CreateResponse(HttpStatusCode.BadRequest);
                return response;
            }


        }


    }
}
