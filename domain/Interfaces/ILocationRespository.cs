using System;
using System.Collections.Generic;
using domain.Entities;

namespace domain.Interfaces
{
    public interface ILocationRespository
    {
        List<Location> GetUserLocations(long userId);

        List<Location> GetCompanyLocations(long userId);


        List<Location> GetUserLocations(long userId, DayOfWeek weekday, bool byDate);
        List<Location> GetUserLocationsInRange(long userId, double centerLat, double centerLng,
            int radiusInMeters);


        bool InsertUserSCLLocation(long userId, double latitude, double longitude, DateTime date);
        bool InsertUserLastLocation(long userId, double latitude, double longitude, DateTime date);
    }
}