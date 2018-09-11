using System;

namespace domain.Entities
{
    [Serializable]
    public class UserLocation
    {
        public enum LocationType
        {
            Start, End
        }
       

        public long LocationId { get; set; }
        public long UserId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Date { get; set; }
        public LocationType Type { get; set; }
    }
}