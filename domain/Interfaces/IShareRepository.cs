using domain.Entities;
using System.Collections.Generic;

namespace domain.Interfaces
{
    public interface IShareRepository
    {
        List<Share> GetUserShares(long userId);
        Share GetShare(long id);
        Share GetUserActiveShare(long userId);
        int Insert(Share share);
        bool SetShareStatus(long shareId, int status);
        bool Delete(long shareId);
        Guest GetGuestById(long shareId, long userId);
        void Insert(Guest guest);
        Guest Complete(Guest guest);
        bool SetGuestStatus(long shareId, long guestId, int status);
        List<Guest> GetShareGuests(long shareId);
    }
}