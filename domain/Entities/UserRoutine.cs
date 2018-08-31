using System;
using System.Collections.Generic;
using System.Text;

namespace domain.Entities
{
    public class UserRoutine
    {
        public long UserId { get; set; }
        public double StartLat { get; set; }
        public double StartLng { get; set; }
        public TimeSpan StartTime { get; set; }
        public double EndLat { get; set; }
        public double EndLng { get; set; }
        public TimeSpan EndTime { get; set; }
        public RoutineType Type { get; set; }

        public enum RoutineType
        {
            Job,
            Home
        }
    }
}