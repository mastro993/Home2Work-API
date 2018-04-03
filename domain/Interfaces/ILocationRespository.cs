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

        int InsertUserLocation(long userId, Location location);
    }
}
