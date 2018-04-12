using System;
using System.Collections.Generic;
using domain.Entities;

namespace domain.Interfaces
{
    public interface ILocationRespository
    {
        List<Location> GetAllLocations();

        List<Location> GetAllUserLocations(long userId, bool byDate);

        List<Location> GetAllUserLocations(long userId, DayOfWeek weekday, bool byDate);

        bool InsertUserLocation(long userId, double latitude, double longitude, DateTime date);
    }
}
