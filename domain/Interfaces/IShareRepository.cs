using domain.Entities;
using System.Collections.Generic;

namespace domain.Interfaces
{
    public interface IShareRepository
    {
        List<Share> GetUserShares(long userId, int? page, int? limit);
        Share GetUserShare(long userId, long shareId);
        Share GetUserActiveShare(long userId);
        long CreateShare(long hostId, double latidue, double longitude);
        bool JoinShare(long shareId, long guestId, double latitude, double longitude);
        bool FinishShare(long shareId, double latidue, double longitude, int distance);
        bool CompleteShare(long shareId, long userId, double latitude, double longitude, int distance);
        bool CancelShare(long shareId);
        bool LeaveShare(long shareId, long guestId);
        bool Delete(long shareId);
        Guest GetGuest(long shareId, long userId);
        List<Guest> GetShareGuests(long shareId);
    }
}