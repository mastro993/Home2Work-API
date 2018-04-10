using System;
using System.Collections.Generic;

namespace domain.Entities
{
    [Serializable]
    public class Share
    {
        public long Id { get; set; }
        public User Host { get; set; }
        public List<Guest> Guests { get; set; }
        public DateTime Time { get; set; }
        public ShareType Type { get; set; }
        public ShareStatus Status { get; set; }
        public double StartLat { get; set; }
        public double StartLng { get; set; }
        public double? EndLat { get; set; }
        public double? EndLng { get; set; }
        public int SharedDistance { get; set; }
    }

    public enum ShareType
    {
        Driver,
        Guest
    }

    public enum ShareStatus
    {
        Created,
        Completed,
        Canceled
    }
}