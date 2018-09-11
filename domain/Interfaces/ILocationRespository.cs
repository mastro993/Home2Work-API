using System;
using System.Collections.Generic;
using domain.Entities;

namespace domain.Interfaces
{
    public interface ILocationRespository
    {
        List<UserLocation> GetUserLocations(long userId);

        List<UserLocation> GetCompanyLocations(long userId);


        List<UserLocation> GetUserLocations(long userId, DayOfWeek weekday, bool byDate);
        List<UserLocation> GetUserLocationsInRange(long userId, double centerLat, double centerLng,
            int radiusInMeters);


        bool InsertUserSCLLocation(long userId, double latitude, double longitude, DateTime date, UserLocation.LocationType locationType);
        bool InsertUserLastLocation(long userId, double latitude, double longitude, DateTime date);
    }
}