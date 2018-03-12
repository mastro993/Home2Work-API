using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeToWork.Routine
{
    class Route
    {
        public Route()
        {
            Locations = new List<Location.Location>();
        }


        public List<Location.Location> Locations  { get; set; }

        public void Add(Location.Location location)
        {
            Locations.Add(location);
        }

        public int Size => Locations.Count;

        public Location.Location Last => Locations.Last();

    }
}