﻿using System;

namespace domain.Entities
{
    [Serializable]
    public class Guest
    {

        public long ShareId { get; set; }
        public User User { get; set; }
        public double StartLat { get; set; }
        public double StartLng { get; set; }
        public double? EndLat { get; set; }
        public double? EndLng { get; set; }
        public Status CurrentStatus { get; set; }
        public int Distance { get; set; }

        public enum Status
        {
            Joined, Completed, Leaved
        }

      
    }
}