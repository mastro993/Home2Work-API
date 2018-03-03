using System;

namespace HomeToWork.Common
{
    [Serializable]
    public class Address
    {
        public string Cap { get; set; }
        public string City { get; set; }
        public string AddressLine { get; set; }
    }
}