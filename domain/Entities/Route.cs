using System.Collections.Generic;
using System.Linq;

namespace domain.Entities
{
    public class Route
    {
        public Route()
        {
            Locations = new List<Location>();
        }


        public List<Location> Locations { get; set; }

        public void Add(Location location)
        {
            Locations.Add(location);
        }

        public int Size => Locations.Count;

        public Location Last => Locations.Last();

    }
}