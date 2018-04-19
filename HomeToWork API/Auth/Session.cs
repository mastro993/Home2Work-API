using data.Repositories;
using domain.Entities;
using domain.Interfaces;
using Microsoft.Ajax.Utilities;

namespace HomeToWork_API.Auth
{
    public class Session
    {
        public static User User { get; set; }

        public static bool Authorized => User != null;

    }
}