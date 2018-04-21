using System.Collections.Generic;
using domain.Entities;

namespace domain.Interfaces
{
    public interface IUserRepository
    {

        string GetUserSalt(string email);
        User Login(string email, string passwordHash);
        User GetById(long userId);
        UserProfile GetProfileById(long userId);
        List<User> GetAll();
        string NewSessionToken(long userId);    
        User GetBySessionToken(string sessionToken);
        UserKarma GetUserKarma(long userId);
        bool AddExpToUser(long userId, long exp);
        UserStats GetUserStats(long userId);
        Dictionary<string, SharingActivity> GetUserMonthlyActivity(long userId);

        bool UpdateUserStatus(long userId, string status);

        bool HideUserStatus(long userId);

        ProfileStatus GetUserStatus(long userId);
    }
}