using System;

namespace domain.Entities
{
    [Serializable]
    public class Location
    {
        public long LocationId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }
    }
}