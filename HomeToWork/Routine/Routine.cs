using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeToWork.Routine
{
    class Routine
    {
        public long routineId { get; set; }
        public long userId { get; set; }
        public List<Location.Location> Route { get; set; }
        public DateTime ArrivalTime { get; set; }
        public DateTime DepartureTime { get; set; }
    }
}