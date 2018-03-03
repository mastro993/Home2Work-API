using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using HomeToWork_API.Auth;

namespace HomeToWork_API.Handler
{
    public class MessageHandler : DelegatingHandler
    {
        /**
         * Intercetta l'header necessario ad autenticare l'utente
         */
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains("X-User-Session-Token"))
            {
                Session.Token = null;
                Session.User = null;
                return await base.SendAsync(request, cancellationToken);
            }


            // Salvo il session token e autentico l'utente
            var headerValues = request.Headers.GetValues("X-User-Session-Token");
            Session.Token = headerValues.FirstOrDefault();
            Session.UserSessionLogin();

            return await base.SendAsync(request, cancellationToken);
        }
    }
}