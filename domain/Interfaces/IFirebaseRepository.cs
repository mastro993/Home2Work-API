namespace domain.Interfaces
{
    public interface IFirebaseRepository
    {
        string GetUserToken(long userId);
        bool SetUserToken(long userId, string token);
        bool UpdateUserToken(long userId, string token);
    }
}
