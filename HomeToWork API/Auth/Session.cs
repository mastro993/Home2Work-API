using HomeToWork.User;
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

            var userDao = new UserDao();
            User = userDao.GetBySessionToken(Token);

            if (User == null) Token = null;
        }
    }
}