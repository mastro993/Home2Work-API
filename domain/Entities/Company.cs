using System;

namespace domain.Entities
{
    [Serializable]
    public class Company
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Domain { get; set; }
        public Address Address { get; set; }

        public DateTime Registration { get; set; }

      
    }
}