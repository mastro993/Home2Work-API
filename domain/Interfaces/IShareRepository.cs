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
        Share Edit(Share share);
        bool Delete(long shareId);
        Guest GetGuestById(long shareId, long userId);
        void Insert(Guest guest);
        Guest Complete(Guest guest);
        Guest Edit(Guest guest);
        List<Guest> GetShareGuests(long shareId);
    }
}