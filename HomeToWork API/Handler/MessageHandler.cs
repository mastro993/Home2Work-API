using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using data.Repositories;
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
            if (!request.Headers.Contains("X-Api-Key"))
            {
                return request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Chiave API non impostata");
            }
                
            // TODO controllo chiave api abilitata

            if (!request.Headers.Contains("X-User-Session-Token"))
            {
                return await base.SendAsync(request, cancellationToken);
            }
               
            // Autenticazione utente tramite token sessione
            var headerValues = request.Headers.GetValues("X-User-Session-Token");
            var token = headerValues.FirstOrDefault();

            var userRepo = new UserRepository();
            Session.User = userRepo.GetBySessionToken(token);

            return await base.SendAsync(request, cancellationToken);

        }
    }
}