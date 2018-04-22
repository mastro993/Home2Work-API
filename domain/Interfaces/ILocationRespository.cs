using System;
using System.Collections.Generic;
using domain.Entities;

namespace domain.Interfaces
{
    public interface ILocationRespository
    {

        List<Location> GetAllUserLocations(long userId, bool byDate);

        List<Location> GetAllUserLocations(long userId, DayOfWeek weekday, bool byDate);

        bool InsertUserSCLLocation(long userId, double latitude, double longitude, DateTime date);
        bool InsertUserLastLocation(long userId, double latitude, double longitude, DateTime date);
    }
}
