using System;

namespace domain.Entities
{
    [Serializable]
    public class Guest
    {
        public static readonly int Joined = 0;
        public static readonly int Completed = 1;
        public static readonly int Canceled = 2;

        public long ShareId { get; set; }
        public User User { get; set; }
        public double StartLat { get; set; }
        public double StartLng { get; set; }
        public double? EndLat { get; set; }
        public double? EndLng { get; set; }
        public int Status { get; set; }
        public int Distance { get; set; }

      
    }
}