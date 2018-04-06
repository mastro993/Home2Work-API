using System.Collections.Generic;
using domain.Entities;

namespace domain.Interfaces
{
    public interface IUserRepository
    {
  
        User Login(string email, string password);
        User GetById(long userId);
        List<User> GetAll();
        string NewSessionToken(long userId);    
        User GetBySessionToken(string sessionToken);
        UserExp GetUserExp(long userId);
        bool AddExpToUser(long userId, long exp);
        UserStats GetUserStats(long userId);

        Dictionary<int, SharingActivity> GetUserMonthlyActivity(long userId);
    }
}