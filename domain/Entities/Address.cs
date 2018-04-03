using System;

namespace domain.Entities
{
    [Serializable]
    public class Address
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Region { get; set; }
        public string District { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        
        
        
    }
}