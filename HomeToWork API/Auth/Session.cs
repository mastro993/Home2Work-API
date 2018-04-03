
using data.Repositories;
using domain.Entities;
using domain.Interfaces;
using Microsoft.Ajax.Utilities;

namespace HomeToWork_API.Auth
{
    public class Session
    {

        public static string Token { get; set; }

        public static bool Authorized => !Token.IsNullOrWhiteSpace();

        public static User User { get; set; }

        public static void UserSessionLogin()
        {
            if (!Authorized) return;

            var userRepo = new UserRepository();
            User = userRepo.GetBySessionToken(Token);

            if (User == null) Token = null;
        }
    }
}